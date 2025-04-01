$(document).ready(function () {
    // Theo dõi thay đổi DOM để gán event cho .rating-start-edit
    const observer = new MutationObserver(() => {
        document.querySelectorAll('.rating-start-edit').forEach(star => {
            if (!star.dataset.initialized) {
                handleStarRating('rating-start-edit', 'ratingValueEdit');
                star.dataset.initialized = "true"; // Đánh dấu đã gán event
            }
        });
    });

    observer.observe(document.body, { childList: true, subtree: true });

    // Kết nối SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/postCommentStartupHub")
        .build();

    connection.on("ReceiveComment", function (IDNguoiDang, Comment, DiemDanhGia, AvatarUrl, NgayDanhGia) {
        let starsHtml = "";
        for (let i = 0; i < DiemDanhGia; i++) {
            starsHtml += '<i class="fa-solid fa-star point"></i> ';
        }

        const commentHtml = `
            <div class="d-flex align-items-start mb-3 p-3 rounded border shadow-sm">
                <img src="${AvatarUrl || '/assets/user/image.png'}" 
                     alt="User Avatar" class="rounded-circle me-3" width="50" height="50" />
                <div>
                    <div class="d-flex align-items-center">
                        <h6 class="fw-bold mb-1 me-2">${Comment}</h6>
                        ${starsHtml}
                    </div>
                    <p class="mb-1">${Comment}</p>
                    <small class="text-muted">${new Date(NgayDanhGia).toLocaleString()}</small>
                </div>
            </div>`;

        const commentsList = document.getElementById("commentsList");
        if (commentsList) {
            commentsList.insertAdjacentHTML("afterbegin", commentHtml);
        } else {
            console.error("Không tìm thấy phần tử #commentsList");
        }

        console.log("Nhận cập nhật từ SignalR, cập nhật danh sách bình luận...");
    });

    connection.start()
        .then(() => console.log("SignalR Comment connected"))
        .catch(err => console.error("SignalR error:", err));
});

// Chỉnh sửa bình luận
function CallEditBox(id) {
    console.log("Chỉnh sửa bình luận với ID: " + id);
    $.ajax({
        url: '/User/CommentStartup/Edit/' + id,
        type: 'GET',
        success: function (response) {
            $("#comment-" + id).html(response);
        },
        error: function (xhr, status, error) {
            console.error("Lỗi AJAX:", status, error);
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
