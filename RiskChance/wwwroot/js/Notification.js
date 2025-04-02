$(document).ready(function () {
    $.ajax({
        url: '/User/Notification/NotifyList',
        method: 'GET',
        success: function (data) {
            $('#notificationListContainer').html(data); // Chèn thông báo vào container
        }
    });

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();
    connection.on("ReceiveNotification", function (message) {
        console.log(typeof (message));
        console.log("Received message:", message);

        // Tạo một thẻ div mới
        var notificationDiv = document.createElement("div");
        notificationDiv.classList.add("notification-item");

        // Gán nội dung vào div
        notificationDiv.innerHTML = `
            <a id="wrapper-${message.idNoti}" href="#" onclick="markAsRead(${message.idNoti})" 
               class="container-fluid notify-box d-flex align-items-center text-decoration-none">
                <div class="avatar-box d-none d-sm-block">
                    <img class="rounded-circle user-img"
                         src="${message.nguoiGui && message.nguoiGui.AvatarUrl ? message.nguoiGui.AvatarUrl : '/assets/user/image.png'}"
                         alt="avatar" />
                </div>
                <div class="notify-content">
                    <p class="line-clamp-2">${message.noiDung}</p>
                    <span class="date-text">${new Date(message.ngayGui).toLocaleDateString('en-GB')}</span>
                </div>
                <div class="notify-status d-none d-sm-block">
                    <div id="status-${message.idNoti}" class="status rounded-circle"></div>
                </div>
            </a>
        `;

        // Lấy container và thêm notification vào đầu danh sách
        var container = document.getElementById("notificationListContainer");
        if (container) {
            container.insertBefore(notificationDiv, container.firstChild);
            console.log("Notification added successfully!");
        } else {
            console.error("Container #notificationListContainer not found!");
        }
    });


    // Bắt đầu kết nối SignalR
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});

// Đánh dấu thông báo là đã đọc
function markAsRead(notificationId) {
    $.ajax({
        url: '/User/Notification/MarkAsRead',
        method: 'POST',
        data: { notificationId: notificationId },
        success: function () {
            var statusElementId = '#status-' + notificationId;
            $(statusElementId).removeClass('status');

            var wrapper = '#wrapper-' + notificationId;
            $(wrapper).addClass('read');
        }
    });
}