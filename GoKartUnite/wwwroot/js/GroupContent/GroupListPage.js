$(document).ready(function () {
    function getTemplate(templateId, groupId) {
        var template = document.getElementById(templateId);
        var clone = template.content.cloneNode(true);
        var form = clone.querySelector("form");
        form.setAttribute("data-groupid", groupId);
        return clone;
    }

    $("#createGroupForm").submit(function (event) {
        event.preventDefault();
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (response) {
                $("#createGroupForm")[0].reset();
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    });

    $(document).on('submit', '.JoinGroupRequest', function (e) {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var groupId = $(this).attr('data-groupId');
        var groupActionsDiv = $('#group-actions-' + groupId);

        $.ajax({
            url: $(this).data('url'),
            type: 'POST',
            data: {
                __RequestVerificationToken: token,
                GroupId: groupId
            },
            success: function (result) {
                var membersCountSpan = $('#members-count-' + groupId);
                var newMemberCount = parseInt(membersCountSpan.text()) + 1;
                membersCountSpan.text(newMemberCount);
                groupActionsDiv.empty().append(getTemplate("leave-group-template", groupId));
            }
        });
        return false;
    });

    $(document).on('submit', '.LeaveGroupRequest', function (e) {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var groupId = $(this).attr('data-groupId');
        var groupActionsDiv = $('#group-actions-' + groupId);

        $.ajax({
            url: $(this).data('url'),
            type: 'POST',
            data: {
                __RequestVerificationToken: token,
                GroupId: groupId
            },
            success: function (result) {
                var membersCountSpan = $('#members-count-' + groupId);
                var newMemberCount = parseInt(membersCountSpan.text()) - 1;
                membersCountSpan.text(newMemberCount);
                groupActionsDiv.empty().append(getTemplate("join-group-template", groupId));

            }
        });
        return false;
    });


    var selector = document.getElementById("GroupListSelector");

    selector.addEventListener("change", function () {
        GetListOfGroupsSorted(selector.value);
    });

    $('#search-button').on('click', function () {
        var searchTerm = $('#SearchGroupsInput').val();
        var selectorVal = selector.value;
        GetListOfGroupsFiltered(searchTerm, selectorVal);
    });
});

const GetListOfGroupsSorted = (filterValue) => {

    $.ajax({
        type: "GET",
        url: `${window.location.origin}/Group/SortedListOfGroups?SortedBy=${filterValue}`,
        contentType: "application/json; charset=utf-8",
        success: async (html) => {
            $("#GroupListContent").empty();
            $("#GroupListContent").append(html);
        }
    });
}

const GetListOfGroupsFiltered = (searchTerm, filterTerm) => {
    $.ajax({
        type: "GET",
        url: `${window.location.origin}/Group/SortedListOfGroups?SortedBy=${filterTerm}&query=${searchTerm}`,
        contentType: "application/json; charset=utf-8",
        success: function (html) {
            $("#GroupListContent").empty().append(html);
        }
    });
}
