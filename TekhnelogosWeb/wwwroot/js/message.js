"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/messagehub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (word, vowelc) {
    console.log(word);
    var msg = word.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "The word '" + word + "' has " + vowelc + " vowel(s)";
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var _message = document.getElementById("messageInput").value;
    let wordMessage = {
        message: _message,
    };
    postWordMessage(wordMessage);

});

const postWordMessage = wordMessage => {
    let request = new XMLHttpRequest();
    request.onreadystatechange = () => {
        if (request.readyState === 4) {
            if (request.status !== 200) {
                console.error("Error on posting word message");
            }
        }
    };

    request.open("POST", "home/sendword");
    request.setRequestHeader("Content-Type", "application/json");
    request.send(JSON.stringify(wordMessage));
};