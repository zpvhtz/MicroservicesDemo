const express = require("express");
const http = require("http");
const fs = require("fs");
const amqp = require('amqplib');

const RABBIT = process.env.RABBIT;

function connectRabbit() {
    console.log(`Connecting to RabbitMQ server at ${RABBIT}.`);

    // return amqp.connect(RABBIT) // Connect to the RabbitMQ server.
    //     .then(connection => {
    //         console.log("Connected to RabbitMQ.");

    //         return connection.createChannel(); // Create a RabbitMQ messaging channel.
    //     });

    return amqp.connect(RABBIT) // Connect to the RabbitMQ server.
        .then(connection => {
            console.log("Connected to RabbitMQ.");

            return connection.createChannel() // Create a RabbitMQ messaging channel.
                .then(messageChannel => {
                    return messageChannel.assertExchange("viewed", "fanout") // Assert that we have a "viewed" exchange.
                        .then(() => {
                            return messageChannel;
                        });
                });
        });
}

function sendViewedMessage(messageChannel, videoPath) {
    console.log(`Publishing message on "viewed" queue.`);

    const msg = { videoPath: videoPath };
    const jsonMsg = JSON.stringify(msg);
    messageChannel.publish("", "viewed", Buffer.from(jsonMsg)); // Queued
    // messageChannel.publish("viewed", "", Buffer.from(jsonMsg)); // Publish message to the "viewed" exchange.
}

//
// Setup event handlers.
//
function setupHandlers(app, messageChannel) {
    app.get("/api/video", (req, res) => { // Route for streaming video.
        const videoPath = "./videos/SampleVideo.mp4";

        fs.stat(videoPath, (err, stats) => {
            if (err) {
                console.error("An error occurred ");
                res.sendStatus(500);
                return;
            }
    
            res.writeHead(200, {
                "Content-Length": stats.size,
                "Content-Type": "video/mp4",
            });
    
            fs.createReadStream(videoPath).pipe(res);

            // sendViewedMessage(videoPath); // Send message to "history" microservice that this video has been "viewed".
            sendViewedMessage(messageChannel, videoPath); // Send message to "history" microservice that this video has been "viewed".
        });
    });
}

//
// Start the HTTP server.
//
function startHttpServer(messageChannel) {
    return new Promise((resolve, reject) => { // Wrap in a promise so we can be notified when the server has started.
        const app = express();
        setupHandlers(app, messageChannel);
        
        const port = process.env.PORT && parseInt(process.env.PORT) || 3000;
        app.listen(port, () => {
            resolve();
        });
    });
}

// function sendViewedMessage(videoPath) {
//     const postOptions = {
//         method: "POST",
//         headers: {
//             "Content-Type": "application/json"
//         }
//     };

//     const requestBody = {
//         videoPath: videoPath
//     };

//     const req = http.request("http://nodejs-history/viewed", postOptions);

//     req.on("close", () => {

//     });

//     req.on("error", (err) => {
//         console.error(err);
//     });
    
//     req.write(JSON.stringify(requestBody));
//     req.end();
// }

//
// Application entry point.
//
function main() {
    return connectRabbit()                          // Connect to RabbitMQ...
        .then(messageChannel => {                   // then...
            return startHttpServer(messageChannel); // start the HTTP server.
        });
}

main()
    .then(() => console.log("Microservice streaming app online."))
    .catch(err => {
        console.error("Microservice streaming app failed to start.");
        console.error(err && err.stack || err);
    });