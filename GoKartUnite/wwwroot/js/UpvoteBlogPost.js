"use strict";


document.addEventListener("DOMContentLoaded", function () {
        const upvoteButtons = document.querySelectorAll('.upvote-btn');

        upvoteButtons.forEach(button => {
            button.addEventListener('click', function() {
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
    });