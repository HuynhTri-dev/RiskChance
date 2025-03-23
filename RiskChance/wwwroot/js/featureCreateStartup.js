$(document).ready(function () {
  // Active input for other business
  $("#Business").change(function () {
      if ($(this).val() === "0") {
        $("#otherBusiness").removeClass("d-none").focus();
    } else {
        $("#otherBusiness").addClass("d-none").val("");
    }
  });

  // Add img logo for startup to review
  $("#logoUpload").change(function (event) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = function (e) {
        $("#logoPreview").attr("src", e.target.result).removeClass("d-none");
      };
      reader.readAsDataURL(file);
    }
  });
});
