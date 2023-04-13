const menu = document.getElementById("dropdownMenu");
document.querySelector(".right-btns")
    .addEventListener("click", (event) => {
        event.stopPropagation();
        menu.classList.toggle("show");
    });

window.onclick = function (event) {
    if (!event.target.matches('.right-btns')) {
        var dropdowns = document.getElementsByClassName("dropdown-menu");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}