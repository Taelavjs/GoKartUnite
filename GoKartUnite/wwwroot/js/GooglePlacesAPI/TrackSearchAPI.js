let serverErrDiv = document.getElementById("ServerErr");
let inputErrorDiv = document.getElementById("TrackNotFound");
let SuccessDiv = document.getElementById("Success");
let Loading = document.getElementById("GroupStatGraphLoading");
Loading.style.visibility = "hidden";

function selectPlace(placeId) {
    Loading.style.visibility = "visible";

    SuccessDiv.hidden = true;
    inputErrorDiv.hidden = true;
    serverErrDiv.hidden = true;
    const listContainer = document.getElementById('placesList');

    listContainer.innerHTML = '';
    $.ajax({
        type: "POST",
        url: `${window.location.origin}/TrackHome/SelectSearchedResult?id=${placeId}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: () => {
            SuccessDiv.hidden = false;
            Loading.style.visibility = "hidden";


        },
        error: (xhr, status, error) => {
            console.error("AJAX Error:", status, error, xhr.responseText);
            serverErrDiv.hidden = false;
            Loading.style.visibility = "hidden";
        }
    });
};

document.addEventListener("DOMContentLoaded", () => {

    let inputElem = document.getElementById("googleSearchInput");
    let btnSubmit = document.getElementById("googleSearchSubmit");

    SuccessDiv.hidden = true;
    inputErrorDiv.hidden = true;
    serverErrDiv.hidden = true;

    btnSubmit.addEventListener("click", (event) => {
        event.preventDefault();
        const searchValue = inputElem.value.trim();

        if (!searchValue) {
            alert("Please enter a search query!");
            return;
        }

        Loading.style.visibility = "visible";

        $.ajax({
            type: "GET",
            url: `${window.location.origin}/TrackHome/GetPlaces?query=${encodeURIComponent(searchValue)}`,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (retObj) => {
                console.log("Search Results:", retObj);

                const template = document.getElementById('PlacesReturnedResult');
                const listContainer = document.getElementById('placesList');

                listContainer.innerHTML = '';
                Loading.style.visibility = "hidden";

                if (!retObj.places || retObj.places.length === 0) {
                    inputErrorDiv.hidden = false;
                } else {
                    retObj.places.forEach(place => {
                        let newItem = template.content.cloneNode(true);

                        newItem.querySelector('.place-title').textContent = place.displayName.text;
                        newItem.querySelector('.place-location').textContent = place.formattedAddress;

                        const selectButton = newItem.querySelector('.btn');
                        selectButton.addEventListener("click", function () {
                            selectPlace(place.id);
                        });
                        listContainer.appendChild(newItem);

                        SuccessDiv.hidden = true;
                        inputErrorDiv.hidden = true;
                        serverErrDiv.hidden = true;
                    });
                }
            },
            error: (xhr, status, error) => {
                Loading.style.visibility = "hidden";
                serverErrDiv.hidden = false;
            }
        });
    });
});
