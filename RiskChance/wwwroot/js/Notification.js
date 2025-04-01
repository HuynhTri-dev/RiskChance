$(document).ready(function () {
    $.ajax({
        url: '/User/Notification/NotifyList',
        method: 'GET',
        success: function (data) {
            $('#notificationListContainer').html(data); // Chèn thông báo vào container
        }
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