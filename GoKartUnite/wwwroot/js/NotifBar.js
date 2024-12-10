"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

console.log(connection);
connection.on("ReceiveMessage", function (user, message) {
    console.log("Dead");

});

connection.start().then(function () {

    console.log("here");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.onclose(err => {
    console.error("Connection closed:", err);
});

$("#Test").on("click", function () {
    var user = "Taeboo";
    var message = "hehe";
    connection.invoke("SendMessage", "Taeboo", "Taeboo").catch(function (err) {
        return console.error(err.toString());
        return console.error(err.toString());
    });
});