$(".slider").slick({
    attrow: true,
    dots: false,
    speed: 500,
    infinite: true,
    centerMode: true,
    // variableWidth: true,
    draggable: false,
    adaptiveHeight: true,
    slidesToShow: 1
});
$(".slider")
    .on("beforeChange", function (event, slick, currentSlide, nextSlide) {
        $(".slick-list").addClass("do-tans");
    })
    .on("afterChange", function () {
        $(".slick-list").removeClass("do-tans");
    })