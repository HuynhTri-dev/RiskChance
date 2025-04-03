$(document).ready(function () {
    // Áp dụng cho cả hai nhóm sao
    handleStarRating('rating-start', 'ratingValue');

    const observer = new MutationObserver(() => {
        if (document.querySelector('.rating-start-edit')) {
            handleStarRating('rating-start-edit', 'ratingValueEdit');
            observer.disconnect(); 
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });

    // can sua lai them ve loi disconnection khi nhan
    // Comment
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/postCommentNewsHub")
        .build();
    connection.on("ReceiveCommentNews", function (comment) {
        console.log("Comment news: ", comment);
        //addCommentToUI(comment);
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});

function CallEditBox(id) {
    console.log("Chỉnh sửa bình luận với ID: " + id);
    $.ajax({
        url: '/User/CommentNews/Edit/' + id,
        type: 'GET',
        success: function (response) {
            $("#comment-" + id).html(response);
        },
        error: function () {
            alert("Có lỗi xảy ra, vui lòng thử lại.");
        }
    });
}


function handleStarRating(starClass, inputId) {
    const stars = document.querySelectorAll(`.${starClass}`);

    stars.forEach(star => {
        star.addEventListener('mouseover', function () {
            let value = this.getAttribute('data-value');
            stars.forEach(s => {
                s.classList.remove('selected');
                if (s.getAttribute('data-value') <= value) {
                    s.classList.add('selected');
                }
            });
        });

        star.addEventListener('click', function () {
            let value = this.getAttribute('data-value');
            document.getElementById(inputId).value = value;
        });

        star.addEventListener('mouseleave', function () {
            let selectedValue = document.getElementById(inputId).value;
            stars.forEach(s => {
                s.classList.remove('selected');
                if (s.getAttribute('data-value') <= selectedValue) {
                    s.classList.add('selected');
                }
            });
        });
    });
}

function addCommentToUI(comment) {
    let stars = "";
    for (let i = 0; i < comment.DiemDanhGia; i++) {
        stars += `<i class="fa-solid fa-star point"></i>`;
    }

    let formattedDate = new Date(comment.NgayBinhLuan).toLocaleString("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });

    let html = `
                    <div class="d-flex align-items-start mb-3 p-3 rounded border shadow-sm">
                        <!-- Avatar -->
                        <img src="${comment.NguoiDung.AvatarUrl}"
                             alt="User Avatar"
                             class="rounded-circle me-3"
                             width="50" height="50" />

                        <div>
                            <div class="d-flex align-center">
                                <h6 class="fw-bold mb-1">${comment.NguoiDung.HoTen}</h6>
                                ${stars}
                            </div>

                            <!-- Nội dung bình luận -->
                            <p class="mb-1">${comment.NhanXet}</p>

                            <!-- Ngày giờ bình luận -->
                            <small class="text-muted">${formattedDate}</small>
                        </div>
                    </div>`;

    document.getElementById("commentsList").insertAdjacentHTML("afterbegin", html);
}