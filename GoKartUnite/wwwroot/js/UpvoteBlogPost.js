"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const upvoteButtons = document.querySelectorAll('.upvote-btn');

    upvoteButtons.forEach(button => {
        button.addEventListener('click', function () {
            const postId = button.getAttribute('data-id');
            const postUpvotes = button.getAttribute('data-upvoteCount');

            console.log('ID:', postId);
            const newUpvotes = parseInt(postUpvotes) + 1;
            button.setAttribute('data-upvoteCount', newUpvotes);
            button.parentNode.textContent = newUpvotes.toString();
            const baseUrl = `${window.location.origin}/BlogHome/UpvoteBlog?id=${postId}`;
            console.log(baseUrl);
            fetch(baseUrl, {
                method: "POST"
            }).catch(err => console.log(err));
        });
    });

    const commentBtns = document.querySelectorAll('.commentBtn');

    commentBtns.forEach(button => {
        button.addEventListener("click", function () {

            var lastCommentId = $(this).closest('.post').data('lastcommentid');
            const postId = button.getAttribute('data-postid');
            const baseUrl = `${window.location.origin}/BlogHome/GetCommentsForBlog?blogId=${postId}&lastCommentId=${lastCommentId}`;

            fetch(baseUrl, {
                method: "GET",
                headers: { 'Content-Type': 'application/json' },
            })
                .then(res => {
                    if (res.ok) {
                        return res.json();
                    }
                    throw new Error("Failed to load comments");
                })
                .then(data => {
                    console.log(data);
                    const commentSection = document.getElementsByClassName(`comment-section-${postId}`)[0];

                    commentSection.innerHTML = '';

                    const lastCommentId = data[data.length - 1]?.id;
                    $(this).closest('.post').data('lastcommentid', lastCommentId);

                    if (data.length == 0) {
                        commentSection.innerHTML = '<li class="text-danger small">No comments.</li>';
                    }

                    data.forEach(comment => {
                        const commentContainer = document.createElement('div');
                        commentContainer.classList.add('comment-item', 'p-2', 'border-bottom', 'rounded', 'text-wrap');

                        // Create a wrapper for the author and the date
                        const commentHeader = document.createElement('div');
                        commentHeader.classList.add('d-flex', 'justify-content-between', 'align-items-center');

                        const commentAuthor = document.createElement('div');
                        commentAuthor.classList.add('fw-bold', 'text-primary', 'small');
                        commentAuthor.textContent = comment.authorName;

                        const commentDate = document.createElement('div');
                        commentDate.classList.add('text-secondary', 'small');
                        commentDate.textContent = new Date(comment.typedAt).toLocaleDateString();

                        commentHeader.appendChild(commentAuthor);
                        commentHeader.appendChild(commentDate);

                        const commentContent = document.createElement('div');
                        commentContent.classList.add('text-muted', 'small', 'mt-1', 'text-wrap', 'text-break');

                        commentContent.textContent = comment.text;

                        commentContainer.appendChild(commentHeader);
                        commentContainer.appendChild(commentContent);

                        commentSection.appendChild(commentContainer);

                    });
                })
                .catch(err => {
                    console.log(err);
                    const commentSection = document.getElementById(`comment-section-${postId}`);
                    commentSection.innerHTML = '<li class="text-danger small">Failed to load comments.</li>';
                });
        });
    });
});
