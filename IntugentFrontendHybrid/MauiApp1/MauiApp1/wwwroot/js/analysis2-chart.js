let analysis2ChartInstances = {};

window.renderAnalysis2Chart = (canvasId, xs, ys, xTitle, yTitle, color) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (analysis2ChartInstances[canvasId]) {
        analysis2ChartInstances[canvasId].destroy();
        delete analysis2ChartInstances[canvasId];
    }

    const points = xs.map((x, i) => ({ x: x, y: ys[i] }));

    analysis2ChartInstances[canvasId] = new Chart(ctx, {
        type: 'scatter',
        data: {
            datasets: [{
                data: points,
                backgroundColor: color,
                pointRadius: 2
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
        if (analysis2ChartInstances[canvasId]) analysis2ChartInstances[canvasId].resetZoom();
    };
};
