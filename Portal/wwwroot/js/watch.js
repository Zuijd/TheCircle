"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var container = document.getElementById('streams-container');
var username = container.getAttribute('data-user');
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `<b>${user}</b>: ${message}`;
    li.id = 'message';
});

connection.start().then(function () {
    $("#sendButton").prop('disabled', false);
    //document.getElementById("sendButton").disabled = false;
    console.log('Button set to enabled')
}).catch(function (err) {
    return console.error(err.toString());
});

console.log('Button set to disabled')
$("#sendButton").prop('disabled', true);


$(document).on("click", "#sendButton", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", username, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", '@username', message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});
//}

$(function () {
    $("#sendButton").click(function () {
        var ChatViewModel = {
            Message: $("#messageInput").val()
        };

        var JSONData = JSON.stringify(ChatViewModel);

        $.ajax({
            url: '@Url.Action("Message","Home")',
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
})

