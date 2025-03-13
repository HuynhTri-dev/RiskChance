$(document).ready(function () {
  // Kéo thả modal
  $("#messengerModal .modal-dialog").draggable({
    handle: ".modal-header",
  });

  $("#messengerModal").on("shown.bs.modal", function () {
    let chatBox = document.getElementById("chatBox");
    chatBox.scrollTop = chatBox.scrollHeight;
  });
});
