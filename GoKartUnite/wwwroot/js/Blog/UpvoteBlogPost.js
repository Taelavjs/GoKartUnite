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

    // Select all comment buttons
    const commentBtns = document.querySelectorAll(".commentBtn");

    commentBtns.forEach(button => {
        button.addEventListener("click", async function () {
            const postElement = button.closest(".post");
            const postId = button.getAttribute("data-postid");
            let lastCommentId = postElement.getAttribute("data-lastcommentid") || "0";

            // API URL for fetching comments
            const baseUrl = `${window.location.origin}/BlogHome/GetCommentsForBlog?blogId=${postId}&lastCommentId=${lastCommentId}`;

            try {
                const response = await fetch(baseUrl, {
                    method: "GET",
                    headers: { "Content-Type": "application/json" },
                });

                if (!response.ok) throw new Error("Failed to load comments");

                const data = await response.json();
                console.log("Fetched Comments:", data);

                // Get the corresponding comment section
                const commentSection = document.querySelector(`.comment-section-${postId}`);
                if (!commentSection) return;

                // Clear existing comments before appending new ones
                commentSection.innerHTML = "";

                if (data.length === 0) {
                    commentSection.innerHTML = '<li class="text-danger small">No comments.</li>';
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

                // Update last comment ID
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

    // Function to create comment elements using the <template>
    function createCommentElement(comment) {
        const template = document.getElementById("comment-template");

        if (!template) {
            console.error("Comment template not found!");
            return null;
        }

        // Clone the template
        const commentElement = template.content.cloneNode(true);

        // Populate the template with data
        commentElement.querySelector(".comment-author").textContent = comment.authorName;
        commentElement.querySelector(".comment-date").textContent = new Date(comment.typedAt).toLocaleDateString();
        commentElement.querySelector(".comment-text").textContent = comment.text;

        return commentElement;
    }
});
