//var container = $('#stream-container'); // Use jQuery selector to get the element with id 'stream-container'

//$(document).ready(function () {
//    // Add an h1 element to show "Please select a streamer" initially
//    const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text('Please select a streamer');
//    $('#streamsContainer').append(selectStreamerTitle);

//    // Show the circle image in the center of the website initially
//    $('#circleImage').show();

//    // Set attribute to undefined
//    container.attr('data-streamer', 'no streamer selected');

//    $('#viewStreamsBtn').click(function () {
//        // Clear the streamsContainer before adding new streamers
//        $('#streamsContainer').empty();

//        const selectedStreamer = $('.streams-container input[type="radio"]:checked')
//            .map(function () {
//                return this.value;
//            })
//            .get();

//        if (selectedStreamer.length === 0) {
//            // Show "Please select a streamer" if no streamers are selected
//            const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text('Please select a streamer');
//            $('#streamsContainer').append(selectStreamerTitle);

//            // Show the circle image in the center of the website
//            $('#circleImage').show();

//            return;
//        }

//        // Remove the selectStreamerTitle if streamers are selected
//        $('#streamerSelectionTitle').remove();
//        $('#circleImage').hide();


//        const videoRow = $('<div class="row"></div>');
//        if (selectedStreamer.length == 1) {
//            const streamer = selectedStreamer;
//            const videoContainer = createVideoContainer(streamer);
//            const chatbox = createChatbox(streamer);
//            videoRow.append(videoContainer);
//            videoRow.append(chatbox);
//        }

//        $('#streamsContainer').append(videoRow);

//    });

//    function createVideoContainer(streamer) {
//        const videoContainer = $('<div class="col-md-6 mb-4"></div>');
//        videoContainer.css({ width: '700px', height: '450px' });

//        const videoElement = $('<video></video>').attr({
//            type: 'video/mp4',
//            controls: true,
//        });
//        videoElement.css({ width: '100%', height: '100%' });

//        videoContainer.append(videoElement);
//        return videoContainer;
//    }

//    function createChatbox(streamer) {
//        const chatboxContainer = $('<div class="col-md-6 mb-4 chatbox-container"></div>');

//        // var streamerId = streamer.split(" ").pop();
//        console.log(streamer)
//        container.attr('data-streamer', streamer);
//        console.log(container.attr('data-streamer'));

//        const chatboxTitle = $('<div class="chatbox-title"></div>').text(`Chatbox for ${streamer}`);
//        const chatboxMessages = $(`<ul class="messagesList" id="messagesList${streamer}"></ul>`);
//        const chatboxInput = $(`<input type="text" class="w-100" id="messageInput${streamer}" /></div>`);

//        const inputText = $('<input type="text" placeholder="Type your message..."</input>').addClass('chatbox-input-text');
//        const sendButton = $(`<div class="btn" id="sendButton${streamer}">Send</div>`);

//        chatboxInput.append(inputText);

//        chatboxContainer.append(chatboxTitle);
//        chatboxContainer.append(chatboxMessages);
//        chatboxContainer.append(chatboxInput);
//        chatboxContainer.append(sendButton);

//        return chatboxContainer;
//    }

//    $('.streams-container input[type="checkbox"]').on('change', function () {
//        if ($('.streams-container input[type="checkbox"]:checked').length > 4) {
//            this.checked = false;
//        }
//    });
//});


// Add event listener to update the value of viewStreamsBtn
var radioButtons = document.querySelectorAll('input[name="streamerButton"]');
var viewStreamsBtn = document.getElementById('viewStreamsBtn');


viewStreamsBtn.addEventListener('click', function () {
    var selectedValue = document.querySelector('input[name="streamerButton"]:checked').value;
    var url = "/Stream/Watch/" + encodeURIComponent(selectedValue);
    window.location.href = url;
});
