"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/botHub").build();

connection.on("ReceiveMessage", function (message) {
    console.log("Message", message);
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messages").appendChild(li);
});

connection.start();