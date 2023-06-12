"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub') // Adjust the URL to match your server endpoint
    .build();

let mediaSource;
// let sourceBuffer = null;
// let receivedBlobs = [];

connection.on("ReceiveChunk", (chunk) => {
    
    const uint8Array = base64ToBytes(chunk);
    const blob = new Blob([uint8Array.buffer], { type: 'video/webm;codecs="vp9,opus"' });

    console.log(blob)
    console.log("Receiving chunk: " + chunk);
    
    appendToStream(blob);
});

async function appendToStream(blob) {

    const videoElement = document.getElementById('video');

    const vidBuff = await blob.arrayBuffer();

    const sourceBuffer = await new Promise((resolve, reject) => {
		const getSourceBuffer = () => {
			try {
				const sourceBuffer = mediaSource.addSourceBuffer(`video/webm; codecs="vp9,opus"`);
				resolve(sourceBuffer);
			} catch (e) {
				reject(e);
			}
		};
		if (mediaSource.readyState === 'open') {
			getSourceBuffer();
		} else {
			mediaSource.addEventListener('sourceopen', getSourceBuffer);
		}
	});

    // Now that we have an "open" source buffer, we can append to it
	sourceBuffer.appendBuffer(vidBuff);
	// Listen for when append has been accepted and
	// You could alternative use `.addEventListener` here instead
	sourceBuffer.onupdateend = () => {
        console.log("UPDATE")
		// Nothing else to load
		mediaSource.endOfStream();
		// Start playback!
		// Note: this will fail if video is not muted, due to rules about
		// autoplay and non-muted videos
		videoElement.play();
	};

    	// Debug Info
	console.log({
		sourceBuffer,
		mediaSource,
		videoElement
	});
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

connection.start()
    .then(() => {
        console.log('Connection established.');

        const videoElement = document.getElementById('video');

        // Controleer of MediaSource wordt ondersteund door de browser
        if (!window.MediaSource) {
            console.error('MediaSource wordt niet ondersteund in deze browser.');
            return;
        }

        mediaSource = new MediaSource();
        videoElement.src = URL.createObjectURL(mediaSource);

        // startVideoStream()
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });