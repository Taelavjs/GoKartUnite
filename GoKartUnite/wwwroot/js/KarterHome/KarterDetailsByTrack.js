let page = 1;

let reverseName = false;
let reverseYoe = false;
document.addEventListener("DOMContentLoaded", () => {
    const nameSortBtn = document.getElementById("NameSortButton");
    let sortValue = nameSortBtn.getAttribute("data-sortValue");
    const urlParams = new URLSearchParams(window.location.search);
    const track = urlParams.get("track");
    const urlToSend = `/KarterHome/GetKartersPartialViews?track=${encodeURIComponent(track)}&page=${page}&sortby=${encodeURIComponent(sortValue)}`;
    ajaxRequestSortedKarterList(urlToSend);

    if (nameSortBtn) {
        nameSortBtn.addEventListener("click", () => {
            sortValue = nameSortBtn.getAttribute("data-sortValue");
            if (reverseName) sortValue = "Reverse" + sortValue;
            const urlToSend = `/KarterHome/GetKartersPartialViews?track=${encodeURIComponent(track)}&page=${page}&sortby=${encodeURIComponent(sortValue)}`;
            ajaxRequestSortedKarterList(urlToSend);
            reverseName = !reverseName;
        });
    }

    const yoeSortButton = document.getElementById("YearsSortButton");
    let sortValueYoe = yoeSortButton.getAttribute("data-sortValue");

    if (yoeSortButton) {
        yoeSortButton.addEventListener("click", () => {
            alert(sortValueYoe);
            sortValueYoe = yoeSortButton.getAttribute("data-sortValue");
            if (reverseYoe) sortValueYoe = "Reverse" + sortValueYoe;
            const urlToSendYoe = `/KarterHome/GetKartersPartialViews?track=${encodeURIComponent(track)}&sortby=${encodeURIComponent(sortValueYoe)}`;
            ajaxRequestSortedKarterList(urlToSendYoe);
            reverseYoe = !reverseYoe;
        });
    }
});

const ajaxRequestSortedKarterList = (urlToSend) => {
    $.ajax({
        url: urlToSend,
        type: 'POST',
        success: function (response) {
            console.log(response);
            $('#ListKarterPartialViews').html(response);
        },
        error: function (xhr, status, error) {
            alert("Error!");
        }
    });
}