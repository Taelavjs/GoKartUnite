let pagesScrolled = 0;

$(document).ready(function () {
    $(window).on('scroll', function () {
        var t = $(this);
        if ($(window).scrollTop() + $(window).height() >= $(document).height()) {
            $.ajax({
                url: `Home/InfiniteScroll?pagesScrolled=${pagesScrolled}`,
                type: 'POST',
                async: false,
                success: function (data) {
                    $(".infiniteScrollDiv").append(data);
                    pagesScrolled++;
                },
                error: function () {
                    alert('Error loading more posts.');
                }
            });
        }
    });
});
