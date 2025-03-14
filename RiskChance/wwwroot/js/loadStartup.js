//function showDetails() {
//  const sidebar = document.getElementById("startupSidebar");
//  sidebar.classList.add("active");
//}

//function closeDetails() {
//  document.getElementById("startupSidebar").classList.remove("active");
//}

var page = 1;
var isLoading = false;

function loadMore() {
    if (isLoading) return;
    isLoading = true;
    $("#load-more-trigger .spinner-border").show();

    $.get("/Startup/LoadMore?page=" + page, function (data) {
        if (data.trim() === "") {
            $("#load-more-trigger").hide(); // Ẩn nếu không còn dữ liệu
        } else {
            $("#startup-container").append(data);
            page++;
        }
        isLoading = false;
        $("#load-more-trigger .spinner-border").hide();
    });
}

// Load dữ liệu lần đầu
$(document).ready(function () {
    loadMore();
});

// Khi cuộn đến cuối trang, tự động tải thêm
$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100) {
        loadMore();
    }
});