



const VerifySuccess = $('#VerifySuccesAlert');
const VerifyError = $('#VerifyFailAlert');
VerifySuccess.addClass('d-none');
VerifyError.addClass('d-none');

function ShowSuccessMessage(messageContent) {
    VerifySuccess.removeClass('d-none');
    VerifySuccess.text(messageContent);
    console.log(messageContent);

    setTimeout(() => VerifySuccess.addClass('d-none'), 3000);
}


function ShowErrorMessage(messageContent) {
    VerifyError.removeClass('d-none');
    console.log(messageContent);
    VerifyError.text(messageContent);

    setTimeout(() => VerifyError.addClass('d-none'), 3000);
}

