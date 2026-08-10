let jetMixingChartInstance = null;

window.renderJetMixingChart = (canvasId, xa, ya, xb, yb) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (jetMixingChartInstance) {
        jetMixingChartInstance.destroy();
        jetMixingChartInstance = null;
    }

    const toPoints = (xs, ys) => xs.map((x, i) => ({ x: x, y: ys[i] }));

    jetMixingChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            datasets: [
                {
                    label: 'Jet A',
                    data: toPoints(xa, ya),
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
                    borderWidth: 2,
                    pointRadius: 0
                },
                {
                    label: 'Jet B',
                    data: toPoints(xb, yb),
                    borderColor: 'rgba(54, 162, 235, 1)',
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderWidth: 2,
                    pointRadius: 0
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                x: { type: 'linear', position: 'bottom' },
                y: {}
            },
            plugins: {
                legend: { display: true }
            }
        }
    });
};
