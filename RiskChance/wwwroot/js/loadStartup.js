//function showDetails() {
//  const sidebar = document.getElementById("startupSidebar");
//  sidebar.classList.add("active");
//}

//function closeDetails() {
//  document.getElementById("startupSidebar").classList.remove("active");
//}

let page = 1;
let loading = false;

async function loadMoreStartups() {
    if (loading) return;
    loading = true;
    page++;

    try {
        const response = await fetch(`/Startup/LoadMore?page=${page}`);
        const data = await response.json();

        if (data.startups.length > 0) {
            data.startups.forEach(startup => {
                const startupHtml = `<div class="startup-item">
                    <img src="${startup.logoUrl}" alt="${startup.tenStartup}">
                    <h3>${startup.tenStartup}</h3>
                    <p>${startup.moTa}</p>
                </div>`;
                document.getElementById("startup-list").innerHTML += startupHtml;
            });

            if (!data.hasMore) {
                document.getElementById("load-more").style.display = "none";
            }
        }
    } catch (error) {
        console.error("Lỗi khi tải dữ liệu:", error);
    }

    loading = false;
}

// Sự kiện cuộn trang
window.addEventListener("scroll", function () {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100) {
        loadMoreStartups();
    }
});
