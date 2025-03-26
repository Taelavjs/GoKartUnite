var loadingElement;
$(document).ready(function () {
    loadingElement = document.getElementById("GroupStatGraphLoading");
    loadingElement.style.visibility = "hidden";

    var selector = document.getElementById("GraphTitleSelector");

    GroupMembersStatsRequest(selector.value);


    selector.addEventListener("change", function () {
        loadingElement.style.visibility = "visible";

        GroupMembersStatsRequest(selector.value);
    });
});

function convertToMilliseconds(timeString) {
    const parts = timeString.split(':');
    const minutes = parseInt(parts[0], 10);
    const seconds = parseInt(parts[1], 10);
    const milliseconds = parseInt(parts[2], 10);
    return (minutes * 60 + seconds) * 1000 + milliseconds;
}

const GroupMembersStatsRequest = (TrackTitle) => {
    var dateTimeData = [];
    loadingElement.style.visibility = "visible";

    $.ajax({
        type: "GET",
        url: `${window.location.origin}/KarterHome/GetKartersStats?TrackTitle=${TrackTitle}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: async (retObj) => {
            console.log(retObj);
            if (Chart.getChart("acquisitions")) {
                Chart.getChart("acquisitions")?.destroy()
            }
            retObj.sort((a, b) => {
                const dateA = new Date(a.dateOnlyRecorded);
                const dateB = new Date(b.dateOnlyRecorded);
                return dateA - dateB;
            });
            const bestLapTimes = retObj.map(row => {
                return convertToMilliseconds(row.bestLapTime);
            });
            const minLapTime = Math.min(...bestLapTimes);
            const maxLapTime = Math.max(...bestLapTimes);
            const stepSize = Math.round((maxLapTime - minLapTime) / 10);
            const lastDate = retObj[0].dateOnlyRecorded;

            const TrackTitle = retObj.length > 0 ? retObj[0].trackTitle : "Unknown Track";
            new Chart(
                document.getElementById("acquisitions"),
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
                                from: minLapTime
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
                                min: lastDate
                            }
                        }
                    },
                    data: {
                        labels: retObj.map(row => row.dateOnlyRecorded),
                        datasets: [
                            {
                                label: `Best Lap Time At ${TrackTitle}`,
                                data: retObj.map(row => ({
                                    x: row.dateOnlyRecorded,
                                    y: convertToMilliseconds(row.bestLapTime)
                                }))
                            }
                        ]
                    }
                }
            );
            loadingElement.style.visibility = "hidden";

        }
    });
}
