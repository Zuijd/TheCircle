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
        const chatboxContainer = $(`<div class="col-md-6 mb-4 chatbox-container"></div>`);

        var streamerId = streamer.split(" ").pop();

        const chatboxTitle = $('<div class="chatbox-title"></div>').text(`Chatbox for ${streamer}`);
        const chatboxMessages = $(`<ul class="messagesList" id="messagesList${streamerId}"></ul>`);
        const chatboxInput = $(`<input type="text" class="w-100" id="messageInput${streamerId}" /></div>`);

        const inputText = $('<input type="text" placeholder="Type your message..."</input>').addClass('chatbox-input-text');
        const sendButton = $(`<div class="btn" id="sendButton${streamerId}">Send</div>`);

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


});
