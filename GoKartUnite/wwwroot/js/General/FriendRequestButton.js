
$(document).ready(function () {
    $(document).on('click', 'button[data-friendid]', function (e) {
        const action = $(this).val();
        const friendId = $(this).data('friendid');
        const $btn = $(this);

        console.log('Action:', action);
        console.log('FriendId:', friendId);
        $.ajax({
            url: `${window.location.origin}/KarterHome/HandleFriendRequest?friendId=${friendId}&friendAction=${action}`,
            type: 'POST',
            success: function (response) {
                console.log(response);
                const $newButton = buildFriendshipButton(friendId, response.newFriendStatus);
                $btn.replaceWith($newButton);
                let messageContent = "";
                switch (action) {
                    case "Add":
                        messageContent = "Friend Added Successfully";
                        break;
                    case "Cancel":
                        messageContent = "Friend Request Cancelled";
                        break;
                    case "Remove":
                        messageContent = "Friend Removed";
                    case "Accept":
                        messageContent = "Request Accepted";
                        break;
                }
                ShowSuccessMessage(messageContent);
            },
            error: function (xhr, status, error) {
                ShowErrorMessage("Error With Friend Requests");
            }
        });
    });

});
function buildFriendshipButton(friendId, action) {
    let btnClass = '';
    let btnText = '';

    switch (action) {
        case 'Accept':
            btnClass = 'btn-sm btn-success';
            btnText = 'Accept';
            break;
        case 'Remove':
            btnClass = 'btn-sm btn-danger';
            btnText = 'Remove';
            break;
        case 'Cancel':
            btnClass = 'btn-sm btn-secondary';
            btnText = 'Cancel';
            break;
        case 'Add':
            btnClass = 'btn-sm btn-info';
            btnText = 'Add';
            break;
        default:
            return $('<div>').text('You'); // fallback if no known action
    }

    return $(`<button type="submit" data-friendid="${friendId}" name="action" value="${action}" class="${btnClass}">${btnText}</button>`);
}