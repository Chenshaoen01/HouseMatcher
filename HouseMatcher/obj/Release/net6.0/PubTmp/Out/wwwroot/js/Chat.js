"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const isMessagePage = window.location.pathname.startsWith('/Message')

connection.on("ReceiveMessage", function (message) {
    if (isMessagePage) {
        const submitReciever = message.senderId === myUserId ? message.receiverId : message.senderId
        message.SubmitReceiver = submitReciever
        message.messageCardKey = message.houseDataId + submitReciever

        updateMessageData(message)

        const targetHouseCardData = messageData[message.messageCardKey]
        updateMessageCard(targetHouseCardData)

        const submitButton = document.getElementById('message-submit')
        const targetMessageCardId = submitButton.dataset.messageCardKey;
        if (targetMessageCardId === message.messageCardKey) {
            addNewMessage(message)
        }
    } else {
        $('#detail-page-message-modal').modal('hide')
        showHint("訊息發送成功")
    }
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

//送出訊息
function messageSubmit() {
    const $submitButton = $('#message-submit')
    const $messageInput = $('#message-input')
    
    const message = $messageInput.val()
    if (message !== null && message !== "") {
        const targetReceiverId = $submitButton.data('receiver-id').toString()
        const targetReceiverName = $submitButton.data('receiver-name')
        const targetSenderName = $submitButton.data('sender-name')
        const targetHosueId = $submitButton.data('house-id').toString()
        const targetHosueName = $submitButton.data('house-name')

        $messageInput.val('')
    
        connection.invoke("SendMessage", message, targetReceiverId, targetHosueId, targetHosueName, targetReceiverName, targetSenderName)
            .catch(function (err) {
                showHint('訊息傳送失敗')
            })
    }
}

// 寫入訊息
function addNewMessage(message) {
    const sentOrReceivedClassName = message.senderId === myUserId ? "message-sent" : "message-received"
    const senderNameShow = message.senderId === myUserId ? "我" : message.senderName
    $('.message-content-area').append(
        `<div class="${sentOrReceivedClassName}">
            <span>${senderNameShow}</span>
            <div class="message-sent-text">${message.messageDescription}</div>
            <div class="message-time-text">${dateTimeTransform(message.createdTime)}</div>
        </div>`
    )

    const isMessageBottom = $('#scroll-down-button').hasClass('d-none')
    if (isMessageBottom) {
        $('.message-content-area').scrollTop($('.message-content-area')[0].scrollHeight);
    }
}
