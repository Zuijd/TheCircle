"use strict";

let mediaRecorder;
let recordedChunks = [];
let timer;
const timerInterval = 10000;

const connectionStream = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub')
    .build();

function startStreaming() {

    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
            const videoElement = document.getElementById('video');
            videoElement.srcObject = stream;

            mediaRecorder = new MediaRecorder(stream, { mimeType: 'video/webm; codecs=vp9,opus', timeslice: timerInterval });

            mediaRecorder.addEventListener('dataavailable', event => {
                recordedChunks.push(event.data);
                sendBlob(event.data);
            });

            mediaRecorder.addEventListener('stop', () => {
                const recordedBlob = new Blob(recordedChunks, { type: 'video/webm' });
                const url = URL.createObjectURL(recordedBlob);

                // const a = document.createElement('a');
                // a.href = url;
                // a.download = 'stream.webm';
                // document.body.appendChild(a);
                // a.click();

                recordedChunks = [];
                URL.revokeObjectURL(url);
            });

            mediaRecorder.start();
            console.log('Recording started.');

            // Trigger the dataavailable event every x seconds
            timer = setInterval(() => {
                mediaRecorder.requestData();
            }, timerInterval);

        })
        .catch(error => {
            console.error('Error accessing media devices:', error);
        });
}

function stopStreaming() {
    if (mediaRecorder && mediaRecorder.state !== 'inactive') {
        mediaRecorder.stop();
        clearInterval(timer);
        console.log('Recording stopped.');
    }
}

function sendBlob(chunk) {
    const reader = new FileReader();
    reader.onloadend = () => {
        const buffer = reader.result;
        const uint8Array = new Uint8Array(buffer);
        const base64String = bytesToBase64(uint8Array);
        console.log("Sending chunk: " + base64String);
        connectionStream.invoke("SendChunk", base64String).catch(error => {
            console.error("Error sending Blob: ", error);
        });
    };
    reader.readAsArrayBuffer(chunk);
}

// Function to convert uint8array to base64
function bytesToBase64(bytes) {
    let result = '';
    const base64abc = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    const padding = '=';

    for (let i = 0; i < bytes.length; i += 3) {
        const byte1 = bytes[i];
        const byte2 = bytes[i + 1];
        const byte3 = bytes[i + 2];

        const char1 = byte1 >> 2;
        const char2 = ((byte1 & 3) << 4) | (byte2 >> 4);
        const char3 = ((byte2 & 15) << 2) | (byte3 >> 6);
        const char4 = byte3 & 63;

        result +=
            base64abc.charAt(char1) +
            base64abc.charAt(char2) +
            base64abc.charAt(char3) +
            base64abc.charAt(char4);
    }

    // Handle padding
    const lastByteCount = bytes.length % 3;
    if (lastByteCount === 1) {
        result = result.slice(0, -2) + padding + padding;
    } else if (lastByteCount === 2) {
        result = result.slice(0, -1) + padding;
    }

    return result;
}

connectionStream.start()
    .then(() => {
        // Connection is established, ready to send/receive signaling messages
        console.log('ConnectionChat established.');
    })
    .catch(error => {
        console.error('Error starting the signaling connection:', error);
    });

var connectionChat = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connectionChat.on("ReceiveMessage", function (message, user) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    //li.textContent = `${user} says ${message}`;
    li.textContent = `${message}`;
});

connectionChat.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connectionChat.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
