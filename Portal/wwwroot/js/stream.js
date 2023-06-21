"use strict";

let mediaRecorder;
let recordedChunks = [];
let timer;
const timerInterval = 5000;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/streamHub") // Adjust the URL to match your server endpoint
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

// HTML Buttons
function startButton() {
    if (!streamBool) {
        streamBool = camBool = true;
        startStream = startLive = new Date();
        startSatoshiTimer();
        startStreamTimer();
        startStreaming();
        FetchAddStream();
    }
}

function stopButton() {
    if (streamBool) {
        stopSatoshiTimer();
        stopStreamTimer();
        stopCamera();
        stopStreaming();
        durationStream = endStream - startStream;
        FetchStopStreaming();
        FetchAddSatoshi();
        streamBool = camBool = false;
    }
}

function switchCamera() {
    switch (true) {
        case camBool && streamBool:
            // Pauzing stream when live
            endLive = startBreak = new Date();
            stopSatoshiTimer();
            stopCamera();
            stopStreaming();
            camBool = false;
            console.log('Camera break')
            FetchAddLive();
            break;
        case camBool && !streamBool:
            // Not streaming, turn camera off
            stopCamera()
            camBool = false
            console.log('Camera OFF')
            break;
        case !camBool && streamBool:
            // Going back live
            endBreak = startLive = new Date();
            startSatoshiTimer();
            startStreaming();
            camBool = true;
            console.log('Camera wake up')
            FetchAddBreak();
            break;
        case !camBool && !streamBool:
            // not streaming, turn camera on
            startCamera();
            camBool = true
            console.log('Camera ON')
            break;
        default:
        // code to be executed if no case matches
    }
}

function startStreaming() {
    navigator.mediaDevices
        .getUserMedia({ video: true, audio: true })
        .then((stream) => {
            const videoElement = document.getElementById("video");
            videoElement.srcObject = stream;

            mediaRecorder = new MediaRecorder(stream, { mimeType: 'video/webm; codecs=vp9,opus', timeslice: timerInterval });

            mediaRecorder.addEventListener("dataavailable", (event) => {
                console.log("New data available: " + event.data.size);
                recordedChunks.push(event.data);
                sendBlob(event.data);
            });

            mediaRecorder.addEventListener("stop", () => {
                const recordedBlob = new Blob(recordedChunks, { type: "video/webm" });
                const url = URL.createObjectURL(recordedBlob);

                //const a = document.createElement("a");
                //a.href = url;
                //a.download = "stream.webm";
                //document.body.appendChild(a);
                //a.click();

                recordedChunks = [];
                URL.revokeObjectURL(url);
            });

            mediaRecorder.start();
            console.log("Recording started.");

            // Trigger the dataavailable event every x seconds
            startTimer();
        })
        .catch((error) => {
            console.error("Error accessing media devices:", error);
        });
}

function stopStreaming() {
    if (mediaRecorder && mediaRecorder.state !== "inactive") {
        mediaRecorder.stop();
        clearInterval(timer);
        console.log("Recording stopped.");
    }

    if (camBool) {
        endStream = endLive = new Date();
        FetchAddLive()
    } else {
        endStream = endBreak = new Date();
        FetchAddBreak();
    }
    durationStream = endStream - startStream;
}

function startTimer() {
    mediaRecorder.requestData();

    timer = setInterval(() => {
        mediaRecorder.requestData();
    }, timerInterval);

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

function sendBlob(chunk) {

    const reader = new FileReader();
    reader.onloadend = () => {
        const buffer = reader.result;
        const uint8Array = new Uint8Array(buffer);
        const base64String = bytesToBase64(uint8Array);
        var JSONData = JSON.stringify(base64String);

        // Create a Blob from the byte array
        const blob = new Blob([uint8Array], { type: 'application/octet-stream' });

        $.ajax({
            url: "/Stream/SaveChunk",
            data: blob, 
            type: "POST",
            contentType: "application/octet-stream", 
            processData: false
        });
        
        console.log("Sending chunk: " + base64String);
        connection.invoke("SendChunk", base64String).catch(error => {
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

connection.start()
    .then(() => {
        // Connection is established, ready to send/receive signaling messages
        console.log("Connection established.");
    })
    .catch((error) => {
        console.error("Error starting the signaling connection:", error);
    });

//create new signalR Hub connection
const watcherHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/watcherHub")
    .build();

//when UpdateWatcherCount has been called -> update viewer count
watcherHubConnection.on("UpdateWatcherCount", (count) => {
    //adjust innerHTML
    document.getElementById("watcherCount").textContent = (count).toString() + " people watching the stream.";
});

//start stream over Hub connection
watcherHubConnection.start()
    .then(() => {
        // Connection is established, ready to send/receive signaling messages
        console.log("Connection established.");
    })
    .catch((error) => {
        console.error("Error starting the signaling connection:", error);
    });


// Fetch calls
function FetchAddStream() {
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
    fetch('/stream/StopStream', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            endStream: endStream,
            durationStream: durationStream,
            earnedSatoshi: earningsBeforeBreak.toFixed(8).toString()
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

function FetchAddSatoshi() {
    console.log('FetchAddSatoshi is called!')
    fetch('/user/AddSatoshi', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            earnedSatoshi: earningsBeforeBreak.toFixed(8).toString()
        })
    })
        .then(response => {
            if (response.ok) {
                console.log('AddBalance to user');
            } else {
                console.log('Failed to add satoshi balance')
            }
        })
        .then(data => {
            // Handle the response data here
            console.log('Response data:', data);
        })
        .catch(error => {
            console.error('An error occurred while adding satoshi:', error);
        });
}

// Satoshi tracking
let initialBalance = 0.00000001;
let earningPerInterval = 0.00000001; // Initial earning per interval

// Timer interval for updating the balance (1 hour = 3600000 milliseconds)
const timerIntervalS = 5000;

let balance = 0; // Balance starts from 0 initially
let satoshiTimer;
let breakSatoshi = false;
let earningsBeforeBreak = 0;

function startSatoshiTimer() {
    balance += initialBalance; // Add initial balance when the timer starts

    if (breakSatoshi) {
        // Reset balance to initial balance after a break
        balance = initialBalance;
        breakSatoshi = false;
        $('#balanceDisplay').text(
            (balance + earningsBeforeBreak).toFixed(8) + ' Satoshi'
        )
    }

    satoshiTimer = setInterval(() => {
        balance = balance * 2;

        $('#balanceDisplay').text(
            (balance + earningsBeforeBreak).toFixed(8) + ' Satoshi'
        );
    }, timerIntervalS);
}

function stopSatoshiTimer() {
    clearInterval(satoshiTimer);
    earningsBeforeBreak += balance; // Add the current balance to earnings before break
    breakSatoshi = true;
}

// StreamTimer
let streamTimer;
let streamStartTime;


function startStreamTimer() {
    streamStartTime = new Date().getTime();
    streamTimer = setInterval(updateStreamTimer, 1000);
}
function stopStreamTimer() {
    clearInterval(streamTimer);
}

function updateStreamTimer() {
    const currentTime = new Date().getTime();
    const elapsedSeconds = Math.floor((currentTime - streamStartTime) / 1000);

    // Format the elapsed time into HH:MM:SS format
    const hours = Math.floor(elapsedSeconds / 3600);
    const minutes = Math.floor((elapsedSeconds % 3600) / 60);
    const seconds = elapsedSeconds % 60;
    const formattedTime = `${padZero(hours)}:${padZero(minutes)}:${padZero(seconds)}`;

    // Update the stream timer display
    document.getElementById('streamTimer').textContent = formattedTime;
}

function padZero(number) {
    return number.toString().padStart(2, '0');
}



