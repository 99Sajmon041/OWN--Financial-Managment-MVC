
//TempData[Message] - dynamic alert in Layout view
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


//Function for dynamic fitering - Income, Expense Index actions
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

//Confirm window to delete item in Index actions
document.addEventListener("DOMContentLoaded", function () {
    const deleteForms = document.querySelectorAll(".delete-form");
    const confirmButtons = document.querySelectorAll("[data-confirm-message]");

    deleteForms.forEach(function (form) {
        form.addEventListener("submit", function (e) {
            const button = form.querySelector("button[type='submit']");
            const message = button?.dataset.confirmMessage || "Opravdu chcete položku smazat?";

            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });

    confirmButtons.forEach(function (button) {
        if (button.closest(".delete-form")) {
            return;
        }

        button.addEventListener("click", function (e) {
            const message = button.dataset.confirmMessage || "Opravdu chcete položku smazat?";

            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
});


//Dynamic filtering in Statistics JS (Multiple) Index view
document.addEventListener("DOMContentLoaded", function () {
    const houseHoldMemberIdsElement = document.getElementById("houseHoldMemberIds");
    const incomeCategoryIdsElement = document.getElementById("incomeCategoryIds");
    const expenseCategoryIdsElement = document.getElementById("expenseCategoryIds");
    const selectedYearElement = document.getElementById("selectedYear");
    const selectedMonthElement = document.getElementById("selectedMonth");
    const clearFiltersBtnElement = document.getElementById("clearFiltersBtn");
    const statisticsJsChartElement = document.getElementById("statisticsJsChart");

    if (!houseHoldMemberIdsElement ||
        !incomeCategoryIdsElement ||
        !expenseCategoryIdsElement ||
        !selectedYearElement ||
        !selectedMonthElement ||
        !clearFiltersBtnElement ||
        !statisticsJsChartElement) {
        return;
    }

    let statisticsJsChart = null;
    let suppressStatisticsReload = false;

    function getSelectedValues(selectElement) {
        return Array.from(selectElement.selectedOptions).map(function (option) {
            return Number(option.value);
        });
    }

    function getCurrentFilters() {
        return {
            householdMembersId: getSelectedValues(houseHoldMemberIdsElement),
            incomeCategoriesId: getSelectedValues(incomeCategoryIdsElement),
            expenseCategoriesId: getSelectedValues(expenseCategoryIdsElement),
            selectedYear: Number(selectedYearElement.value),
            selectedMonth: Number(selectedMonthElement.value)
        };
    }

    function setDefaultTomSelectValue(tomSelectInstance) {
        tomSelectInstance.clear(true);
        tomSelectInstance.addItem("0", true);
    }

    function normalizeTomSelectSelection(tomSelectInstance) {
        const values = tomSelectInstance.getValue();

        if (!values || values.length === 0) {
            setDefaultTomSelectValue(tomSelectInstance);
            return;
        }

        if (values.includes("0") && values.length > 1) {
            const filteredValues = values.filter(function (value) {
                return value !== "0";
            });

            tomSelectInstance.setValue(filteredValues, true);
        }
    }

    async function loadStatistics() {
        if (suppressStatisticsReload) {
            return;
        }

        const filters = getCurrentFilters();

        const response = await fetch("/Statistics/JsIndexData", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(filters)
        });

        if (!response.ok) {
            return;
        }

        const data = await response.json();

        updateSummary(data);

        if (statisticsJsChart) {
            statisticsJsChart.destroy();
        }

        createStatisticsJsChart(data);
    }

    function updateSummary(data) {
        const incomeEl = document.getElementById("incomeTotal");
        const expenseEl = document.getElementById("expenseTotal");
        const balanceEl = document.getElementById("balanceTotal");
        const balanceCardMessageEl = document.getElementById("balanceMessage");

        if (!incomeEl || !expenseEl || !balanceEl || !balanceCardMessageEl) {
            return;
        }

        incomeEl.innerText = data.incomeTotal.toLocaleString("cs-CZ", {
            style: "currency",
            currency: "CZK"
        });

        expenseEl.innerText = data.expenseTotal.toLocaleString("cs-CZ", {
            style: "currency",
            currency: "CZK"
        });

        balanceEl.innerText = data.balance.toLocaleString("cs-CZ", {
            style: "currency",
            currency: "CZK"
        });

        balanceEl.classList.remove("text-success", "text-danger");
        balanceEl.classList.add(data.balance >= 0 ? "text-success" : "text-danger");

        const selectedMonthItem = data.months.find(function (month) {
            return Number(month.value) === data.selectedMonth;
        });

        balanceCardMessageEl.innerText = data.yearly
            ? `Vývoj financí za rok ${data.selectedYear}`
            : `Vývoj financí za ${selectedMonthItem ? selectedMonthItem.text : ""} / ${data.selectedYear}`;
    }

    function createStatisticsJsChart(data) {
        const labels = Object.keys(data.balanceChart);

        const incomeValues = labels.map(function (label) {
            return data.incomeChart[label] ?? 0;
        });

        const expenseValues = labels.map(function (label) {
            return data.expenseChart[label] ?? 0;
        });

        const balanceValues = labels.map(function (label) {
            return data.balanceChart[label] ?? 0;
        });

        const allValues = incomeValues.concat(expenseValues, balanceValues);

        const maxValue = Math.max(...allValues, 0);
        const minValue = Math.min(...allValues, 0);

        const suggestedMax = maxValue > 0 ? maxValue * 1.2 : 100;
        const suggestedMin = minValue < 0 ? minValue * 1.2 : 0;

        statisticsJsChart = new Chart(statisticsJsChartElement, {
            type: "line",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "Zůstatek",
                        data: balanceValues,
                        borderColor: "rgba(108, 117, 125, 0.35)",
                        backgroundColor: "rgba(108, 117, 125, 0.08)",
                        fill: true,
                        tension: 0.35,
                        pointRadius: 1,
                        pointHoverRadius: 3,
                        borderWidth: 1.5
                    },
                    {
                        label: "Příjmy",
                        data: incomeValues,
                        borderColor: "rgba(25, 135, 84, 1)",
                        backgroundColor: "rgba(25, 135, 84, 0)",
                        fill: false,
                        tension: 0,
                        cubicInterpolationMode: "monotone",
                        pointRadius: 3,
                        pointHoverRadius: 5,
                        borderWidth: 3
                    },
                    {
                        label: "Výdaje",
                        data: expenseValues,
                        borderColor: "rgba(220, 53, 69, 1)",
                        backgroundColor: "rgba(220, 53, 69, 0)",
                        fill: false,
                        tension: 0,
                        cubicInterpolationMode: "monotone",
                        pointRadius: 3,
                        pointHoverRadius: 5,
                        borderWidth: 3
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                interaction: {
                    mode: "index",
                    intersect: false
                },
                plugins: {
                    legend: {
                        position: "top"
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return context.dataset.label + ": " +
                                    context.raw.toLocaleString("cs-CZ", {
                                        style: "currency",
                                        currency: "CZK"
                                    });
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        suggestedMin: suggestedMin,
                        suggestedMax: suggestedMax,
                        ticks: {
                            callback: function (value) {
                                return value.toLocaleString("cs-CZ") + " Kč";
                            }
                        }
                    }
                }
            }
        });
    }

    const houseHoldMemberTomSelect = new TomSelect("#houseHoldMemberIds", {
        plugins: ["remove_button"],
        maxItems: null,
        closeAfterSelect: false,
        placeholder: "Vyberte členy domácnosti",
        searchField: ["text"],
        render: {
            no_results: function () {
                return '<div class="no-results">Žádné výsledky</div>';
            }
        },
        onChange: function () {
            normalizeTomSelectSelection(houseHoldMemberTomSelect);
            loadStatistics();
        }
    });

    const incomeCategoryTomSelect = new TomSelect("#incomeCategoryIds", {
        plugins: ["remove_button"],
        maxItems: null,
        closeAfterSelect: false,
        placeholder: "Vyberte kategorie příjmů",
        searchField: ["text"],
        render: {
            no_results: function () {
                return '<div class="no-results">Žádné výsledky</div>';
            }
        },
        onChange: function () {
            normalizeTomSelectSelection(incomeCategoryTomSelect);
            loadStatistics();
        }
    });

    const expenseCategoryTomSelect = new TomSelect("#expenseCategoryIds", {
        plugins: ["remove_button"],
        maxItems: null,
        closeAfterSelect: false,
        placeholder: "Vyberte kategorie výdajů",
        searchField: ["text"],
        render: {
            no_results: function () {
                return '<div class="no-results">Žádné výsledky</div>';
            }
        },
        onChange: function () {
            normalizeTomSelectSelection(expenseCategoryTomSelect);
            loadStatistics();
        }
    });

    selectedYearElement.addEventListener("change", loadStatistics);
    selectedMonthElement.addEventListener("change", loadStatistics);

    clearFiltersBtnElement.addEventListener("click", function () {
        suppressStatisticsReload = true;

        setDefaultTomSelectValue(houseHoldMemberTomSelect);
        setDefaultTomSelectValue(incomeCategoryTomSelect);
        setDefaultTomSelectValue(expenseCategoryTomSelect);

        if (window.statisticsJsInitialData) {
            selectedYearElement.value = window.statisticsJsInitialData.selectedYear;
            selectedMonthElement.value = window.statisticsJsInitialData.selectedMonth;
        }

        suppressStatisticsReload = false;
        loadStatistics();
    });

    if (window.statisticsJsInitialData) {
        createStatisticsJsChart(window.statisticsJsInitialData);
    }
});


//Get chart data for Statistics (Single) Index view
document.addEventListener("DOMContentLoaded", function () {
    const statisticsChartElement = document.getElementById("statisticsChart");

    if (!statisticsChartElement || !window.statisticsInitialData) {
        return;
    }

    const incomeChart = window.statisticsInitialData.incomeChart;
    const expenseChart = window.statisticsInitialData.expenseChart;
    const balanceChart = window.statisticsInitialData.balanceChart;

    const labels = Object.keys(balanceChart);

    const incomeValues = labels.map(function (label) {
        return incomeChart[label] ?? 0;
    });

    const expenseValues = labels.map(function (label) {
        return expenseChart[label] ?? 0;
    });

    const balanceValues = labels.map(function (label) {
        return balanceChart[label] ?? 0;
    });

    const allValues = incomeValues.concat(expenseValues, balanceValues);

    const maxValue = Math.max(...allValues, 0);
    const minValue = Math.min(...allValues, 0);

    const suggestedMax = maxValue > 0 ? maxValue * 1.2 : 100;
    const suggestedMin = minValue < 0 ? minValue * 1.2 : 0;

    new Chart(statisticsChartElement, {
        type: "line",
        data: {
            labels: labels,
            datasets: [
                {
                    label: "Zůstatek",
                    data: balanceValues,
                    borderColor: "rgba(108, 117, 125, 0.35)",
                    backgroundColor: "rgba(108, 117, 125, 0.08)",
                    fill: true,
                    tension: 0.35,
                    pointRadius: 1,
                    pointHoverRadius: 3,
                    borderWidth: 1.5
                },
                {
                    label: "Příjmy",
                    data: incomeValues,
                    borderColor: "rgba(25, 135, 84, 1)",
                    backgroundColor: "rgba(25, 135, 84, 0)",
                    fill: false,
                    tension: 0,
                    cubicInterpolationMode: "monotone",
                    pointRadius: 3,
                    pointHoverRadius: 5,
                    borderWidth: 3
                },
                {
                    label: "Výdaje",
                    data: expenseValues,
                    borderColor: "rgba(220, 53, 69, 1)",
                    backgroundColor: "rgba(220, 53, 69, 0)",
                    fill: false,
                    tension: 0,
                    cubicInterpolationMode: "monotone",
                    pointRadius: 3,
                    pointHoverRadius: 5,
                    borderWidth: 3
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            interaction: {
                mode: "index",
                intersect: false
            },
            plugins: {
                legend: {
                    position: "top"
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.dataset.label}: ${context.raw.toLocaleString("cs-CZ", {
                                style: "currency",
                                currency: "CZK"
                            })}`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: false,
                    suggestedMin: suggestedMin,
                    suggestedMax: suggestedMax,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString("cs-CZ") + " Kč";
                        }
                    }
                }
            }
        }
    });
});


// Toggle filter card state and persist it in URL
document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("filterCollapseToggle");

    if (!toggle) {
        return;
    }

    toggle.addEventListener("click", function (e) {
        e.preventDefault();

        const url = new URL(window.location.href);
        const currentValue = url.searchParams.get("filtersCollapsed");
        const isCollapsed = currentValue === "true";
        const newValue = (!isCollapsed).toString();

        url.searchParams.set("filtersCollapsed", newValue);

        window.location.href = url.toString();
    });
});


//Pager View - submit form on pageSize change
document.addEventListener("DOMContentLoaded", function () {
    const pageSizeSelect = document.getElementById("selectPager");

    if (!pageSizeSelect) {
        return;
    }

    pageSizeSelect.addEventListener("change", function () {
        const url = new URL(window.location.href);

        url.searchParams.set("pageSize", this.value);
        url.searchParams.set("page", "1");

        window.location.href = url.toString();
    });
});


//remove value form dropdown- filter element when value is equal to "-- Vyberte --". Visually looks better for end user
document.addEventListener("DOMContentLoaded", function () {
    const operatorSelects = document.querySelectorAll(".filter-operator");

    function syncFilterValue(operatorSelect) {
        const filterName = operatorSelect.dataset.filterName;
        const valueField = document.querySelector('.filter-value[data-filter-name="' + filterName + '"]');

        if (!valueField) {
            return;
        }

        const isNone = operatorSelect.value === "None";

        if (isNone) {
            valueField.value = "";
            valueField.disabled = true;
        } else {
            valueField.disabled = false;
        }
    }

    operatorSelects.forEach(function (operatorSelect) {
        syncFilterValue(operatorSelect);

        operatorSelect.addEventListener("change", function () {
            syncFilterValue(operatorSelect);
        });
    });
});