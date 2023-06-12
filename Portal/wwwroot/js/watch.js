"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub') // Adjust the URL to match your server endpoint
    .build();

let mediaSource;
let sourceBuffer;
let receivedChunks = [];
let watcherCount = 0;

connection.on("ReceiveChunk", (chunk) => {
    const videoElement = document.getElementById('video');

    console.log("New data available: " + chunk);

    if (!mediaSource) {
        mediaSource = new MediaSource();
        videoElement.src = URL.createObjectURL(mediaSource);
        mediaSource.addEventListener('sourceopen', handleSourceOpen);
    }

    receivedChunks.push(chunk);

    if (sourceBuffer && !sourceBuffer.updating) {
        appendNextChunk();
    }
});

connection.on('UpdateWatcherCount', (count) => {
    watcherCount = count;
    updateWatcherCountUI();
})

connection.start()
    .then(() => {
        console.log('Connection established.');
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });

function handleSourceOpen() {
    sourceBuffer = mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
    appendNextChunk();
}

function appendNextChunk() {
    if (sourceBuffer && !sourceBuffer.updating && receivedChunks.length > 0) {
        const chunk = receivedChunks.shift();
        const uint8Array = base64ToBytes(chunk);
        const chunkData = new Uint8Array(uint8Array.buffer);
        sourceBuffer.appendBuffer(chunkData);
    }
}
function updateWatcherCountUI() {
    const watcherCountElement = document.getElementById('watcherCount');
    watcherCountElement.textContent = (watcherCount - 1).toString();
}

function base64ToBytes(base64) {
    const base64abc = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    const padding = '=';

    let result = [];
    let bytes = (base64.length / 4) * 3;
    if (base64[base64.length - 1] === padding) {
        bytes--;
        if (base64[base64.length - 2] === padding) {
            bytes--;
        }
    }

    for (let i = 0, j = 0; i < base64.length; i += 4, j += 3) {
        const char1 = base64abc.indexOf(base64[i]);
        const char2 = base64abc.indexOf(base64[i + 1]);
        const char3 = base64abc.indexOf(base64[i + 2]);
        const char4 = base64abc.indexOf(base64[i + 3]);

        const byte1 = (char1 << 2) | (char2 >> 4);
        const byte2 = ((char2 & 15) << 4) | (char3 >> 2);
        const byte3 = ((char3 & 3) << 6) | char4;

        result[j] = byte1;
        if (char3 !== 64) {
            result[j + 1] = byte2;
        }
        if (char4 !== 64) {
            result[j + 2] = byte3;
        }
    }

    return new Uint8Array(result);
}
