// watch.js
document.addEventListener('DOMContentLoaded', () => {
    const videoElement = document.getElementById('video');
  
    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
      .then(stream => {
        videoElement.srcObject = stream;
  
        const peerConnection = new RTCPeerConnection();
  
        stream.getTracks().forEach(track => {
          peerConnection.addTrack(track, stream);
        });
  
        peerConnection.addEventListener('track', event => {
          if (event.track.kind === 'video') {
            videoElement.srcObject = event.streams[0];
          }
        });
  
        const signalingConnection = new signalR.HubConnectionBuilder()
          .withUrl('/streamHub') // Adjust the URL to match your server endpoint
          .build();
  
        signalingConnection.on('OfferReceived', handleOffer);
        signalingConnection.on('AnswerReceived', handleAnswer);
        signalingConnection.on('IceCandidateReceived', handleIceCandidate);
  
        signalingConnection.start()
          .then(() => {
            // Connection is established, ready to send/receive signaling messages
          })
          .catch(error => {
            console.error('Error starting the signaling connection:', error);
          });
      })
      .catch(error => {
        console.error('Error accessing media devices:', error);
      });
  });
  
  function handleOffer(offer) {
    const peerConnection = new RTCPeerConnection();
  
    peerConnection.setRemoteDescription(new RTCSessionDescription(offer))
      .then(() => peerConnection.createAnswer())
      .then(answer => peerConnection.setLocalDescription(answer))
      .then(() => {
        const signalingConnection = new signalR.HubConnectionBuilder()
          .withUrl('/signalr') // Adjust the URL to match your server endpoint
          .build();
  
        signalingConnection.on('IceCandidateReceived', handleIceCandidate);
  
        signalingConnection.start()
          .then(() => {
            signalingConnection.invoke('SendAnswer', peerConnection.localDescription)
              .catch(error => {
                console.error('Error sending answer:', error);
              });
          })
          .catch(error => {
            console.error('Error starting the signaling connection:', error);
          });
      })
      .catch(error => {
        console.error('Error creating answer:', error);
      });
  }
  
  function handleAnswer(answer) {
    peerConnection.setRemoteDescription(new RTCSessionDescription(answer))
      .catch(error => {
        console.error('Error setting remote description:', error);
      });
  }
  
  function handleIceCandidate(candidate) {
    peerConnection.addIceCandidate(new RTCIceCandidate(candidate))
      .catch(error => {
        console.error('Error adding ICE candidate:', error);
      });
  }
  
  