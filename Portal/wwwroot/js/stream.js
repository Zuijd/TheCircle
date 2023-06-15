"use strict";

let mediaRecorder;
let recordedChunks = [];
let timer;
const timerInterval = 10000;

const connectionStream = new signalR.HubConnectionBuilder()
    .withUrl('/streamHub')
    .build();


// current Stream after create in DB 
var streamId
// Bool if Camera is on/off
var camBool
// Bool if stream in on/off
var streamBool
// Stream time
var startStream
var endStream
var durationStream
//  Live moments
var startLive
var endLive
// Break moments
var startBreak
var endBreak



function startStreaming() {
    if (!streamBool) {


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

                streamBool = camBool = true;
                startStream = startLive = new Date();
                FetchAddStream();

                startTimer();

                //// Trigger the dataavailable event every x seconds
                //timer = setInterval(() => {
                //    mediaRecorder.requestData();
                //}, timerInterval);



            })
            .catch(error => {
                console.error('Error accessing media devices:', error);
            });
    }
}


function startTimer() {
    timer = setInterval(() => {
        mediaRecorder.requestData();
    }, timerInterval);

}

function stopStreaming() {

    if (streamBool) {

        if (mediaRecorder && mediaRecorder.state !== 'inactive') {
            mediaRecorder.stop();
            clearInterval(timer);

            console.log('Recording stopped.');
        }

        if (camBool) {
            endStream = endLive = new Date();
            FetchAddLive()
        } else {
            endStream = endBreak = new Date();
            FetchAddBreak();
        }
        durationStream = endStream - startStream;

        FetchStopStreaming();
        stopCamera();
        streamBool = false;
    }

}

function stopCamera() {
    const videoElement = document.getElementById('video');
    const mediaStream = videoElement.srcObject;

    if (mediaStream) {
        const tracks = mediaStream.getTracks();

        tracks.forEach(track => track.stop());
        videoElement.srcObject = null;
    }
}


function startCamera() {
    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
            const videoElement = document.getElementById('video');
            videoElement.srcObject = stream;
        })
        .catch(error => {
            console.error('Error accessing media devices:', error);
        });
}

function switchCamera() {
    switch (true) {
        case camBool && streamBool:
            // Nothing should happen, because you are already streaming with camera on
            endLive = startBreak = new Date();
            stopCamera();
            // startBreak()
            camBool = false;
            console.log('Camera break')
            FetchAddLive();
            break;
        case camBool && !streamBool:
            // Camera is already on, so just stop the camera
            stopCamera()
            camBool = false
            console.log('Camera OFF')
            break;
        case !camBool && streamBool:
            // camBool to true & break should end and live should start
            endBreak = startLive = new Date();
            //endBreak();
            startCamera();
            startTimer();
            camBool = true;
            console.log('Camera wake up')
            FetchAddBreak();
            break;
        case !camBool && !streamBool:
            // camBool to true
            startCamera();
            camBool = true
            console.log('Camera ON')
            break;
        default:
        // code to be executed if no case matches
    }
}

function startBreak() {
    //Put line below in html where you want to display the video
    //<video id="breakVideo" style="display: none;"></video>
    // Show the break video
    //breakVideoElement.src = 'your_break_video_url.mp4'; // Replace with your break video URL or data
    //breakVideoElement.style.display = 'block';
    //breakVideoElement.play();
}

function endBreak() {
    // Hide the break video
    //breakVideoElement.style.display = 'none';
    //breakVideoElement.pause();
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


// Fetch calls
function FetchAddStream() {
    console.log('FetchAddStream: ')
    console.log('startStream: ' + startStream)
    console.log('startLive: ' + startLive)
    console.log('camBool: ' + camBool)

    fetch('/stream/AddStream', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            startStream: startStream
        })
    })
        .then(response => {
            if (response.ok) {
                // Streaming started successfully
                console.log('Streaming started!');
                return response.json(); // Parse response data as JSON
            } else {
                // Error occurred
                console.error('Failed to start streaming:', response.statusText);
                throw new Error('Failed to start streaming'); // Throw an error to handle it in the catch block
            }
        })
        .then(data => {
            // Handle the response data here
            streamId = data;
            console.log('Response data:', streamId);
        })
        .catch(error => {
            console.error('An error occurred while starting streaming:', error);
        });


}

function FetchStopStreaming() {
    console.log('FetchStopStream: ')

    console.log('DurationStream: ' + durationStream)

    fetch('/stream/StopStream', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            endStream: endStream,
            durationStream: durationStream
        })
    })
        .then(response => {
            if (response.ok) {
                // Streaming started successfully
                console.log('Streaming started!');
                return response.json(); // Parse response data as JSON
            } else {
                // Error occurred
                console.error('Failed to start streaming:', response.statusText);
                throw new Error('Failed to start streaming'); // Throw an error to handle it in the catch block
            }
        })
        .then(data => {
            // Handle the response data here
            console.log('Response data:', data);
        })
        .catch(error => {
            console.error('An error occurred while starting streaming:', error);
        });

}

function FetchAddBreak() {
    console.log('FetchAddBreak: ')
    if (camBool) {
        console.log('camBool: ' + camBool)
        console.log('StartLive: ' + startLive)

    } else {
        console.log('camBool: ' + camBool)
        console.log('endStream: ' + endStream)
    }
    console.log('endBreak : ' + endBreak)


    fetch('/stream/AddBreak', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            startBreak: startBreak,
            endBreak: endBreak
        })
    })
        .then(response => {
            if (response.ok) {
                // Streaming started successfully
                console.log('Streaming started!');
            } else {
                // Error occurred
                console.error('Failed to start streaming:', response.statusText);
            }
        })
        .then(data => {
            // Handle the response data here
            console.log('Response data:', data);
        })
        .catch(error => {
            console.error('An error occurred while starting streaming:', error);
        });
}

function FetchAddLive() {
    console.log('FetchAddLive: ')
    console.log('camBool: ' + camBool)
    console.log('endStream: ' + endStream)
    console.log('endLive : ' + endLive)

    fetch('/stream/AddLive', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            startLive: startLive,
            endLive: endLive
            })
        })
            .then(response => {
                if (response.ok) {
                    // Streaming started successfully
                    console.log('Streaming started!');
                } else {
                    // Error occurred
                    console.error('Failed to start streaming:', response.statusText);
                }
            })
            .then(data => {
                // Handle the response data here
                console.log('Response data:', data);
            })
            .catch(error => {
                console.error('An error occurred while starting streaming:', error);
            });

    }


