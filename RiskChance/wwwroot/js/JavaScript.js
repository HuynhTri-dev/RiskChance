let page = 1;
let isLoading = false;

async function loadNews() {
    if (isLoading) return; // Nếu đang tải, không làm gì cả
    isLoading = true;
    $("#loading").removeClass("d-none"); // Hiện biểu tượng loading

    try {
        let response = await fetch(`/TinTuc/LoadNews?page=${page}`);
        let data = await response.json();

        if (data.length === 0) {
            $(window).off("scroll"); // Hết dữ liệu thì ngừng gọi API
            $("#loading").hide();
            return;
        }

        data.forEach(news => {
            $("#newsContainer").append(`
        <div class="news-item row align-items-center mb-3">
            <!-- Hình ảnh tin tức -->
            <div class="col-lg-4">
                <a href="./detailNews.html?id=${news.idTinTuc}">
                    <img src="${news.imgTinTuc ? news.imgTinTuc : '/default-image.jpg'}"
                        alt="News Image"
                        class="news-img img-fluid rounded-3 d-none d-lg-block shadow" />
                </a>
            </div>

            <!-- Nội dung tin tức -->
            <div class="news-content-box col-lg-8">
                <div class="news-content mb-2">
                    <a href="./detailNews.html?id=${news.idTinTuc}" class="text-decoration-none">
                        <h4 class="h4 fw-bold news-title text-black">${news.noiDung.substring(0, 50)}...</h4>
                    </a>
                    <p class="news-desc text-muted line-clamp-2">
                        ${news.noiDung}
                    </p>
                    <p class="news-date text-secondary small">
                        <i class="fa-solid fa-calendar"></i> ${new Date(news.ngayDang).toLocaleDateString()}
                    </p>
                </div>

                <!-- Tác giả -->
                <a class="news-author d-flex align-items-center text-decoration-none" href="./detailNews.html?id=${news.idTinTuc}">
                    <img class="author-img rounded-circle me-2 border"
                         src="../assets/cooperators/TPS.png"
                         alt="Author"
                         width="40"
                         height="40" />
                    <p class="author-name fw-bold text-dark">${news.nguoiDang}</p>
                </a>
            </div>
        </div>
            `);
        });


        page++; // Tăng trang sau khi load thành công
    } catch (error) {
        console.error("Lỗi tải tin tức:", error);
    } finally {
        isLoading = false;
        $("#load-more-trigger").addClass("d-none"); // Ẩn biểu tượng loading
    }
}

// Khi kéo xuống hết trang thì load tiếp
$(window).on("scroll", function () {
    if ($(window).scrollTop() + $(window).height() >= $(document).height() - 50) {
        loadNews();
    }
});

// Load tin tức ngay khi vào trang
$(document).ready(loadNews);