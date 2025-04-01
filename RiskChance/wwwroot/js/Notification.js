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
        addNotify(message);
    });

    // Bắt đầu kết nối SignalR
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});

function addNotify(message) {
    var mess = `<a id="wrapper-${message.IDNoti}" href="#" onclick="markAsRead(${message.IDNoti})" class="container-fluid notify-box d-flex align-items-center text-decoration-none">
            <div class="avatar-box d-none d-sm-block">
                <img class="rounded-circle user-img"
                     src="${message.NguoiGui.AvatarUrl ? message.NguoiGui.AvatarUrl : '/assets/user/image.png'}"
                     alt="avatar" />
            </div>

            <div class="notify-content">
                <p class="line-clamp-2">
                    ${message.NoiDung}
                </p>
                <span class="date-text">${new Date(message.NgayGui).toLocaleDateString('en-GB')}</span>
            </div>

            <div class="notify-status d-none d-sm-block">
                <div id="status-${message.IDNoti}" class="status rounded-circle"></div>
            </div>
        </a>`;

    $('#notificationListContainer ').prepend('<div class="notification-item">' + mess + '</div>');
}

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