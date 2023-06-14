$(document).ready(function () {
    // Add an h1 element to show "Please select a streamer" initially
    const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text('Please select a streamer');
    $('#streamsContainer').append(selectStreamerTitle);

    // Show the circle image in the center of the website initially
    $('#circleImage').show();

    $('#viewStreamsBtn').click(function () {
        // Clear the streamsContainer before adding new streamers
        $('#streamsContainer').empty();

        const selectedStreamers = $('.streams-container input[type="checkbox"]:checked')
            .map(function () {
                return this.value;
            })
            .get();

        if (selectedStreamers.length === 0) {
            // Show "Please select a streamer" if no streamers are selected
            const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text('Please select a streamer');
            $('#streamsContainer').append(selectStreamerTitle);

            // Show the circle image in the center of the website
            $('#circleImage').show();

            return;
        }

        // Remove the selectStreamerTitle if streamers are selected
        $('#streamerSelectionTitle').remove();
        $('#circleImage').hide();


        for (let i = 0; i < selectedStreamers.length; i += 2) {
            const videoRow = $('<div class="row"></div>');

            const streamer1 = selectedStreamers[i];
            const videoContainer1 = createVideoContainer(streamer1);
            const chatbox1 = createChatbox(streamer1);
            videoRow.append(videoContainer1);
            videoRow.append(chatbox1);

            if (i + 1 < selectedStreamers.length) {
                const streamer2 = selectedStreamers[i + 1];
                const videoContainer2 = createVideoContainer(streamer2);
                const chatbox2 = createChatbox(streamer2);
                videoRow.append(videoContainer2);
                videoRow.append(chatbox2);
            }

            $('#streamsContainer').append(videoRow);
        }
    });

    function createVideoContainer(streamer) {
        const videoContainer = $('<div class="col-md-6 mb-4"></div>');
        videoContainer.css({ width: '700px', height: '450px' });

        const videoElement = $('<video></video>').attr({
            type: 'video/mp4',
            controls: true,
        });
        videoElement.css({ width: '100%', height: '100%' });

        videoContainer.append(videoElement);
        return videoContainer;
    }

    function createChatbox(streamer) {
        const chatboxContainer = $('<div class="col-md-6 mb-4 chatbox-container"></div>');

        const chatboxTitle = $('<div class="chatbox-title"></div>').text(`Chatbox for ${streamer}`);
        const chatboxMessages = $('<ul id="messagesList"></ul>');
        const chatboxInput = $('<input type="text" class="w-100" id="messageInput" /></div>');

        const inputText = $('<input type="text" placeholder="Type your message..."</input>').addClass('chatbox-input-text');
        const sendButton = $('<div class="btn" id="sendButton">Send</div>');
        console.log('Button generated')

        chatboxInput.append(inputText);

        chatboxContainer.append(chatboxTitle);
        chatboxContainer.append(chatboxMessages);
        chatboxContainer.append(chatboxInput);
        chatboxContainer.append(sendButton);


        return chatboxContainer;
    }

    $('.streams-container input[type="checkbox"]').on('change', function () {
        if ($('.streams-container input[type="checkbox"]:checked').length > 4) {
            this.checked = false;
        }
    });

    "use strict";

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var container = document.getElementById('streams-container');
    var username = container.getAttribute('data-user');
    connection.on("ReceiveMessage", function (user, message) {
        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);
        li.textContent = `${user} : ${message}`;
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


});
