"use strict";

var chatConnection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var container = document.getElementById('stream-container');
var username = container.getAttribute('data-user');

// Get current URL
var currentUrl = window.location.href;

// Create a new URL object
var url = new URL(currentUrl);

// Extract the parameter value
var streamerParam = url.pathname.split('/').pop();

console.log(streamerParam);

chatConnection.on("ReceiveMessage", function (user, message, streamUser) {
    console.log("Recieved message " + user + " " + message + " " + streamUser);

    var li = document.createElement("li");
    var messagesList = document.getElementById(`messagesList`);

    console.log(streamerParam + streamUser);

    if (streamerParam === streamUser) {
        li.innerHTML = `<b>${user}</b>: ${message}`;
        li.id = 'message';
        messagesList.appendChild(li)
    }
});

chatConnection.start().then(function () {
    $(`#sendButton`).prop('disabled', false);
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$(`#sendButton`).prop('disabled', true);


$(document).on("click", `#sendButton`, function (event) {

    var message = document.getElementById(`messageInput`).value;

    chatConnection.invoke("JoinGroup", streamerParam).catch(function (err) {
        return console.error(err.toString());
    });
    chatConnection.invoke("SendMessage", username, message, streamerParam).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();

    var ChatViewModel = {
        Message: $(`#messageInput`).val(),
        ViewName: document.getElementById("viewName").value
    };


    var JSONData = JSON.stringify(ChatViewModel);

    $.ajax({
        url: '/Chat/Message',
        data: JSONData,
        type: "Post",
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            console.log("d")
        },
        error: function (result) {
            window.alert("oh");
        }
    });
});

