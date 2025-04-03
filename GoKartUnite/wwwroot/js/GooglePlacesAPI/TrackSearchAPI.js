
function selectPlace(placeId) {
    $.ajax({
        type: "POST",
        url: `${window.location.origin}/TrackHome/SelectSearchedResult?id=${placeId}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: (retObj) => {
            alert("Success");
        }
    });
};

document.addEventListener("DOMContentLoaded", () => {
    let inputElem = document.getElementById("googleSearchInput");
    let btnSubmit = document.getElementById("googleSearchSubmit");

    btnSubmit.addEventListener("click", (event) => {
        event.preventDefault();
        const searchValue = inputElem.value.trim();

        if (!searchValue) {
            alert("Please enter a search query!");
            return;
        }

        $.ajax({
            type: "GET",
            url: `${window.location.origin}/TrackHome/GetPlaces?query=${encodeURIComponent(searchValue)}`,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (retObj) => {
                console.log("running");

                console.log("Search Results:", retObj);
                console.log("running");

                const template = document.getElementById('PlacesReturnedResult');
                const listContainer = document.getElementById('placesList');  // Container where we will append list items

                listContainer.innerHTML = '';
                console.log("running");
                retObj.places.forEach(place => {
                    let newItem = template.content.cloneNode(true);

                    newItem.querySelector('.place-title').textContent = place.displayName.text; 
                    newItem.querySelector('.place-location').textContent = place.formattedAddress;

                    const selectButton = newItem.querySelector('.btn');
                    selectButton.addEventListener("click", function () {
                        selectPlace(place.id);
                    });
                    listContainer.appendChild(newItem);

                });
            },
            error: (xhr, status, error) => {
                console.error("Error fetching places:", error);
            }
        });
    });
});

