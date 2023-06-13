"use strict";

$(document).ready(function () {

  function initializeWatcherCount(count) {
    watcherCount = count;
    updateWatcherCountUI();
  }

  connection.on("InitializeWatcherCount", (count) => {
    initializeWatcherCount(count);
  });

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/streamHub") // Adjust the URL to match your server endpoint
    .build();

  let mediaSource;
  let sourceBuffer;
  let receivedChunks = [];
  let watcherCount = 0;

  connection.on("ReceiveChunk", (chunk) => {
    const videoElement = $(`#${chunk.streamer}`).find("video")[0];

    console.log("New data available: " + chunk);

    if (!mediaSource) {
      mediaSource = new MediaSource();
      videoElement.src = URL.createObjectURL(mediaSource);
      mediaSource.addEventListener("sourceopen", handleSourceOpen);
    }

    receivedChunks.push(chunk);

    if (sourceBuffer && !sourceBuffer.updating) {
      appendNextChunk();
    }
  });

  connection
    .start()
    .then(() => {
      console.log("Connection established.");
    })
    .catch((error) => {
      console.error("Error starting the signaling connection:", error);
    });

  function handleSourceOpen() {
    sourceBuffer = mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
    appendNextChunk();
  }

  function appendNextChunk() {
    if (sourceBuffer && !sourceBuffer.updating && receivedChunks.length > 0) {
      const chunk = receivedChunks.shift();
      const uint8Array = base64ToBytes(chunk.data);
      const chunkData = new Uint8Array(uint8Array.buffer);
      sourceBuffer.appendBuffer(chunkData);
    }
  }

  function updateWatcherCountUI() {
    const watcherCountElement = document.getElementById("watcherCount");
    if (watcherCountElement) {
      watcherCountElement.textContent = (watcherCount - 1).toString();
    }
  }

  function base64ToBytes(base64) {
    const base64abc =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    const padding = "=";

    let result = [];
    let bytes = (base64.length / 4) * 3;
    if (base64[base64.length - 1] === padding) {
      bytes--;
      if (base64[base64.length - 2] === padding) {
        bytes--;
      }
    }

    for (let i = 0, j = 0; i < base64.length; i += 4, j += 3) {
      const char1 = base64abc.indexOf(base64[i]);
      const char2 = base64abc.indexOf(base64[i + 1]);
      const char3 = base64abc.indexOf(base64[i + 2]);
      const char4 = base64abc.indexOf(base64[i + 3]);

      const byte1 = (char1 << 2) | (char2 >> 4);
      const byte2 = ((char2 & 15) << 4) | (char3 >> 2);
      const byte3 = ((char3 & 3) << 6) | char4;

      result[j] = byte1;
      if (char3 !== 64) {
        result[j + 1] = byte2;
      }
      if (char4 !== 64) {
        result[j + 2] = byte3;
      }
    }

    return new Uint8Array(result);
  }

  // Show the circle image in the center of the website initially
  $("#circleImage").show();

  $("#viewStreamsBtn").click(function () {
    // Clear the streamsContainer before adding new streamers
    $("#streamsContainer").empty();

    const selectedStreamers = $(
      '.streams-container input[type="checkbox"]:checked'
    )
      .map(function () {
        return this.value;
      })
      .get();

    if (selectedStreamers.length === 0) {
      // Show "Please select a streamer" if no streamers are selected
      const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text(
        "Please select a streamer"
      );
      $("#streamsContainer").append(selectStreamerTitle);

      // Show the circle image in the center of the website
      $("#circleImage").show();

      return;
    }

    // Remove the selectStreamerTitle if streamers are selected
    $("#streamerSelectionTitle").remove();
    $("#circleImage").hide();

    for (let i = 0; i < selectedStreamers.length; i += 2) {
      const streamer1 = selectedStreamers[i];
      const videoContainer1 = createVideoContainer(streamer1);
      const chatbox1 = createChatbox(streamer1);

      const videoRow = $('<div class="row"></div>');
      videoRow.append(videoContainer1);
      videoRow.append(chatbox1);

      if (i + 1 < selectedStreamers.length) {
        const streamer2 = selectedStreamers[i + 1];
        const videoContainer2 = createVideoContainer(streamer2);
        const chatbox2 = createChatbox(streamer2);
        videoRow.append(videoContainer2);
        videoRow.append(chatbox2);
      }

      $("#streamsContainer").append(videoRow);
    }
  });

  function createVideoContainer(streamer) {
    const videoContainer = $('<div class="col-md-6 mb-4"></div>');
    videoContainer.css({ width: "700px", height: "450px" });

    const videoElement = $("<video></video>").attr({
      type: "video/mp4",
      controls: true,
      id: streamer,
    });
    videoElement.css({ width: "100%", height: "100%" });

    videoContainer.append(videoElement);
    return videoContainer;
  }

  function createChatbox(streamer) {
    const chatboxContainer = $(
      '<div class="col-md-6 mb-4 chatbox-container"></div>'
    );

    const chatboxTitle = $('<div class="chatbox-title"></div>').text(
      `Chatbox for ${streamer}`
    );
    const chatboxMessages = $('<div class="chatbox-messages"></div>');
    const chatboxInput = $('<div class="chatbox-input"></div>');

    const inputText = $(
      '<input type="text" placeholder="Type your message...">'
    ).addClass("chatbox-input-text");
    const sendButton = $("<button>Send</button>").addClass(
      "chatbox-input-button"
    );

    chatboxInput.append(inputText);
    chatboxInput.append(sendButton);

    chatboxContainer.append(chatboxTitle);
    chatboxContainer.append(chatboxMessages);
    chatboxContainer.append(chatboxInput);

    return chatboxContainer;
  }

  $('.streams-container input[type="checkbox"]').on("change", function () {
    if ($('.streams-container input[type="checkbox"]:checked').length > 4) {
      this.checked = false;
    }
  });
});
("use strict");

$(document).ready(function () {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/streamHub") // Adjust the URL to match your server endpoint
    .build();

  let mediaSource;
  let sourceBuffer;
  let receivedChunks = [];
  let watcherCount = 0;

  connection.on("ReceiveChunk", (chunk) => {
    const videoElement = $(`#${chunk.streamer}`).find("video")[0];

    console.log("New data available: " + chunk);

    if (!mediaSource) {
      mediaSource = new MediaSource();
      videoElement.src = URL.createObjectURL(mediaSource);
      mediaSource.addEventListener("sourceopen", handleSourceOpen);
    }

    receivedChunks.push(chunk);

    if (sourceBuffer && !sourceBuffer.updating) {
      appendNextChunk();
    }
  });

  connection.on("UpdateWatcherCount", (count) => {
    watcherCount = count;
    updateWatcherCountUI();
  });

  connection
    .start()
    .then(() => {
      console.log("Connection established.");
    })
    .catch((error) => {
      console.error("Error starting the signaling connection:", error);
    });

  function handleSourceOpen() {
    sourceBuffer = mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
    appendNextChunk();
  }

  function appendNextChunk() {
    if (sourceBuffer && !sourceBuffer.updating && receivedChunks.length > 0) {
      const chunk = receivedChunks.shift();
      const uint8Array = base64ToBytes(chunk.data);
      const chunkData = new Uint8Array(uint8Array.buffer);
      sourceBuffer.appendBuffer(chunkData);
    }
  }

  function updateWatcherCountUI() {
    const watcherCountElement = $("#watcherCount");
    watcherCountElement.text((watcherCount - 1).toString());
  }

  function base64ToBytes(base64) {
    const base64abc =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    const padding = "=";

    let result = [];
    let bytes = (base64.length / 4) * 3;
    if (base64[base64.length - 1] === padding) {
      bytes--;
      if (base64[base64.length - 2] === padding) {
        bytes--;
      }
    }

    for (let i = 0, j = 0; i < base64.length; i += 4, j += 3) {
      const char1 = base64abc.indexOf(base64[i]);
      const char2 = base64abc.indexOf(base64[i + 1]);
      const char3 = base64abc.indexOf(base64[i + 2]);
      const char4 = base64abc.indexOf(base64[i + 3]);

      const byte1 = (char1 << 2) | (char2 >> 4);
      const byte2 = ((char2 & 15) << 4) | (char3 >> 2);
      const byte3 = ((char3 & 3) << 6) | char4;

      result[j] = byte1;
      if (char3 !== 64) {
        result[j + 1] = byte2;
      }
      if (char4 !== 64) {
        result[j + 2] = byte3;
      }
    }

    return new Uint8Array(result);
  }

  // Add an h1 element to show "Please select a streamer" initially
  const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text(
    "Please select a streamer"
  );
  $("#streamsContainer").append(selectStreamerTitle);

  // Show the circle image in the center of the website initially
  $("#circleImage").show();

  $("#viewStreamsBtn").click(function () {
    // Clear the streamsContainer before adding new streamers
    $("#streamsContainer").empty();

    const selectedStreamers = $(
      '.streams-container input[type="checkbox"]:checked'
    )
      .map(function () {
        return this.value;
      })
      .get();

    if (selectedStreamers.length === 0) {
      // Show "Please select a streamer" if no streamers are selected
      const selectStreamerTitle = $('<h1 id="selectStreamText"></h1>').text(
        "Please select a streamer"
      );
      $("#streamsContainer").append(selectStreamerTitle);

      // Show the circle image in the center of the website
      $("#circleImage").show();

      return;
    }

    // Remove the selectStreamerTitle if streamers are selected
    $("#streamerSelectionTitle").remove();
    $("#circleImage").hide();

    for (let i = 0; i < selectedStreamers.length; i += 2) {
      const streamer1 = selectedStreamers[i];
      const videoContainer1 = createVideoContainer(streamer1);
      const chatbox1 = createChatbox(streamer1);

      const videoRow = $('<div class="row"></div>');
      videoRow.append(videoContainer1);
      videoRow.append(chatbox1);

      if (i + 1 < selectedStreamers.length) {
        const streamer2 = selectedStreamers[i + 1];
        const videoContainer2 = createVideoContainer(streamer2);
        const chatbox2 = createChatbox(streamer2);
        videoRow.append(videoContainer2);
        videoRow.append(chatbox2);
      }

      $("#streamsContainer").append(videoRow);
    }
  });

  function createVideoContainer(streamer) {
    const videoContainer = $('<div class="col-md-6 mb-4"></div>');
    videoContainer.css({ width: "700px", height: "450px" });

    const videoElement = $("<video></video>").attr({
      type: "video/mp4",
      controls: true,
      id: streamer,
    });
    videoElement.css({ width: "100%", height: "100%" });

    videoContainer.append(videoElement);
    return videoContainer;
  }

  function createChatbox(streamer) {
    const chatboxContainer = $(
      '<div class="col-md-6 mb-4 chatbox-container"></div>'
    );

    const chatboxTitle = $('<div class="chatbox-title"></div>').text(
      `Chatbox for ${streamer}`
    );
    const viewerCount = $('<div class="viewer-count"></div>').text(`Viewers: `);
    const watcherCountElement = $('<span id="watcherCount"></span>');
    viewerCount.append(watcherCountElement);
    const chatboxMessages = $('<div class="chatbox-messages"></div>');
    const chatboxInput = $('<div class="chatbox-input"></div>');

    const inputText = $(
      '<input type="text" placeholder="Type your message...">'
    ).addClass("chatbox-input-text");
    const sendButton = $("<button>Send</button>").addClass(
      "chatbox-input-button"
    );

    chatboxInput.append(inputText);
    chatboxInput.append(sendButton);

    chatboxContainer.append(chatboxTitle);
    chatboxContainer.append(viewerCount);
    chatboxContainer.append(chatboxMessages);
    chatboxContainer.append(chatboxInput);

    return chatboxContainer;
  }

  $('.streams-container input[type="checkbox"]').on("change", function () {
    if ($('.streams-container input[type="checkbox"]:checked').length > 4) {
      this.checked = false;
    }
  });
});
