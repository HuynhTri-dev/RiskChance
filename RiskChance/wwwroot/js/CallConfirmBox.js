function confirmDelete(id, deleteUrl) {
    $("#deleteItemId").val(id);
    $("#deleteForm").attr("action", deleteUrl);

    $("#confirmDeleteModal").modal("show");
}