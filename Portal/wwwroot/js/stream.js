/*
 *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/streamHub").build();

// Put variables in global scope to make them available to the browser console.
const constraints = window.constraints = {
    audio: true,
    video: true
};

connection.start();

function handleSuccess(stream) {
    const video = document.querySelector('video');
    
    // const videoTracks = stream.getVideoTracks();
    // console.log('Got stream with constraints:', constraints);
    // console.log(`Using video device: ${videoTracks[0].label}`);

    // video.srcObject = stream;

    navigator.mediaDevices.getUserMedia(constraints).then((stream) => video.srcObject = stream);

    const getFrame = () => {
        const canvas = document.createElement('canvas');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        canvas.getContext('2d').drawImage(video, 0, 0);
        const data = canvas.toDataURL('image/png');
        return data;
    }

    const FPS = 60;
    setInterval(() => {
        console.log("Sending stream")
        connection.invoke("SendStream", getFrame()).catch(function (err) {
            return console.error(err);
        });
    }, 1000 / FPS);

}

function handleError(error) {
    if (error.name === 'OverconstrainedError') {
        const v = constraints.video;
        errorMsg(`The resolution ${v.width.exact}x${v.height.exact} px is not supported by your device.`);
    } else if (error.name === 'NotAllowedError') {
        errorMsg('Permissions have not been granted to use your camera and ' +
            'microphone, you need to allow the page access to your devices in ' +
            'order for the demo to work.');
    }
    errorMsg(`getUserMedia error: ${error.name}`, error);
}

function errorMsg(msg, error) {
    const errorElement = document.querySelector('#errorMsg');
    console.log(msg)
    errorElement.innerHTML += `<p>${msg}</p>`;
    if (typeof error !== 'undefined') {
        console.error(error);
    }
}

async function init(e) {
    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        handleSuccess(stream);
        e.target.disabled = true;
    } catch (e) {
        handleError(e);
    }
}

document.querySelector('#startStream').addEventListener('click', e => init(e));