const loadingElement = document.getElementById("GroupStatGraphLoading");
const graphElement = document.getElementById('acquisitions');
const MessageInput = document.getElementById('MessageInput');
const groupId = document.getElementById('acquisitions').dataset.groupid;

const MessageSubmitButton = document.getElementById('MessageInputButton');
const MessageError = document.getElementById('MessageErrorDiv');
(async function () {
    document.getElementById("GroupStatGraphLoading").style.visibility = "hidden";

    var selectItem = document.getElementById("groupStatTrackSelect");
    selectItem.addEventListener("change", function () {
        console.log(selectItem);
        GroupMembersStatsRequest(selectItem.value);
    });

    MessageSubmitButton.addEventListener("click", () => {
        console.log("message sent");
        CreateMessage();
    })

    document.addEventListener("DOMContentLoaded", function () {
        console.log(selectItem);
        GroupMembersStatsRequest(selectItem.value);
    });

})();


const GroupMembersStatsRequest = async (TrackTitle) => {
    console.log("Id : ", groupId, " Track : ", TrackTitle
    )

    loadingElement.style.visibility = "visible";
    const MIN_LOADING_TIME = 1000;

    $.ajax({
        url: `${window.location.origin}/group/GroupStats?GroupId=${groupId}&TrackTitle=${TrackTitle}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: (retObj) => {
            if (Chart.getChart("acquisitions")) {
                Chart.getChart("acquisitions")?.destroy()
            }
            setTimeout(() => {
                loadingElement.style.visibility = "hidden";
                const bestLapTimes = retObj.map(row => row.bestLapTime);
                const minLapTime = Math.min(...bestLapTimes);
                const maxLapTime = Math.max(...bestLapTimes);
                const stepSize = Math.round((maxLapTime - minLapTime) / 10);
                new Chart(
                    graphElement,
                    {
                        type: 'scatter',
                        options: {
                            animation: {
                                x: {
                                    duration: 1500,
                                    easing: 'easeOutQuad',
                                    from: 0
                                },
                                y: {
                                    duration: 1500,
                                    easing: 'easeOutQuad',
                                    from: 0
                                }
                            },
                            scales: {
                                y: {
                                    grid: {
                                        color: "rgba(255, 255, 255, 0.2)" // Y-axis grid lines
                                        text
                                    },
                                    type: 'linear',
                                    ticks: {
                                        color: "white",
                                        min: minLapTime,
                                        max: maxLapTime,
                                        stepSize: stepSize,
                                        callback: function (value) {
                                            const minutes = Math.floor(value / 60000);
                                            const seconds = ((value % 60000) / 1000).toFixed(0);
                                            return minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
                                        }
                                    }
                                },
                                x: {
                                    grid: {
                                        color: "rgba(255, 255, 255, 0.2)" // Y-axis grid lines
                                    },
                                    type: "category",
                                    offset: true,
                                    ticks: {
                                        color: "white",
                                    },
                                }
                            },
                            plugins: {
                                legend: {
                                    labels: {
                                        color: "white"
                                    }
                                }
                            }
                        },
                        data: {
                            labels: retObj.map(row => row.karterName),
                            datasets: [
                                {
                                    label: `Best Lap Time At ${TrackTitle}`,
                                    data: retObj.map(row => ({
                                        x: row.karterName,
                                        y: row.bestLapTime
                                    }))
                                }
                            ]
                        }
                    }
                );
            }, MIN_LOADING_TIME);
        }
    })
}


const CreateMessage = async () => {
    const messageToSend = MessageInput.value;
    console.log(messageToSend);

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    $.ajax({
        url: `${window.location.origin}/group/home?GroupId=${groupId}`,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(messageToSend),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('RequestVerificationToken', token);
        },
        success: function (response) {
            console.log("Success:", response);
            MessageInput.value = '';
            MessageError.hidden = true;
            var now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            appendMessage(response.userName, `${hours}:${minutes}:${seconds}`, messageToSend);
            const chatContainer = document.getElementById('chatContainer');
            chatContainer.scrollTo({ top: chatContainer.scrollHeight, behavior: 'smooth' });
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
            MessageError.innerText = "Error With message";  // Safer, for plain text
            MessageError.hidden = false;
        }
    });
}

function appendMessage(name, timeSent, messageContent) {
    const template = document.getElementById("messageTemplate");

    const messageElement = template.content.cloneNode(true);

    messageElement.querySelector(".message-sender").textContent = name;
    messageElement.querySelector(".message-time").textContent = timeSent;
    messageElement.querySelector(".message-content").textContent = messageContent;

    document.getElementById("chatContainer").appendChild(messageElement);
}

