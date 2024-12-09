const port = window.location.port;

const socketUrl = `wss://${window.location.hostname}:${port}/ws`;
const socket = new WebSocket(socketUrl);

socket.onopen = function (event) {
    console.log('WebSocket connection established');
    socket.send("Success");
};

socket.onmessage = function (event) {
    console.log('Message from server:', event.data);

    $("#NumberFriends").text(event.data);
};

// Event handler for errors
socket.onerror = function (event) {
    console.error('WebSocket error:', event);
};

// Event handler for when the WebSocket connection is closed
socket.onclose = function (event) {
    console.log('WebSocket connection closed:', event);
};