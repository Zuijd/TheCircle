"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub') // Adjust the URL to match your server endpoint
    .build();

let mediaSource;
let sourceBuffer;
let receivedChunks = [];
let isAppending = false;

connection.on("ReceiveChunk", (chunk) => {
    const videoElement = document.getElementById('video');

    console.log("New data available: " + chunk);

    if (!mediaSource) {
        mediaSource = new MediaSource();
        videoElement.src = URL.createObjectURL(mediaSource);
        mediaSource.addEventListener('sourceopen', handleSourceOpen);
    }

    receivedChunks.push(chunk);

    if (sourceBuffer && !isAppending) {
        processChunks();
    }
});

connection.start()
    .then(() => {
        console.log('Connection established.');
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });

function handleSourceOpen() {
    console.log('MediaSource opened.');
    sourceBuffer = mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
    sourceBuffer.addEventListener('updateend', handleUpdateEnd);
    processChunks();
}

function handleUpdateEnd() {
    console.log('handleUpdateEnd called.');
    isAppending = false;
    processChunks();
}

function processChunks() {
    console.log('processChunks called.');
    if (sourceBuffer && !isAppending && receivedChunks.length > 0) {
        isAppending = true;
        const chunksToAppend = receivedChunks.splice(0, receivedChunks.length);
        const mergedChunk = mergeChunks(chunksToAppend);
        const uint8Array = base64ToBytes(mergedChunk);
        console.log('Appending chunk:', mergedChunk);
        sourceBuffer.appendBuffer(uint8Array);
    }
}

function mergeChunks(chunks) {
    return chunks.join('');
}

function base64ToBytes(base64) {
    const binaryString = window.atob(base64);
    const length = binaryString.length;
    const bytes = new Uint8Array(length);

    for (let i = 0; i < length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    return bytes;
}
