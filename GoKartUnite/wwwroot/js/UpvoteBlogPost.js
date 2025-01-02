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
            fetch(`BlogHome/UpvoteBlog?id=${postId}`, {
                method: "POST"
            }).catch(err => console.log(err));
        });
    });


    const commentBtns = document.querySelectorAll('.commentBtn');

    commentBtns.forEach(button => {
        button.addEventListener("click", function () {
            const postId = button.getAttribute('data-postid');
            fetch(`BlogHome/GetCommentsForBlog?blogId=${postId}`, {
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

                    data.forEach(comment => {
                        const li = document.createElement('li');
                        li.textContent = comment.Text;
                        commentSection.appendChild(li);
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