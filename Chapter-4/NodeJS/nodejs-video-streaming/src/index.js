require('dotenv').config();

const express = require('express');
const http = require('http');
const mongodb = require('mongodb');
const fs = require('fs');

const app = express();
// const port = 3001;

if(!process.env.PORT) {
    throw new Error('Please specify the port number for the HTTP server with the environment variable PORT/');
}

const port = process.env.PORT;
const VIDEO_STORAGE_HOST = process.env.VIDEO_STORAGE_HOST;
const VIDEO_STORAGE_PORT = parseInt(process.env.VIDEO_STORAGE_PORT);
const DBHOST = process.env.DBHOST;
const DBNAME = process.env.DBNAME;

function main() {
    console.log('test')
    console.warn('warn')

    return mongodb.MongoClient.connect(DBHOST).then(client => {
        const db = client.db(DBNAME);
        const videosCollection = db.collection("videos");

        app.get("/api/video", (req, res) => {
            // const videoId = new mongodb.ObjectID(req.query.id);
            const videoId = parseInt(req.query.id);
            console.log(videoId)
            videosCollection.findOne({ _id: videoId })
                            .then(videoRecord => {
                                if(!videoRecord) {
                                    res.sendStatus(404);
                                    return;
                                }
                                
                                const forwardRequest = http.request({
                                    host: VIDEO_STORAGE_HOST,
                                    port: VIDEO_STORAGE_PORT,
                                    path: `/video?path=${videoRecord.videoPath}`,
                                    method: 'GET',
                                    headers: req.headers
                                }, forwardResponse => {
                                    res.writeHeader(forwardResponse.statusCode, forwardResponse.headers);
                                    forwardResponse.pipe(res);
                                });

                                req.pipe(forwardRequest);
                            })
                            .catch(err => {
                                console.error("Database query failed.");
                                console.error(err && err.stack || err);
                                res.sendStatus(500);
                            });
        });

        app.get('/', (req, res) => {
            res.send('Hello World!');
        });

        app.listen(port, () => {
            console.log(`Microservice streaming app listening on port ${port}!`)
        })

        // app.get('/api/video', (req, res) => {
        //     const forwardRequest = http.request({
        //         host: VIDEO_STORAGE_HOST,
        //         port: VIDEO_STORAGE_PORT,
        //         path: '/video?path=SampleVideo.mp4',
        //         method: 'GET',
        //         headers: req.headers
        //     }, forwardResponse => {
        //         res.writeHeader(forwardResponse.statusCode, forwardResponse.headers);
        //         forwardResponse.pipe(res);
        //     });
            
        //     req.pipe(forwardRequest);
        // });
    });
}

main()
    .then(() => console.log('Microservice streaming app online.'))
    .catch(err => {
        console.error("Microservice failed to start.");
        console.error(err && err.stack || err);
    });

// For Command Prompt: set PORT=3001
// For Power Shell: $env:PORT=3001
// For Bash (Windows): export PORT=3001