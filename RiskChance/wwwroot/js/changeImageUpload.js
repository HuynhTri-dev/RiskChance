$(document).ready(function () {
  $("#imageUpload").change(function () {
    let file = this.files[0];
    if (file) {
      let reader = new FileReader();
      reader.onload = function (e) {
        $("#profileImage").attr("src", e.target.result);
      };
      reader.readAsDataURL(file);
    }
  });
});
