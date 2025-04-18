document.addEventListener('DOMContentLoaded', function () {
    let blogPostsButton = document.getElementById("ShowUsersPosts");
    const showPosts = document.getElementById("showPosts");
    blogPostsButton.addEventListener("click", function () {
        const karterId = this.getAttribute("data-karterId");
        $.ajax({
            url: `${window.location.origin}/Admin/GetKartersBlogPosts?karterid=${karterId}`,
            type: 'GET',
            success: function (response) {
                showPosts.innerHTML = response;
            },
            error: function (xhr, status, error) {
                alert("Error");
            }
        });
    });

    let showCommentsBtn = document.getElementById("CommentsShow");
    showCommentsBtn.addEventListener("click", function () {
        const karterId = this.getAttribute("data-karterId");
        $.ajax({
            url: `${window.location.origin}/Admin/GetKartersComments?karterid=${karterId}`,
            type: 'GET',
            success: function (response) {
                showPosts.innerHTML = response;
            },
            error: function (xhr, status, error) {
                alert("Error");
            }
        });
    });
});
