"use strict";

const connectionStream = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub') // Adjust the URL to match your server endpoint
    .build();

let mediaSource;
let sourceBuffer;
<<<<<<< HEAD
=======
let receivedChunks = [];
let watcherCount = 0;
>>>>>>> origin/feature-uiIntegrations

connectionStream.on("ReceiveChunk", (chunk) => {
    const uint8Array = base64ToBytes(chunk);
    const blob = new Blob([uint8Array.buffer], { type: 'video/webm;codecs="vp9,opus"' });

    console.log(blob);
    console.log("Receiving chunk: " + chunk);

    appendToStream(blob);
});

<<<<<<< HEAD
async function appendToStream(blob) {
    if (!sourceBuffer || sourceBuffer.updating) {
        return;
=======
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
>>>>>>> origin/feature-uiIntegrations
    }

    const vidBuff = await blob.arrayBuffer();

    sourceBuffer.appendBuffer(vidBuff);
}
function updateWatcherCountUI() {
    const watcherCountElement = document.getElementById('watcherCount');
    watcherCountElement.textContent = (watcherCount - 1).toString();
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

connectionStream.start()
    .then(() => {

        var userName = window.location.pathname.split("/")[3];

        if (!userName) {
            console.log("No user name provided.");
        }


        connectionStream.invoke("JoinGroup", userName).catch(err => {
            console.log(err)
        })
        
        console.log('Connection established.');

        const videoElement = document.getElementById('video');

        // Check if MediaSource is supported by the browser
        if (!window.MediaSource) {
            console.error('MediaSource is not supported in this browser.');
            return;
        }
        
        mediaSource = new MediaSource();
        videoElement.src = URL.createObjectURL(mediaSource);

        mediaSource.addEventListener('sourceopen', () => {
            sourceBuffer = mediaSource.addSourceBuffer(`video/webm; codecs="vp9,opus"`);
            console.log('Source buffer is ready.');
        });

        videoElement.addEventListener('play', () => {
            videoElement.play().catch((error) => {
                console.error('Error starting video playback:', error);
            });
        });
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });