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
                console.log("Search Results:", retObj);
            },
            error: (xhr, status, error) => {
                console.error("Error fetching places:", error);
            }
        });
    });
});
