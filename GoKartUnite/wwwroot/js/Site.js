
$.ajaxSetup({
    headers: {
        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
    }
});