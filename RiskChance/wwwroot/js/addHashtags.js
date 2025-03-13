$(document).ready(function () {
  // Add and remove hashtag
  let hashtags = new Set();

  $("#hashtagInput").on("keypress", function (e) {
    if (e.which === 13) {
      // Enter key
      e.preventDefault();
      let tag = $(this).val().trim();
      if (tag && !hashtags.has(tag)) {
        hashtags.add(tag);
        let span = $(
          `<span class="badge bg-secondary me-1">#${tag} <span class="ms-1 text-white" style="cursor:pointer;" onclick="removeTag(this, '${tag}')">×</span></span>`
        );
        $("#hashtagList").append(span);
        $(this).val("");
      }
    }
  });

  window.removeTag = function (element, tag) {
    $(element).parent().remove();
    hashtags.delete(tag);
  };

  // Review pic
  $("#postImage").on("change", function (event) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = function (e) {
        $("#imagePreview").attr("src", e.target.result).removeClass("d-none");
      };
      reader.readAsDataURL(file);
    } else {
      $("#imagePreview").attr("src", "").addClass("d-none");
    }
  });
});
