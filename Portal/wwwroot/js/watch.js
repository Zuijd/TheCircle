/*
 *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/streamHub").build();

connection.on("ReceiveStream", function (stream) {
    const video = document.querySelector('img');

    video.src = stream
});


connection.start().then(function () {
    console.log("Connected to stream");
    // connection.invoke("SendStreamServer", "Follower connected to stream").catch(function (err) {
    //     return console.error(err.toString());
    // });
}).catch(function (err) {
    return console.error(err.toString());
});