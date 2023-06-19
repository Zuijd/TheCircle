"use strict";

$(document).ready(function () {
  $("#startStreamBtn").click(function () {
    var button = $(this);
    var streamBox = $("#streamBox");
    var circleImage = $("#circleImage");
    var chatBox = $("#chatBox");

    if (button.hasClass("btn-success")) {
      button
        .removeClass("btn-success")
        .addClass("btn-danger")
        .text("Stop Streaming");
      streamBox.show();
      circleImage.hide();
      startStreaming();
      chatBox.show();
    } else {
      button
        .removeClass("btn-danger")
        .addClass("btn-success")
        .text("Start Streaming");
      streamBox.hide();
      circleImage.show();
      stopStreaming();
      chatBox.hide();
    }
  });
});

let mediaRecorder;
let recordedChunks = [];
let timer;
const timerInterval = 5000;
let watcherCount = 0;

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message, user) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    //li.textContent = `${user} says ${message}`;
    li.textContent = `${message}`;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function updateWatcherCountUI() {
  const watcherCountElement = document.getElementById("watcherCount");
  watcherCountElement.textContent = (watcherCount - 1).toString();
}

connection.start()
    .then(() => {
        document.getElementById("sendButton").disabled = false;
    }).catch((error) => {
        console.error(error.toString());
    });