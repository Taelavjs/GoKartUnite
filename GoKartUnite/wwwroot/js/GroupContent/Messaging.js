"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("ReceiveMessage", function (user, message) {

});

connection.start().then(function () {
    connection.invoke("AddToGroup", groupId).then(function () {
        MessageSubmitButton.addEventListener("click", () => {
            const messageToSend = MessageInput.value;
            connection.invoke("SendMessageToAllInGroup", groupId, messageToSend).catch(function (err) {

            });
        });

        connection.on("MessageIncoming", function (username, message) {
            var now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            appendMessage(username, `${hours}:${minutes}:${seconds}`, message);
            const chatContainer = document.getElementById('chatContainer');
            chatContainer.scrollTo({ top: chatContainer.scrollHeight, behavior: 'smooth' });
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });



}).catch(function (err) {
    return console.error(err.toString());
});


