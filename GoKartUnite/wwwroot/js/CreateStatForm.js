$(document).ready(function () {
    $("#statCreateForm").on("submit", function (e) {
        e.preventDefault(); // Prevent default form submission

        // Serialize form data
        var formData = $(this).serialize();

        // Send an AJAX request
        $.ajax({
            url: `${window.location.origin}/KarterHome/CreatTrackStat`,
            type: 'POST',
            data: formData,
            success: function (response) {
                // Handle successful response (optional)
                $("#statCreateForm")[0].reset();
                $("#successStatCreated").removeAttr("hidden");
            },
            error: function (xhr, status, error) {
                alert("An error occurred: " + error);
                $("#failedStatCreated").removeAttr("hidden");
            }
        });
    });
});