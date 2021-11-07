require('dotenv').config();

const express = require('express');
const azure = require('azure-storage');
const fs = require('fs');

const app = express();

if(!process.env.PORT) {
    throw new Error('Please specify the port number for the HTTP server with the environment variable PORT/');
}

const PORT = process.env.PORT;
const STORAGE_ACCOUNT_NAME = process.env.STORAGE_ACCOUNT_NAME;
const STORAGE_ACCESS_KEY = process.env.STORAGE_ACCESS_KEY;

function createBlobService() {
    const blobService = azure.createBlobService(STORAGE_ACCOUNT_NAME, STORAGE_ACCESS_KEY);
    return blobService;
} 

app.get('/', (req, res) => {
    res.send('Hello World!');
});

app.get('/video', (req, res) => {
    const videoPath = req.query.path; 
    const blobService = createBlobService();
    const containerName = "videoscontainer";

    blobService.getBlobProperties(containerName, videoPath, (err, properties) => {
        if(err) {
            res.sendStatus(500);
            return;
        }

        res.writeHead(200, {
            "Content-Length": properties.contentLength,
            "Content-Type": "video/mp4"
        });

        blobService.getBlobToStream(containerName, videoPath, res, err => {
            if(err) {
                res.sendStatus(500);
                return;
            }
        });
    });

    // const path = './videos/SampleVideo.mp4';

    // fs.stat(path, (err, stats) => {
    //     if(err) {
    //         console.error('An error occured');
    //         res.sendStatus(500);
    //         return;
    //     }

    //     res.writeHead(200, {
    //         'Content-Length': stats.size,
    //         'Content-Type': "video/mp4"
    //     });

    //     fs.createReadStream(path).pipe(res);
    // });
});

app.listen(PORT, () => {
    console.log(`Microservice storage app listening on port ${PORT}!`);
});

// For Command Prompt: set PORT=3001
// For Power Shell: $env:PORT=3001
// For Bash (Windows): export PORT=3001