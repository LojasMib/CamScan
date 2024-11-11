
function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");

    // Alterna a posição da sidebar
    if (sidebar.style.transform === "translateX(0px)") {
        sidebar.style.transform = "translateX(-250px)";
    } else {
        sidebar.style.transform = "translateX(0px)";
    }
}

function retractSidebar() {
    const sidebar = document.getElementById("sidebar");

    // Alterna a posição da sidebar
    if (sidebar.style.transform === "translateX(0px)") {
        sidebar.style.transform = "translateX(-250px)";
    }
}
