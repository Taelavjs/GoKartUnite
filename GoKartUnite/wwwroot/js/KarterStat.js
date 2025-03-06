$(document).ready(function () {




    function convertToMilliseconds(timeString) {
        console.log(timeString);

        const parts = timeString.split(':');
        const minutes = parseInt(parts[0], 10);
        const seconds = parseInt(parts[1], 10);
        const milliseconds = parseInt(parts[2], 10);
        return (minutes * 60 + seconds) * 1000 + milliseconds;
    }

    var dateTimeData = [];

    let threeMonthsAgo = new Date();
    threeMonthsAgo.setMonth(new Date().getMonth() - 3);
    var myChart;
    $.ajax({
        type: "GET",
        url: `${window.location.origin}/KarterHome/GetKartersStats`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            let trackToFilterFor;
            for (let i = 0; i < response.length; i++) {
                const kartStat = response[i];

                const timeInMs = convertToMilliseconds(kartStat.bestLapTime);
                if (!trackToFilterFor) trackToFilterFor = kartStat.trackTitle;
                const date = new Date(kartStat.dateOnlyRecorded);
                dateTimeData.push({ date: date, time: timeInMs, timeString: kartStat.bestLapTime, trackTitle: kartStat.trackTitle });
            }
            dateTimeData.sort((a, b) => a.date - b.date);

            let filteredDateTimeData = dateTimeData.filter(item => item.trackTitle == trackToFilterFor);

            var dates = filteredDateTimeData.map(item => item.date);
            var timesInMilliseconds = filteredDateTimeData.map(item => item.time);
            var lapTimeString = filteredDateTimeData.map(item => item.timeString);
            const ctx = document.getElementById('myLineChart').getContext('2d');
            myChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: dates,
                    datasets: [{
                        label: trackToFilterFor,
                        data: timesInMilliseconds,
                        borderColor: 'rgb(75, 192, 192)',
                        fill: false
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        x: {
                            type: 'time',
                            time: {
                                unit: 'month',
                                tooltipFormat: 'P'
                            },
                            min: threeMonthsAgo,
                            max: new Date(),
                            title: {
                                display: true,
                                text: 'Date'
                            }
                        },
                        y: {
                            ticks: {
                                beginAtZero: false,
                                suggestedMin: Math.min(...timesInMilliseconds) - 1000,
                                callback: function (value) {
                                    const minutes = Math.floor(value / (60 * 1000));
                                    const seconds = Math.floor((value % (60 * 1000)) / 1000);
                                    const milliseconds = value % 1000;
                                    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}:${milliseconds < 100 ? '0' : ''}${milliseconds}`;
                                }
                            },
                            title: {
                                display: true,
                                text: 'Lap Time'
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    const timeInMilliseconds = tooltipItem.raw;

                                    const minutes = Math.floor(timeInMilliseconds / (60 * 1000));
                                    const seconds = Math.floor((timeInMilliseconds % (60 * 1000)) / 1000);
                                    const milliseconds = timeInMilliseconds % 1000;

                                    const formattedTime = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}:${milliseconds < 100 ? '0' : ''}${milliseconds}`;

                                    return `Lap Time: ${formattedTime}`;
                                }
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }



    });
    $('#GraphTitleSelector').change(function () {
        const newTitle = $(this).val();
        let filteredData;

        filteredData = dateTimeData.filter(item => item.trackTitle == newTitle);
        console.log(filteredData);
        console.log(dateTimeData);
        const filteredDates = filteredData.map(item => item.date);
        const filteredTimesInMilliseconds = filteredData.map(item => item.time);

        myChart.data.labels = filteredDates;
        myChart.data.datasets[0].data = filteredTimesInMilliseconds;
        myChart.data.datasets[0].label = newTitle;
        myChart.update();
    });
});