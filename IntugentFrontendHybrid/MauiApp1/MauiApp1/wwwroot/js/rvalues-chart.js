let rvaluesChartInstance = null;

window.renderRValuesChart = (canvasId, datasets, xTitle, yTitle) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (rvaluesChartInstance) {
        rvaluesChartInstance.destroy();
        rvaluesChartInstance = null;
    }

    rvaluesChartInstance = new Chart(ctx, {
        type: 'line',
        data: { datasets: datasets },
        options: {
            responsive: true,
            scales: {
                x: {
                    type: 'linear',
                    position: 'bottom',
                    title: { display: true, text: xTitle }
                },
                y: {
                    title: { display: true, text: yTitle }
                }
            },
            plugins: {
                legend: { display: true },
                zoom: {
                    zoom: {
                        wheel: { enabled: true },
                        pinch: { enabled: true },
                        drag: { enabled: true },
                        mode: 'xy'
                    },
                    pan: {
                        enabled: true,
                        mode: 'xy',
                        threshold: 10,
                        speed: 10
                    }
                }
            }
        }
    });

    canvas.ondblclick = () => {
        if (rvaluesChartInstance) rvaluesChartInstance.resetZoom();
    };
};
