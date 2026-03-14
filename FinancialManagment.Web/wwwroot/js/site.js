
document.addEventListener("DOMContentLoaded", function () {
    const alerts = document.querySelectorAll(".alert-dismissible");

    if (alerts.length === 0) return;

    setTimeout(function () {

        alerts.forEach(function (alert) {

            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        });
    }, 4000);
});


(function () {
    function initFilters() {
        const form = document.getElementById("FilterForm");
        if (!form) {
            return;
        }

        const searchInput = document.getElementById("searchInput");
        const clearBtn = document.getElementById("clearSearchBtn");
        const sortSelect = document.getElementById("sortSelect");
        const descCheck = document.getElementById("descCheck");
        const pageSizeSelect = document.getElementById("pageSize");

        if (searchInput && sessionStorage.getItem("restoreIncomeCategorySearchFocus") === "true") {
            searchInput.focus();

            const valueLength = searchInput.value.length;
            searchInput.setSelectionRange(valueLength, valueLength);

            sessionStorage.removeItem("restoreIncomeCategorySearchFocus");
        }

        function submitFormWithFocusRestore() {
            if (searchInput === document.activeElement) {
                sessionStorage.setItem("restoreIncomeCategorySearchFocus", "true");
            }

            form.submit();
        }

        if (sortSelect) {
            sortSelect.addEventListener("change", function () {
                form.submit();
            });
        }

        if (descCheck) {
            descCheck.addEventListener("change", function () {
                form.submit();
            });
        }

        if (pageSizeSelect) {
            pageSizeSelect.addEventListener("change", function () {
                form.submit();
            });
        }

        let timerId = null;
        if (searchInput) {
            searchInput.addEventListener("input", function () {
                if (timerId) {
                    clearTimeout(timerId);
                }

                timerId = setTimeout(function () {
                    submitFormWithFocusRestore();
                }, 700);
            });
        }

        if (clearBtn && searchInput) {
            clearBtn.addEventListener("click", function () {
                searchInput.value = "";
                sessionStorage.setItem("restoreIncomeCategorySearchFocus", "true");
                form.submit();
            });
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initFilters);
    } else {
        initFilters();
    }
})();