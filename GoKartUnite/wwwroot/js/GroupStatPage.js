const loadingElement = document.getElementById("GroupStatGraphLoading");
const graphElement = document.getElementById('acquisitions');
(async function () {
    document.getElementById("GroupStatGraphLoading").style.visibility = "hidden";

    var selectItem = document.getElementById("groupStatTrackSelect");
    selectItem.addEventListener("change", function () {
        console.log(selectItem);
        GroupMembersStatsRequest(selectItem.value);
    });

    document.addEventListener("DOMContentLoaded", function () {
        console.log(selectItem);
        GroupMembersStatsRequest(selectItem.value);
    });
})();


const GroupMembersStatsRequest = async (TrackTitle) => {
    const groupId = document.getElementById('acquisitions').dataset.groupid;
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
                                    type: 'linear',
                                    ticks: {
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
                                    type: "category",
                                    offset: true,
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