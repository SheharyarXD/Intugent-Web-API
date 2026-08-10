let aiModelChartInstance = null;

window.renderAiModelChart = (canvasId, yy, yyp, yth, xTitle, yTitle) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (aiModelChartInstance) {
        aiModelChartInstance.destroy();
        aiModelChartInstance = null;
    }

    const points = yy.map((x, i) => ({ x: x, y: yyp[i] }));
    const line = yth.map(v => ({ x: v, y: v }));

    aiModelChartInstance = new Chart(ctx, {
        type: 'scatter',
        data: {
            datasets: [
                {
                    label: 'Predicted vs Actual',
                    data: points,
                    backgroundColor: 'rgba(54, 162, 235, 0.7)',
                    pointRadius: 3
                },
                {
                    type: 'line',
                    label: '45° Reference',
                    data: line,
                    borderColor: 'rgba(255, 99, 132, 0.8)',
                    borderWidth: 2,
                    pointRadius: 0
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                x: { type: 'linear', position: 'bottom', title: { display: true, text: xTitle } },
                y: { title: { display: true, text: yTitle } }
            },
            plugins: {
                legend: { display: true },
                zoom: {
                    zoom: { wheel: { enabled: true }, pinch: { enabled: true }, drag: { enabled: true }, mode: 'xy' },
                    pan: { enabled: true, mode: 'xy', threshold: 10, speed: 10 }
                }
            }
        }
    });

    canvas.ondblclick = () => {
        if (aiModelChartInstance) aiModelChartInstance.resetZoom();
    };
};
