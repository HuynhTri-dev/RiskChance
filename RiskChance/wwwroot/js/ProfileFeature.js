document.addEventListener("DOMContentLoaded", function () {
    var successAlert = document.getElementById("success-alert");
    if (successAlert) {
        setTimeout(function () {
            var alert = new bootstrap.Alert(successAlert);
            alert.close();
        }, 2000); // 3000ms = 3 giây
    }

    $("#AvatarFile").change(function () {
        let file = this.files[0];
        if (file) {
            let reader = new FileReader();
            reader.onload = function (e) {
                $("#avatarPreview").attr("src", e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});