document.addEventListener('DOMContentLoaded', function () {
    const showPosts = document.getElementById("showPosts");
    const subGroups = document.getElementById("subgroup");
    const messageContainer = document.getElementById("groupMessages");

    function fetchKarterData(buttonId, endpoint) {
        const button = document.getElementById(buttonId);
        if (!button) return;

        button.addEventListener("click", function () {
            const karterId = this.getAttribute("data-karterId");
            $.ajax({
                url: `${window.location.origin}/Admin/${endpoint}?karterid=${karterId}`,
                type: 'GET',
                success: function (response) {
                    $("#subgroup").hide();

                    showPosts.innerHTML = response;
                    $("#showPosts").show();

                },
                error: function () {
                    alert("Error fetching data.");
                }
            });
        });
    }

    function createBtn(value, key, karterId) {

        const button = document.createElement("button");
        button.type = "button";
        button.id = "ShowUsersPosts";
        button.textContent = value;
        button.setAttribute("data-karterId", karterId);
        button.setAttribute("data-groupId", key);

        button.addEventListener("click", function () {
            $.ajax({
                url: `${window.location.origin}/Admin/GetKarterMessagesInGroup?karterId=${karterId}&groupId=${key}`,
                type: 'GET',
                success: function (response) {
                    messageContainer.innerHTML = response;
                },
                error: function () {
                    alert("Error fetching data.");
                }
            });
        });

        document.getElementById("buttonContainer").appendChild(button);
    }

    function fetchKarterGroupsList(buttonId, endpoint) {
        const button = document.getElementById(buttonId);
        if (!button) return;

        button.addEventListener("click", function () {
            const karterId = this.getAttribute("data-karterId");
            $.ajax({
                url: `${window.location.origin}/Admin/${endpoint}?karterid=${karterId}`,
                type: 'GET',
                success: function (response) {
                    $("#showPosts").hide();

                    document.getElementById("buttonContainer").innerHTML = "";
                    Object.entries(response.message).forEach(([key, value]) => {
                        createBtn(value, key, karterId);
                    });
                    $("#subgroup").show();

                },
                error: function () {
                    alert("Error fetching data.");
                }
            });
        });
    }

    function fetchKarterGroupMessages(buttonId, endpoint) {
        const button = document.getElementById(buttonId);
        if (!button) return;
        alert(true);
        button.addEventListener("click", function () {
            const karterId = this.getAttribute("data-karterId");
            $.ajax({
                url: `${window.location.origin}/Admin/${endpoint}?karterid=${karterId}`,
                type: 'GET',
                success: function (response) {
                    console.log(response)
                },
                error: function () {
                    alert("Error fetching data.");
                }
            });
        });
    }

    fetchKarterData("ShowUsersPosts", "GetKartersBlogPosts");
    fetchKarterData("CommentsShow", "GetKartersComments");
    fetchKarterGroupsList("GetListKartersGroups", "GetListKartersGroups");
});
