document.addEventListener("DOMContentLoaded", function () {
    const upvoteButtons = document.querySelectorAll('.upvote-btn');

    upvoteButtons.forEach(button => {
        button.addEventListener('click', function () {
            const postId = button.getAttribute('data-id');
            const postUpvotes = button.getAttribute('data-upvoteCount');


            const baseUrl = `${window.location.origin}/BlogHome/UpvoteBlog?id=${postId}`;
            console.log(baseUrl);
            fetch(baseUrl, {
                method: "POST"
            }).then((response) => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error("Error updating upvote");
            }).then((responseJson) => {
                const newUpvotes = parseInt(postUpvotes) + parseInt(responseJson.message);
                button.setAttribute('data-upvoteCount', newUpvotes);
                $(`#UpvoteCounter-${postId}`).text(newUpvotes.toString());
            }).catch(err => alert("error"));
        });
    });

    const commentBtns = document.querySelectorAll(".commentBtn");

    commentBtns.forEach(button => {
        button.addEventListener("click", async function () {
            const postElement = button.closest(".post");
            const postId = button.getAttribute("data-postid");
            let lastCommentId = postElement.getAttribute("data-lastcommentid") || "0";

            const baseUrl = `${window.location.origin}/BlogHome/GetCommentsForBlog?blogId=${postId}&lastCommentId=${lastCommentId}`;

            try {
                const response = await fetch(baseUrl, {
                    method: "GET",
                    headers: { "Content-Type": "application/json" },
                });

                if (!response.ok) throw new Error("Failed to load comments");

                const data = await response.json();
                console.log("Fetched Comments:", data);

                const commentSection = document.querySelector(`.comment-section-${postId}`);
                if (!commentSection) return;


                if (data.length === 0) {
                    commentSection.innerHTML += '<li class="text-danger small">No comments.</li>';
                    return;
                }

                let newLastCommentId = null;

                data.forEach(comment => {
                    const commentElement = createCommentElement(comment);
                    if (commentElement) {
                        commentSection.appendChild(commentElement);
                    }
                    newLastCommentId = comment.id;
                });

                if (newLastCommentId) {
                    postElement.setAttribute("data-lastcommentid", newLastCommentId);
                }

            } catch (error) {
                console.error("Error loading comments:", error);
                const commentSection = document.querySelector(`.comment-section-${postId}`);
                if (commentSection) {
                    commentSection.innerHTML = '<li class="text-danger small">Failed to load comments.</li>';
                }
            }
        });
    });

    function createCommentElement(comment) {
        const template = document.getElementById("comment-template");

        if (!template) {
            console.error("Comment template not found!");
            return null;
        }

        const commentElement = template.content.cloneNode(true);

        commentElement.querySelector(".comment-author").textContent = comment.authorName;
        commentElement.querySelector(".comment-date").textContent = new Date(comment.typedAt).toLocaleDateString();
        commentElement.querySelector(".comment-text").textContent = comment.text;

        return commentElement;
    }
});
