function showDetails() {
  const sidebar = document.getElementById("startupSidebar");
  sidebar.classList.add("active");
}

function closeDetails() {
  document.getElementById("startupSidebar").classList.remove("active");
}
