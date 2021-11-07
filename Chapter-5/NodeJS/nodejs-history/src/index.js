require('dotenv').config();

const express = require('express');
const http = require('http');
const fs = require('fs');
const bodyParser = require('body-parser');
const mongodb = require('mongodb');
const amqp = require("amqplib");

if(!process.env.PORT) {
    throw new Error('Please specify the port number for the HTTP server with the environment variable PORT');
}

const port = process.env.PORT;
const DBHOST = process.env.DBHOST;
const DBNAME = process.env.DBNAME;
const RABBIT = process.env.RABBIT;

function connectRabbit() {
    console.log(`Connecting to RabbitMQ server at ${RABBIT}.`);

    return amqp.connect(RABBIT).then(messagingConnection => {
        console.log("Connected to RabbitMQ.");
        return messagingConnection.createChannel();
    });
}

function connectDb() {
    return mongodb.MongoClient.connect(DBHOST) 
        .then(client => {
            return client.db(DBNAME);
        });
}

function setupHandlers(app, db, messageChannel) {
    const videosCollection = db.collection("videos");

    function consumeViewedMessage(msg) {
        console.log("Received a 'viewed' message in history");

        const parsedMsg = JSON.parse(msg.content.toString());
       
        return videosCollection.insertOne({ videoPath: parsedMsg.videoPath }).then(() => {
            messageChannel.ack(msg);
        });
    };
    // app.post("/viewed", (req, res) => {
    //     const videoPath = req.body.videoPath;

    //     videosCollection.insertOne({ videoPath: videoPath })
    //                     .then(() => {
    //                         console.log(`Added video ${videoPath} to history.`);
    //                         res.sendStatus(200);
    //                     })
    //                     .catch(err => {
    //                         console.error(`Error adding video ${videoPath} to history`);
    //                         console.error(err && err.stack || err);
    //                         res.sendStatus(500);
    //                     });
    // });

    //Queue
    // return messageChannel.assertQueue("viewed", {}).then(() => {
    //     return messageChannel.consume("viewed", consumeViewedMessage);
    // });

    //Exchange
    return messageChannel.assertExchange("viewed", "fanout").then(() => {
        return messageChannel.assertQueue("", { exclusive: true });
    }).then(response => {
        const queueName = response.queue;
        console.log(`Created queue ${queueName}, binding it to "viewed" exchange.`);
        return messageChannel.bindQueue(queueName, "viewed", "").then(() => {
            return messageChannel.consume(queueName, consumeViewedMessage);
        });
    });
}

function startHttpServer(db, messageChannel) {
    return new Promise(resolve => {
        const app = express();
        app.use(bodyParser.json());
        setupHandlers(app, db, messageChannel);
    
        const port = process.env.PORT && parseInt(process.env.PORT) || 3003;
        app.listen(port, () => {
            resolve();
        });
    });
}

function main() {
    return connectDb(DBHOST)            // Connect to the database...
        .then(db => {                   // then...
            return connectRabbit().then(messageChannel => {
                return startHttpServer(db, messageChannel); // start the HTTP server.
            })
        });
}

main()
    .then(() => console.log('Microservice history app online.'))
    .catch(err => {
        console.error("Microservice history app failed to start.");
        console.error(err && err.stack || err);
    });

// For Command Prompt: set PORT=3001
// For Power Shell: $env:PORT=3001
// For Bash (Windows): export PORT=3001