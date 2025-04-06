$(document).ready(function () {
  // Kéo thả modal
  $("#messengerModal .modal-dialog").draggable({
    handle: ".modal-header",
  });

  $("#messengerModal").on("shown.bs.modal", function () {
    let chatBox = document.getElementById("chatBox");
    chatBox.scrollTop = chatBox.scrollHeight;
  });

    loadListFriend();
    getMessage();

    $('#searchUser').on('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault(); // Ngăn form submit (nếu có)
            let search = $(this).val().trim();
            if (search !== "") {
                FindName(search); // Gọi hàm AJAX
            }
        }
    });
    // goi rieng cho detail
    $('#contactBtn').click(function () {
        var receiverId = $(this).data('id');
        getMessage(receiverId);
        getInfo(receiverId);
    });

    $('.user-info-trigger').click(function () {
        var receiverId = $(this).data('id');
        console.log("Receiver ID:", receiverId); // Debugging
        if (!receiverId) {
            alert("Receiver ID is missing!");
            return;
        }
        getInfo(receiverId);
        getMessage(receiverId);
    });

    $('#sendMessageBtn').click(function (e) {
        e.preventDefault(); 

        var message = $('#messageInput').val().trim();
        var receiverId = $('#receiverId').val();

        if (message === "" || receiverId === "") {
            alert("Sign in to send message");
            return;
        }

        $.ajax({
            url: '/User/Messenger/SendMess',
            method: 'POST',
            data: {
                id: receiverId,
                mess: message
            },
            success: function (response) {
                $('#messageInput').val("");
                addMessToListA(response);
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi gửi:", error);
            }
        });
    });

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/messengerHub")
        .build();
    connection.on("ReceiveMess", function (message) {
        //console.log(message);
        getMessage(message.idNguoiGui);
        addMessToListB(message);
        getInfo(message.idNguoiGui)
    });
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});

function addMessToListA(mess) {
    // Tạo HTML cho tin nhắn
    var context = '<div class="mess-box mb-2 sender d-flex justify-content-end">' +
        '<div class="p-2 bg-primary text-white rounded">' +
        '<p>' + mess.noiDung + '</p>' +
        '</div>' +
        '</div>';

    // Thêm vào cuối danh sách chatBox
    $('#chatBox').append(context);
}

function addMessToListB(mess) {
    // Tạo HTML cho tin nhắn
    var context = '<div class="mess-box mb-2 receive d-flex justify-content-start">' +
        '<div class="p-2 bg-light rounded">' +
        '<p>' + mess.noiDung + '</p>' +
        '</div>' +
        '</div>';

    // Thêm vào cuối danh sách chatBox
    $('#chatBox').append(context);
}

function FindName(search) {
    $.ajax({
        url: "/User/Messenger/FindName",
        method: "GET",
        data: { name: search },
        success: function (response) {
            $("#userList").html(response);
        },
        error: function () {
            console.log("Error loading friend list.");
        }
    });
}

function loadListFriend() {
    $.ajax({
        url: "/User/Messenger/ListFriend",
        method: "GET",
        success: function (response) {
            $("#userList").html(response);
        },
        error: function () {
            console.log("Error loading friend list.");
        }
    });
}

function getMessage(userid) {
    $.ajax({
        url: "/User/Messenger/ListMessages",
        method: "GET",
        data: { receiverId: userid },
        success: function (response) {
            $("#chatBox").html(response);
            $("#receiverId").val(userid);
            getInfo(userid);
        },
        error: function () {
            console.log("Error loading friend list.");
        }
    });
}

function getInfo(userid) {
    $.ajax({
        url: "/User/Messenger/GetInfo",
        method: "GET",
        data: { id: userid },
        success: function (response) {
            $("#receiverInfo").html(response);
        },
        error: function () {
            console.log("Error loading friend list.");
        }
    });
}