document.addEventListener('DOMContentLoaded', () => {
    const startButton = document.getElementById('startButton');
    const stopButton = document.getElementById('stopButton');
    const videoElement = document.getElementById('video');
  
    let mediaRecorder;
    let recordedChunks = [];
  
    startButton.addEventListener('click', () => {
      startStreaming();
    });
  
    stopButton.addEventListener('click', () => {
      stopStreaming();
    });
  
    const signalingConnection = new signalR.HubConnectionBuilder()
      .withUrl('/streamHub') // Adjust the URL to match your server endpoint
      .build();
  
    signalingConnection.on('OfferReceived', handleOffer);
    signalingConnection.on('IceCandidateReceived', handleIceCandidate);
  
    signalingConnection.start()
      .then(() => {
        // Connection is established, ready to send/receive signaling messages
      })
      .catch(error => {
        console.error('Error starting the signaling connection:', error);
      });
  
    function startStreaming() {
      navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
          videoElement.srcObject = stream;
  
          mediaRecorder = new MediaRecorder(stream);
  
          mediaRecorder.addEventListener('dataavailable', event => {
            recordedChunks.push(event.data);
          });
  
          mediaRecorder.addEventListener('stop', () => {
            const recordedBlob = new Blob(recordedChunks, { type: 'video/webm' });
            const url = URL.createObjectURL(recordedBlob);
  
            const a = document.createElement('a');
            a.href = url;
            a.download = 'stream.webm';
            document.body.appendChild(a);
            a.click();
  
            recordedChunks = [];
            URL.revokeObjectURL(url);
          });
  
          mediaRecorder.start();
          console.log('Recording started.');
        })
        .catch(error => {
          console.error('Error accessing media devices:', error);
        });
    }
  
    function stopStreaming() {
        if (mediaRecorder && mediaRecorder.state !== 'inactive') {
          mediaRecorder.stop();
          console.log('Recording stopped.');
      
          const tracks = videoElement.srcObject.getTracks();
          tracks.forEach(track => track.stop());
          videoElement.srcObject = null;
        }
      }
      
  
    function handleOffer(offer) {
      const peerConnection = new RTCPeerConnection();
  
      navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
          stream.getTracks().forEach(track => {
            peerConnection.addTrack(track, stream);
          });
  
          peerConnection.addEventListener('icecandidate', event => {
            if (event.candidate) {
              signalingConnection.invoke('SendIceCandidate', event.candidate)
                .catch(error => {
                  console.error('Error sending ICE candidate:', error);
                });
            }
          });
  
          peerConnection.createOffer()
            .then(offer => peerConnection.setLocalDescription(offer))
            .then(() => {
              signalingConnection.invoke('SendOffer', peerConnection.localDescription)
                .catch(error => {
                  console.error('Error sending offer:', error);
                });
            })
            .catch(error => {
              console.error('Error creating offer:', error);
            });
        })
        .catch(error => {
          console.error('Error accessing media devices:', error);
        });
    }
  
    function handleIceCandidate(candidate) {
      peerConnection.addIceCandidate(new RTCIceCandidate(candidate))
        .catch(error => {
          console.error('Error adding ICE candidate:', error);
        });
    }
  });
  