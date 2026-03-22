
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
        const isActiveCheck = document.getElementById("isActiveCheck");
        const incomeCategorySelect = document.getElementById("incomeCategorySelect");
        const expenseCategorySelect = document.getElementById("expenseCategorySelect");
        const houseHoldMemberSelect = document.getElementById("houseHoldMemberSelect");
        const fromInput = document.getElementById("fromInput");
        const toInput = document.getElementById("toInput");

        if (searchInput && sessionStorage.getItem("restoreFilterSearchFocus") === "true") {
            searchInput.focus();

            const valueLength = searchInput.value.length;
            searchInput.setSelectionRange(valueLength, valueLength);

            sessionStorage.removeItem("restoreFilterSearchFocus");
        }

        function submitFormWithFocusRestore() {
            if (searchInput === document.activeElement) {
                sessionStorage.setItem("restoreFilterSearchFocus", "true");
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

        if (isActiveCheck) {
            isActiveCheck.addEventListener("change", function () {
                form.submit();
            });
        }

        if (houseHoldMemberSelect) {
            houseHoldMemberSelect.addEventListener("change", function () {
                form.submit();
            });
        }

        if (incomeCategorySelect) {
            incomeCategorySelect.addEventListener("change", function () {
                form.submit();
            });
        }

        if (expenseCategorySelect) {
            expenseCategorySelect.addEventListener("change", function () {
                form.submit();
            });
        }

        if (fromInput) {
            fromInput.addEventListener("change", function () {
                form.submit();
            });
        }

        if (toInput) {
            toInput.addEventListener("change", function () {
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
                sessionStorage.setItem("restoreFilterSearchFocus", "true");
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


document.addEventListener("DOMContentLoaded", function () {
    const deleteforms = document.querySelectorAll(".delete-form");

    deleteforms.forEach(function (form) {
        form.addEventListener("submit", function (e) {
            const button = form.querySelector("button[type='submit']");
            const message = button?.dataset.confirmMessage || "Opravdu chcete položku smazat ?";

            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
});

