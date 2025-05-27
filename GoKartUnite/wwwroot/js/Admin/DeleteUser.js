document.addEventListener("DOMContentLoaded", () => {

    document.querySelectorAll('.deleteUser').forEach(btn => {
        const userId = btn.getAttribute("data-userId");
        console.log("Found button for user:", userId);

        btn.addEventListener('click', () => {
            console.log("Delete clicked for user:", userId);
            $.ajax({
                url: `DeleteKarterById?id=${userId}`, success: function (result) {
                    ShowSuccessMessage(result.message);
                    console.log("btn to remove:", btn);
                    btn.closest("tr").remove();

                }, error: function (err) {
                    const errorMessage = err.responseText || "An unknown error occurred.";

                    ShowSuccessMessage(errorMessage);

                }
            });
        });
    });
});