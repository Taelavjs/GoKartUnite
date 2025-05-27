
document.addEventListener("DOMContentLoaded", () => {
    $(document).on('click', '.VerifyTrackBtn', function () {
        const $this = $(this);
        const trackId = $this.data('trackid');

        $.ajax({
            url: `${window.location.origin}/Admin/VerifyTracks?id=${trackId}`,
            type: 'POST',
            success: function (response) {
                const row = document.getElementById(trackId);
                if (row) {
                    row.remove();
                }
                ShowSuccessMessage("Verified Track Successfully");

            },
            error: function (xhr, status, error) {
                ShowErrorMessage("Unable To Verified Track");

            }
        });

    });

    $(document).on('click', '.RemoveVerifyTrack', function () {
        const $this = $(this);
        const trackId = $this.data('trackid');

        $.ajax({
            url: `${window.location.origin}/Admin/DeVerifyTracks?id=${trackId}`,
            type: 'POST',
            success: function (response) {

                const row = document.getElementById(trackId);
                if (row) {
                    row.remove();
                }
                ShowSuccessMessage("Removed Track Verification Request");


            },
            error: function (xhr, status) {
                ShowErrorMessage("Unable To Delete Track Verification Request");
            }
        });

    });
})