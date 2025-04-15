$(document).ready(function () {
    const initialHTML = $('#hover-card-container').html();
    let $currentTrigger;
    let isInHoverCardContainer = false;

    const profileLoadingIndication = $('#profileLoading');
    console.log(initialHTML);
    function showProfileCard($trigger) {
        const offset = $trigger.offset();
        $('#hover-card-container').css({
            top: offset.top + $trigger.outerHeight() + 5,
            left: offset.left,
            display: 'block',
        });
        profileLoadingIndication.show();
        const username = $trigger.data('username');
        sleep(500).then(() => {
            $.ajax({
                url: `${window.location.origin}/KarterHome/ProfileCard?username=${username}`,
                type: 'GET',
                success: function (response) {
                    profileLoadingIndication.hide();
                    $('#profileLoaded').html(response);
                },
                error: function (xhr, status, error) {
                    alert("An error occurred: " + error);
                }
            });
        });

        $currentTrigger = $trigger;
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    $(document).on('mouseenter', '.username-hover', function () {
        $('#profileLoaded').html("");
        const $this = $(this);
        showProfileCard($this);
    });

    $(document).on('mouseleave', '#hover-card-container', function () {
        $('#hover-card-container').hide();
        $('#profileLoaded').html("");
    });
});
