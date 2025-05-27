$(document).ready(function () {
    $('.followTrackLink').on('click', function (event) {
        event.preventDefault();
        let track = $(this).data('track');
        const prevText = $(this).text().trim();
        const $link = $(this);
        $.ajax({
            url: `${window.location.origin}/FollowTrack?track=${track}`,
            type: 'GET',
            success: function (response) {
                if (prevText === "Follow") {
                    $link.text("Unfollow");
                } else {
                    $link.text("Follow");
                }
            },
            error: function (xhr, status, error) {
                console.log("error servber");
            }
        });
    });
});
