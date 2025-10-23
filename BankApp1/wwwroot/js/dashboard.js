window.renderBalanceChart = (percentage) => {
    const canvas = document.getElementById("balanceChart");
    if (!canvas) return;

    if (window.balanceChartInstance) {
        window.balanceChartInstance.destroy();
    }

    const ctx = canvas.getContext("2d");
    window.balanceChartInstance = new Chart(ctx, {
        type: "doughnut",
        data: {
            datasets: [
                {
                    data: [percentage, 100 - percentage],
                    backgroundColor: ["#007bff", "#e9ecef"],
                    borderWidth: 0,
                },
            ],
        },
        options: {
            cutout: "80%",
            plugins: {
                legend: { display: false },
                tooltip: { enabled: false },
            },
        },
    });
};
window.renderBalanceChart = (accounts) => {
    const canvas = document.getElementById("balanceChart");
    if (!canvas || !accounts || accounts.length === 0) return;

    // Rensa tidigare diagram
    if (window.balanceChartInstance) {
        window.balanceChartInstance.destroy();
    }

    const ctx = canvas.getContext("2d");

    const labels = accounts.map(a => a.name);
    const data = accounts.map(a => a.balance);

    const colors = [
        "#007bff", // blå
        "#28a745", // grön
        "#ffc107", // gul
        "#dc3545", // röd
        "#17a2b8", // turkos
        "#6f42c1", // lila
        "#fd7e14"  // orange
    ];

    window.balanceChartInstance = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors.slice(0, accounts.length),
                borderWidth: 0
            }]
        },
        options: {
            cutout: "75%",
            plugins: {
                legend: {
                    position: "bottom",
                    labels: {
                        boxWidth: 12,
                        padding: 10
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: $${context.formattedValue}`;
                        }
                    }
                }
            }
        }
    });
};
