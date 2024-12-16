"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub?username=tvjs").build();

connection.on("ReceiveMessage", function (user, message) {
    $("#NumberFriends").text(user);
});

connection.start().then(function (ret) {

    console.log(ret);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.onclose(err => {
    console.log("Connection closed:", err);
});

$("#Test").on("click", function () {
    var user = "Taeboo";
    var message = "hehe";
    connection.invoke("SendMessage", "Taeboo", "Taeboo").catch(function (err) {
        return console.error(err.toString());
        return console.error(err.toString());
    });
});