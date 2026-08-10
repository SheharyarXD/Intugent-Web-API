let spcChartInstance = null;

window.renderSpcChart = (canvasId, xa, ya, xAvg, yAvg, yucl, ylcl, bottomTitle, leftTitle) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (spcChartInstance) {
        spcChartInstance.destroy();
        spcChartInstance = null;
    }

    const toPoints = (xs, ys) => xs.map((x, i) => ({ x: x, y: ys[i] }));
    const datasets = [
        {
            label: 'Data',
            data: toPoints(xa, ya),
            borderColor: 'rgba(54, 162, 235, 1)',
            backgroundColor: 'rgba(54, 162, 235, 0.6)',
            borderWidth: 2,
            pointRadius: 0
        }
    ];

    if (xAvg && xAvg.length === 2) {
        datasets.push({ label: 'Average', data: toPoints(xAvg, yAvg), borderColor: 'rgba(40, 167, 69, 1)', borderWidth: 2, pointRadius: 0, borderDash: [6, 3] });
        datasets.push({ label: 'UCL', data: toPoints(xAvg, yucl), borderColor: 'rgba(220, 53, 69, 1)', borderWidth: 2, pointRadius: 0, borderDash: [6, 3] });
        datasets.push({ label: 'LCL', data: toPoints(xAvg, ylcl), borderColor: 'rgba(220, 53, 69, 1)', borderWidth: 2, pointRadius: 0, borderDash: [6, 3] });
    }

    spcChartInstance = new Chart(ctx, {
        type: 'line',
        data: { datasets: datasets },
        options: {
            responsive: true,
            scales: {
                x: { type: 'linear', position: 'bottom', title: { display: true, text: bottomTitle } },
                y: { title: { display: true, text: leftTitle } }
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
        if (spcChartInstance) spcChartInstance.resetZoom();
    };
};
