"use strict";

var chatConnection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var currentUser = document.getElementById('stream-container').getAttribute('data-user');
var path = window.location.pathname.split("/").pop();

chatConnection.on("ReceiveMessage", function (message, username, streamUser) {

    console.log("RECEIVED MESSAGE")
    if (streamUser == "Stream") {
        streamUser = path;
    }
    
    console.log("Received message from " + username + ": " + message + " (stream: " + streamUser + ")");

    var li = document.createElement("li");
    var messagesList = document.getElementById(`messagesList`);

    li.innerHTML = `<b>${username}</b>: ${message}`;
    li.id = 'message';
    messagesList.appendChild(li)
});


$(document).on("click", `#sendButton`, function (event) {

    var streamUser = path == "Stream" ? currentUser : path;
    
    var message = document.getElementById(`messageInput`).value;

    console.log("message: ", message);
    console.log("Currentuser: ", currentUser);
    console.log("StreamUser: ", streamUser);
    
    chatConnection.invoke("SendMessage", message, currentUser, streamUser).catch(function (err) {
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
            console.log("Succes sending message")
        },
        error: function (result) {
            window.alert("Error sending message");
        }
    });
});

chatConnection.start()
    .then(() => {

        if (path.toLowerCase() != "stream") {
            chatConnection.invoke("JoinGroup", path).catch(err => {
                console.log(err)
            })
        } else {
            chatConnection.invoke("JoinGroup", currentUser).catch(err => {
                console.log(err)
            })
        }

        $(`#sendButton`).prop('disabled', false);

        console.log("Connection established.");

        //document.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });
