// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const viewStreamsBtn = document.getElementById('viewStreamsBtn');
const streamsContainer = document.getElementById('streamsContainer');
const streamersSelect = document.getElementById('streamers');

viewStreamsBtn.addEventListener('click', function () {
    streamsContainer.innerHTML = '';

    const selectedStreamers = Array.from(streamersSelect.selectedOptions, option => option.value);

    if (selectedStreamers.length > 4) {
        alert('Please select a maximum of 4 streamers.');
        return;
    }

    for (let i = 0; i < selectedStreamers.length; i += 2) {
        const videoRow = document.createElement('div');
        videoRow.classList.add('row');

        const streamer1 = selectedStreamers[i];
        const videoContainer1 = createVideoContainer(streamer1);
        const chatbox1 = createChatbox(streamer1);
        videoRow.appendChild(videoContainer1);
        videoRow.appendChild(chatbox1);

        if (i + 1 < selectedStreamers.length) {
            const streamer2 = selectedStreamers[i + 1];
            const videoContainer2 = createVideoContainer(streamer2);
            const chatbox2 = createChatbox(streamer2);
            videoRow.appendChild(videoContainer2);
            videoRow.appendChild(chatbox2);
        }

        streamsContainer.appendChild(videoRow);
    }
});

function createVideoContainer(streamer) {
    const videoContainer = document.createElement('div');
    videoContainer.classList.add('col-md-6');
    videoContainer.classList.add('mb-4');
    videoContainer.style.width = '700px';
    videoContainer.style.height = '450px';

    const videoElement = document.createElement('video');
    videoElement.src = `${streamer}.mp4`;
    videoElement.type = 'video/mp4';
    videoElement.controls = true;
    videoElement.style.width = '100%';
    videoElement.style.height = '100%';

    videoContainer.appendChild(videoElement);
    return videoContainer;
}

function createChatbox(streamer) {
    const chatboxContainer = document.createElement('div');
    chatboxContainer.classList.add('col-md-6');
    chatboxContainer.classList.add('mb-4');
    chatboxContainer.classList.add('chatbox-container');

    const chatboxTitle = document.createElement('div');
    chatboxTitle.classList.add('chatbox-title');
    chatboxTitle.textContent = `Chatbox for ${streamer}`;

    const chatboxMessages = document.createElement('div');
    chatboxMessages.classList.add('chatbox-messages');

    const chatboxInput = document.createElement('div');
    chatboxInput.classList.add('chatbox-input');

    const inputText = document.createElement('input');
    inputText.type = 'text';
    inputText.placeholder = 'Type your message...';
    inputText.classList.add('chatbox-input-text');

    const sendButton = document.createElement('button');
    sendButton.textContent = 'Send';
    sendButton.classList.add('chatbox-input-button');

    chatboxInput.appendChild(inputText);
    chatboxInput.appendChild(sendButton);

    chatboxContainer.appendChild(chatboxTitle);
    chatboxContainer.appendChild(chatboxMessages);
    chatboxContainer.appendChild(chatboxInput);

    return chatboxContainer;
}

// Restrict the maximum number of selected streamers to 4
streamersSelect.addEventListener('change', function () {
    const selectedOptions = Array.from(this.selectedOptions);
    if (selectedOptions.length > 4) {
        const lastSelectedOption = selectedOptions[selectedOptions.length - 1];
        lastSelectedOption.selected = false;
    }
});

