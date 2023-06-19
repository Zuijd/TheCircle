//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
//var container = document.getElementById('stream-container');
//var username = container.getAttribute('data-user');

//var streamerId = 1;

//connection.on("ReceiveMessage", function (user, message, streamUserId) {
//    console.log("Recieved message " + user + " " + message + " " + streamUserId);

//    var li = document.createElement("li");
//    var messagesList = document.getElementById(`messagesList${streamUserId}`);

//    console.log(streamerId + streamUserId);

//    if (streamerId.toString() === streamUserId) {
//        li.innerHTML = `<b>${user}</b>: ${message}`;
//        li.id = 'message';
//        messagesList.appendChild(li)
//    }
//});

//connection.start().then(function () {
//    $(`#sendButton${streamerId}`).prop('disabled', false);
//    //document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//$(`#sendButton${streamerId}`).prop('disabled', true);


//$(document).on("click", `#sendButton${streamerId}`, function (event) {

//    var message = document.getElementById(`messageInput${streamerId}`).value;

//    connection.invoke("JoinGroup", streamerId.toString()).catch(function (err) {
//        return console.error(err.toString());
//    });
//    connection.invoke("SendMessage", username, message, streamerId.toString()).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();

//    var ChatViewModel = {
//        Message: $(`#messageInput${streamerId}`).val(),
//        ViewName: document.getElementById("viewName").value
//    };


//    var JSONData = JSON.stringify(ChatViewModel);

//    $.ajax({
//        url: '/Chat/Message',
//        data: JSONData,
//        type: "Post",
//        contentType: 'application/json;charset=utf-8',
//        success: function (result) {
//            console.log("d")
//        },
//        error: function (result) {
//            window.alert("oh");
//        }
//    });
//});

