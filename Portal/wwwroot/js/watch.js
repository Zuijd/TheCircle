"use strict";

const connectionStream = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub') // Adjust the URL to match your server endpoint
    .build();

let mediaSource;
let sourceBuffers = {};

connectionStream.on("ReceiveChunk", (streamId, chunk) => {
    const uint8Array = base64ToBytes(chunk);
    const blob = new Blob([uint8Array.buffer], { type: 'video/webm;codecs="vp9,opus"' });

    console.log(blob);
    console.log("Receiving chunk: " + chunk);

    appendToStream(streamId, blob);
});

async function appendToStream(streamId, blob) {
    if (!sourceBuffers[streamId] || sourceBuffers[streamId].updating) {
        return;
    }

    const reader = new FileReader();

    reader.onloadend = () => {
        const arrayBuffer = reader.result;
        sourceBuffers[streamId].appendBuffer(arrayBuffer);
    };

    reader.readAsArrayBuffer(blob);
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
        console.log('Connection established.');

        const videoContainer = document.getElementById('video-container');

        // Check if MediaSource is supported by the browser
        if (!window.MediaSource) {
            console.error('MediaSource is not supported in this browser.');
            return;
        }

        mediaSource = new MediaSource();
        videoContainer.appendChild(mediaSource);

        mediaSource.addEventListener('sourceopen', () => {
            console.log('MediaSource sourceopen event triggered.');

            const streamIds = connectionStream.invoke("GetStreamIds").catch(error => {
                console.error("Error getting stream IDs:", error);
            });

            streamIds.then(ids => {
                ids.forEach(streamId => {
                    createSourceBuffer(streamId);
                });
            });
        });

        mediaSource.addEventListener('error', (e) => {
            console.error('MediaSource error:', e);
        });
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });

function createSourceBuffer(streamId) {
    const sourceBuffer = mediaSource.addSourceBuffer('video/webm; codecs="vp9,opus"');

    sourceBuffer.addEventListener('updateend', () => {
        console.log(`Source buffer ${streamId} updateend event triggered.`);
    });

    sourceBuffers[streamId] = sourceBuffer;
}
