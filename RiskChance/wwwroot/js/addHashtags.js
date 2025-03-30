$(document).ready(function () {
    // Add and remove hashtag

    let hashtagsJson = $("#hiddenHashtags").val();

    if (hashtagsJson == null) {
        let hashtags = [];
    }
    else
    {
        hashtags = JSON.parse(hashtagsJson);
    }

    

    $("#hashtagInput").on("keypress", function (e) {
        if (e.which === 13) {  // Enter key
            e.preventDefault();
            let tag = $(this).val().trim();
            if (tag && !hashtags.includes(tag)) {
                hashtags.push(tag);
                let span = $(`
          <span class="badge bg-secondary me-1">
            ${tag} <span class="ms-1 text-white" style="cursor:pointer;" onclick="removeTag('${tag}', this)">×</span>
          </span>
        `);
                $("#hashtagList").append(span);
                $(this).val("");
                updateHiddenInput();
            }
        }
    }); 

    window.removeTag = function (tag, element) {
        $(element).parent().remove();
        hashtags = hashtags.filter(t => t !== tag);
        updateHiddenInput();
    };

    function updateHiddenInput() {
        $("#hiddenHashtags").val(JSON.stringify(hashtags));
        //$("#hiddenHashtags").val(hashtags);
    }

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
