document.addEventListener("DOMContentLoaded", function () {
    console.log("Dropdown JS initialized ✅");

    const submenuToggles = document.querySelectorAll('.dropdown-submenu .dropdown-toggle');

    if (submenuToggles.length > 0) {
        submenuToggles.forEach(function (toggle) {
            toggle.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();

                const submenu = this.nextElementSibling;
                if (submenu && submenu.classList.contains('dropdown-menu')) {
                    document.querySelectorAll('.dropdown-submenu .dropdown-menu.show').forEach(function (openMenu) {
                        if (openMenu !== submenu) {
                            openMenu.classList.remove('show');
                        }
                    });

                    submenu.classList.toggle('show');
                }
            });
        });

        document.addEventListener('click', function () {
            document.querySelectorAll('.dropdown-submenu .dropdown-menu.show').forEach(function (submenu) {
                submenu.classList.remove('show');
            });
        });
    }

    // ✅ Null-check for search
    const searchBtn = document.querySelector('.search-btn');
    const searchInput = document.querySelector('.search-input');

    if (searchBtn && searchInput) {
        searchBtn.addEventListener('click', function () {
            const query = searchInput.value.trim();
            if (query) {
                window.location.href = `/search?q=${encodeURIComponent(query)}`;
            }
        });

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                searchBtn.click();
            }
        });
    }
});
