﻿"use strict";


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

                headers: {
                    'Content-Type': 'application/json',
                },
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

                    data.forEach(comment => {
                        const commentContainer = document.createElement('div');
                        commentContainer.classList.add('comment-item');

                        const commentAuthor = document.createElement('div');
                        commentAuthor.classList.add('CommentAuthor');
                        commentAuthor.textContent = comment.authorName;

                        const commentContent = document.createElement('div');
                        commentContent.classList.add('CommentContent');
                        commentContent.textContent = comment.text;

                        const commentDate = document.createElement('div');
                        commentDate.classList.add('CommentDate');
                        commentDate.textContent = new Date(comment.typedAt).toLocaleDateString();

                        commentContainer.appendChild(commentAuthor);
                        commentContainer.appendChild(commentContent);
                        commentContainer.appendChild(commentDate);

                        commentSection.appendChild(commentContainer);
                    });
                })
                .catch(err => {
                    console.log(err);
                    const commentSection = document.getElementById(`comment-section-${postId}`);
                    commentSection.innerHTML = '<li>Failed to load comments.</li>';
                });
        });
    });

});