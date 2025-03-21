(async function () {
    const groupId = document.getElementById('acquisitions').dataset.groupid;
    console.log("Id : ", groupId)

    $.ajax({
        url: `${window.location.origin}/group/GroupStats?GroupId=${groupId}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: (retObj) => {
            console.log(retObj);

            const bestLapTimes = retObj.map(row => row.bestLapTime);
            const minLapTime = Math.min(...bestLapTimes);
            const maxLapTime = Math.max(...bestLapTimes);
            const stepSize = Math.round((maxLapTime - minLapTime) / 10);
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
                                offset: true, // This will space out the labels evenly

                            }
                        }
                    },
                    data: {
                        labels: retObj.map(row => row.karterName),
                        datasets: [
                            {
                                label: 'Acquisitions by year',
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
})();
