$(window).on("scroll", function () {
  let scrollPosition = $(window).scrollTop();
  let opacity = Math.max(0, 1 - scrollPosition / 500);

  let maxMoveX = $(window).width() - $("#rocket").width();
  let maxMoveY = $(window).height() - $("#rocket").height();

  let moveX = Math.min(scrollPosition / 5, maxMoveX);
  let moveY = Math.min(-scrollPosition / 5, maxMoveY);

  // Điều kiện dừng: khi `scrollPosition` đạt 1000px
  if (scrollPosition >= 500) {
    return;
  }

  $("#rocket").css({
    transform: `translate(${moveX}px, ${moveY}px)`,
  });

  $("#fire").css({
    opacity: opacity,
    transform: `translate(${moveX}px, ${moveY}px)`,
  });
});

let $list = $(".startup-list");
let itemWidth = $(".startup-item").outerWidth(true);
let position = 0;

$("#nextBtn").click(function () {
  if (position > -itemWidth * ($(".startup-item").length - 3)) {
    position -= itemWidth;
    $list.css("transform", `translateX(${position}px)`);
  }
});

$("#prevBtn").click(function () {
  if (position < 0) {
    position += itemWidth;
    $list.css("transform", `translateX(${position}px)`);
  }
});
