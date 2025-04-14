document.addEventListener("DOMContentLoaded", function () {
    const upvoteButtons = document.querySelectorAll('.upvote-btn');
    const commentCreateButtons = document.querySelectorAll(".create-comment-btn");
    const commentFetchButtons = document.querySelectorAll(".commentBtn");
    // CREATE BLOG POST ==============================
    $('#blogPostForm').on('submit', function (event) {
        event.preventDefault();

        $.validat

        var token = $('input[name="__RequestVerificationToken"]').val();
        if (!$("#blogPostForm").valid()) {
            alert("invalid");
            return;
        }
        $.ajax({
            type: 'POST',
            url: '/BlogHome/Create',
            data: $("#blogPostForm").serialize(),
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded', // FIXED
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.status == "success") {
                    $('#response').html('<div class="alert alert-success">Post created successfully!</div>');
                } else {
                    $('#response').html('<div class="alert alert-danger">Error: ' + response.message + '</div>');
                }
            },
            error: function (xhr, status, error) {
                console.log(error);
                var response = JSON.parse(xhr.responseText);
                if (response.message && Array.isArray(response.message)) {
                    $('#response').html('<div class="alert alert-danger">Error: ' + response.message[0] + '</div>');
                }
            }
        });


    });
    // CREATE BLOG POST ==============================

    // COMMENT CREATION ==============================
    commentCreateButtons.forEach(button => {
        button.addEventListener("click", function () {
            const postId = button.getAttribute("data-postid");
            const commentText = document.getElementById(`CommentInput-${postId}`).value;
            const baseUrl = `${window.location.origin}/BlogHome/CreateComment`;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            fetch(baseUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify({ blogId: postId, comment: commentText })
            }).then((response) => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error("Error creating comment");
            }).then((responseJson) => {
                console.log(responseJson);
            }).catch(err => alert("Error creating comment"));
        });
    });
    // COMMENT CREATION ==============================

    // UPVOTING ======================================
    upvoteButtons.forEach(button => {
        button.addEventListener('click', function () {
            const postId = button.getAttribute('data-id');
            const postUpvotes = button.getAttribute('data-upvoteCount');

            const baseUrl = `${window.location.origin}/BlogHome/UpvoteBlog?id=${postId}`;
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
            }).catch(err => alert("Error upvoting"));
        });
    });
    // UPVOTING ======================================

    // FETCH COMMENTS ================================
    commentFetchButtons.forEach(button => {
        button.addEventListener("click", async function () {
            const postElement = button.closest(".post");
            const postId = button.getAttribute("data-postid");
            let lastCommentId = postElement.getAttribute("data-lastcommentid") || "0";

            const baseUrl = `${window.location.origin}/BlogHome/GetCommentsForBlog?blogId=${postId}&lastCommentId=${lastCommentId}`;

            button.disabled = true;

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
                    commentSection.innerHTML += '<li class="text-danger small">No More Comments.</li>';
                    button.disabled = true;
                    return;
                }

                let newLastCommentId = null;

                data.forEach((comment, index) => {
                    const commentElement = createCommentElement(comment, index);
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
            } finally {
                setTimeout(() => {
                    button.disabled = false;
                }, 2000);
            }
        });
    });
    // FETCH COMMENTS ================================

    function createCommentElement(comment, index) {
        const template = document.getElementById("comment-template");

        if (!template) {
            console.error("Comment template not found!");
            return null;
        }

        const commentElement = template.content.cloneNode(true);
        const commentItem = commentElement.querySelector(".comment-item");

        commentElement.querySelector(".comment-author").textContent = comment.authorName;
        commentElement.querySelector(".comment-date").textContent = new Date(comment.typedAt).toLocaleDateString();
        commentElement.querySelector(".comment-text").textContent = comment.text;

        commentItem.classList.add('fade');
        setTimeout(() => {
            commentItem.classList.add('show');
        }, index * 200);

        return commentElement;
    }
});


