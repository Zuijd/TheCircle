"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var container = document.getElementById('stream-container');
var username = container.getAttribute('data-user');
connection.on("ReceiveMessage", function (user, message, streamUserId) {
    console.log("Recieved message " + user + " " + message + " " + streamUserId);

    var li = document.createElement("li");
    var messagesList = document.getElementById("messagesList");

    var streamerId = messagesList.getAttribute('data-streamerId')

    console.log(streamerId + streamUserId);

    if (streamerId === streamUserId) {
        li.innerHTML = `<b>${user}</b>: ${message}`;
        li.id = 'message';
        messagesList.appendChild(li)
    }
});

connection.start().then(function () {
    $("#sendButton").prop('disabled', false);
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$("#sendButton").prop('disabled', true);


$(document).on("click", "#sendButton", function (event) {
    var streamerId = 2;

    var message = document.getElementById("messageInput").value;

    connection.invoke("JoinGroup", streamerId.toString()).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("SendMessage", username, message, streamerId.toString()).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();

    var ChatViewModel = {
        Message: $("#messageInput").val(),
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

