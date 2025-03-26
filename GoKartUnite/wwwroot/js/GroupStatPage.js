(async function () {
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


const GroupMembersStatsRequest = (TrackTitle) => {
    const groupId = document.getElementById('acquisitions').dataset.groupid;
    console.log("Id : ", groupId, " Track : ", TrackTitle
    )
    $.ajax({
        url: `${window.location.origin}/group/GroupStats?GroupId=${groupId}&TrackTitle=${TrackTitle}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: (retObj) => {


            const bestLapTimes = retObj.map(row => row.bestLapTime);
            const minLapTime = Math.min(...bestLapTimes);
            const maxLapTime = Math.max(...bestLapTimes);
            const stepSize = Math.round((maxLapTime - minLapTime) / 10);

            if (Chart.getChart("acquisitions")) {
                Chart.getChart("acquisitions")?.destroy()
            }
            new Chart(
                document.getElementById('acquisitions'),
                {
                    type: 'scatter',
                    options: {
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
        }
    })
}