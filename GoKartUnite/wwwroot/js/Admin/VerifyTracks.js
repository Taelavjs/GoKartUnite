
document.addEventListener("DOMContentLoaded", () => {
    const eleteSuccess = $('#successAlert');
    const DeleteError = $('#errorAlert');
    eleteSuccess.addClass('d-none');
    DeleteError.addClass('d-none');

    const VerifySuccess = $('#VerifySuccesAlert');
    const VerifyError = $('#VerifyFailAlert');
    VerifySuccess.addClass('d-none');
    VerifyError.addClass('d-none');
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
                VerifySuccess.removeClass('d-none');

                setTimeout(() => VerifySuccess.addClass('d-none'), 3000);

            },
            error: function (xhr, status, error) {
                VerifyError.removeClass('d-none');

                setTimeout(() => VerifyError.addClass('d-none'), 3000);
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
                eleteSuccess.removeClass('d-none');

                setTimeout(() => eleteSuccess.addClass('d-none'), 3000);

            },
            error: function (xhr, status) {
                DeleteError.removeClass('d-none');

                setTimeout(() => DeleteError.addClass('d-none'), 3000);

            }
        });

    });
})