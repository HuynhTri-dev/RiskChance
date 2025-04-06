$(document).ready(function () {
    
    $('.user-info-trigger').on('click', function (e) {
        e.preventDefault();

        const userId = $(this).data('id'); // Get user ID
        console.log("User ID:", userId);

        if (!userId) {
            alert('User ID is missing.');
            return;
        }

        // Fetch modal content
        $.ajax({
            url: '/User/GetUserInfo',
            type: 'GET',
            data: { id: userId },
            success: function (htmlContent) {
                // Inject content into the modal placeholder
                $('#modalPlaceholder').html(htmlContent);

                // Show the modal
                $('#userInfoModal').modal('show');

                $("#userInfoModal .modal-dialog").draggable({
                    handle: ".modal-header",
                });
            },
            error: function () {
                alert('Unable to fetch user information.');
            }
        });
    });
});
