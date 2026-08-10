let scatterChartInstances = {};

window.renderScatterChart = (canvasId, xx, yy, xTitle, yTitle) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (scatterChartInstances[canvasId]) {
        scatterChartInstances[canvasId].destroy();
        delete scatterChartInstances[canvasId];
    }

    const points = xx.map((x, i) => ({ x: x, y: yy[i] }));

    scatterChartInstances[canvasId] = new Chart(ctx, {
        type: 'scatter',
        data: {
            datasets: [{
                label: yTitle,
                data: points,
                backgroundColor: 'rgba(54, 162, 235, 0.7)',
                pointRadius: 3
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: { type: 'linear', position: 'bottom', title: { display: true, text: xTitle } },
                y: { title: { display: true, text: yTitle } }
            },
            plugins: {
                legend: { display: false },
                zoom: {
                    zoom: { wheel: { enabled: true }, pinch: { enabled: true }, drag: { enabled: true }, mode: 'xy' },
                    pan: { enabled: true, mode: 'xy', threshold: 10, speed: 10 }
                }
            }
        }
    });

    canvas.ondblclick = () => {
        if (scatterChartInstances[canvasId]) scatterChartInstances[canvasId].resetZoom();
    };
};
