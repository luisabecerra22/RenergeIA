window.imprimirPagina = function () { window.print(); };

window.downloadFile = function (fileName, base64Content, mimeType) {
    const blob = base64ToBlob(base64Content, mimeType);
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

function base64ToBlob(base64, mimeType) {
    const bytes = atob(base64);
    const buffer = new ArrayBuffer(bytes.length);
    const view = new Uint8Array(buffer);
    for (let i = 0; i < bytes.length; i++) {
        view[i] = bytes.charCodeAt(i);
    }
    return new Blob([buffer], { type: mimeType });
}

// --- Charts ---
const _charts = {};

window.renderLineChart = function (canvasId, labels, data1, data2, label1, label2) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: label1,
                    data: data1,
                    borderColor: '#198754',
                    backgroundColor: 'rgba(25,135,84,0.15)',
                    tension: 0.3,
                    fill: true,
                    pointRadius: 4
                },
                {
                    label: label2,
                    data: data2,
                    borderColor: '#0d6efd',
                    backgroundColor: 'rgba(13,110,253,0.08)',
                    borderDash: [6, 3],
                    tension: 0.3,
                    fill: false,
                    pointRadius: 3
                }
            ]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: 'top' } },
            scales: {
                y: { min: 0, max: 100, ticks: { callback: v => v + '%' } }
            }
        }
    });
};

window.renderBarChart = function (canvasId, labels, data1, data2, label1, label2) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: label1,
                    data: data1,
                    backgroundColor: 'rgba(25,135,84,0.75)',
                    borderColor: '#198754',
                    borderWidth: 1
                },
                {
                    label: label2,
                    data: data2,
                    backgroundColor: 'rgba(13,110,253,0.45)',
                    borderColor: '#0d6efd',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: 'top' } },
            scales: {
                y: { min: 0, max: 100, ticks: { callback: v => v + '%' } }
            }
        }
    });
};

window.renderDoughnutChart = function (canvasId, labels, data, colors) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderColor: '#fff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            cutout: '62%',
            plugins: {
                legend: { position: 'right', labels: { boxWidth: 14, padding: 12 } },
                tooltip: {
                    callbacks: {
                        label: ctx => ' ' + ctx.label + ': ' + ctx.parsed + ' act.'
                    }
                }
            }
        }
    });
};

window.renderHorizontalBarChart = function (canvasId, labels, data, label) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: data.map(v => v < -15 ? 'rgba(220,53,69,0.8)' : 'rgba(255,193,7,0.85)'),
                borderColor:     data.map(v => v < -15 ? '#dc3545' : '#ffc107'),
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            plugins: { legend: { display: false } },
            scales: {
                x: { ticks: { callback: v => v + '%' } }
            }
        }
    });
};

window.renderHistograma = function (canvasId, labels, totales, label) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: totales,
                backgroundColor: 'rgba(13,110,253,0.65)',
                borderColor: '#0d6efd',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                tooltip: { callbacks: { label: c => ' ' + c.parsed.y + ' ' + label.toLowerCase() } }
            },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 } }
            }
        }
    });
};

window.renderHistogramaComparativo = function (canvasId, labels, planificados, reales) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Planificado',
                    data: planificados,
                    backgroundColor: 'rgba(24,57,99,0.75)',
                    borderColor: '#183963',
                    borderWidth: 1,
                    borderRadius: 3
                },
                {
                    label: 'Real',
                    data: reales,
                    backgroundColor: 'rgba(106,191,75,0.75)',
                    borderColor: '#6ABF4B',
                    borderWidth: 1,
                    borderRadius: 3
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                tooltip: { callbacks: { label: c => ' ' + c.dataset.label + ': ' + c.parsed.y } }
            },
            scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
        }
    });
};

window.renderDesviacion = function (canvasId, labels, desviaciones) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const colors = desviaciones.map(v =>
        v > 0  ? 'rgba(220,53,69,0.75)'  :
        v < 0  ? 'rgba(106,191,75,0.75)' :
                 'rgba(108,117,125,0.5)'
    );
    const borders = desviaciones.map(v =>
        v > 0  ? '#dc3545' :
        v < 0  ? '#6ABF4B' :
                 '#6c757d'
    );
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: '% Desviación',
                data: desviaciones,
                backgroundColor: colors,
                borderColor: borders,
                borderWidth: 1,
                borderRadius: 3
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: { callbacks: { label: c => ' ' + c.parsed.y.toFixed(1) + '%' } }
            },
            scales: {
                y: {
                    ticks: { callback: v => v + '%' },
                    grid: { color: ctx2 => ctx2.tick.value === 0 ? '#aaa' : 'rgba(0,0,0,0.05)' }
                }
            }
        }
    });
};

window.renderPlanTrabajoChart = function (canvasId, planificadas, ejecutadas, pendientes, vencidas) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    const total = planificadas > 0 ? planificadas : 1;
    const valores = [planificadas, ejecutadas, pendientes, vencidas];
    const pcts    = [100, Math.round(ejecutadas * 100 / total), Math.round(pendientes * 100 / total), Math.round(vencidas * 100 / total)];
    const coloresFondo  = ['rgba(24,57,99,0.85)', 'rgba(106,191,75,0.85)', 'rgba(255,193,7,0.85)', 'rgba(220,53,69,0.85)'];
    const coloresBorde  = ['#183963', '#6ABF4B', '#ffc107', '#dc3545'];
    const coloresTexto  = ['#183963', '#6ABF4B', '#856404', '#dc3545'];

    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Planificadas', 'Ejecutadas', 'Pendientes', 'Vencidas'],
            datasets: [{
                data: valores,
                backgroundColor: coloresFondo,
                borderColor: coloresBorde,
                borderWidth: 2,
                borderRadius: 8,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function(c) {
                            return ' ' + c.parsed.y + ' actividades — ' + pcts[c.dataIndex] + '%';
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max: Math.max(...valores, 1) * 1.35,
                    ticks: { stepSize: 1, color: '#6c757d' },
                    grid: { color: 'rgba(0,0,0,0.05)' }
                },
                x: { ticks: { color: '#495057', font: { weight: '600' } }, grid: { display: false } }
            },
            animation: {
                onComplete: function () {
                    const chart = _charts[canvasId];
                    if (!chart) return;
                    const ctx2 = chart.ctx;
                    chart.data.datasets.forEach(function(dataset, dsi) {
                        chart.getDatasetMeta(dsi).data.forEach(function(bar, idx) {
                            const val = dataset.data[idx];
                            const pct = pcts[idx];
                            const color = coloresTexto[idx];
                            // número grande
                            ctx2.save();
                            ctx2.font = 'bold 16px Arial';
                            ctx2.fillStyle = color;
                            ctx2.textAlign = 'center';
                            ctx2.textBaseline = 'bottom';
                            ctx2.fillText(val, bar.x, bar.y - 18);
                            // porcentaje pequeño
                            ctx2.font = '11px Arial';
                            ctx2.fillStyle = '#6c757d';
                            ctx2.fillText(pct + '%', bar.x, bar.y - 4);
                            ctx2.restore();
                        });
                    });
                }
            }
        }
    });
};

function destroyChart(canvasId) {
    if (_charts[canvasId]) {
        _charts[canvasId].destroy();
        delete _charts[canvasId];
    }
}

// ── Inspecciones Dashboard ─────────────────────────────────────────────────────

window.renderHallazgosPorArea = function (canvasId, labels, valores) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const maxV = Math.max(...valores, 1);
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: 'Hallazgos',
                data: valores,
                backgroundColor: valores.map(v => {
                    const p = v / maxV;
                    return p > 0.66 ? 'rgba(220,53,69,0.85)' : p > 0.33 ? 'rgba(255,193,7,0.85)' : 'rgba(106,191,75,0.85)';
                }),
                borderRadius: 4, borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y', responsive: true, maintainAspectRatio: false,
            plugins: { legend: { display: false }, tooltip: { callbacks: { label: c => ' ' + c.parsed.x + ' hallazgos' } } },
            scales: { x: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' }, grid: { color: 'rgba(0,0,0,0.05)' } }, y: { ticks: { font: { size: 11 }, color: '#495057' }, grid: { display: false } } }
        }
    });
};

window.renderActosVsCondiciones = function (canvasId, labels, actos, condiciones) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [
                { label: 'Actos Inseguros', data: actos, backgroundColor: 'rgba(220,53,69,0.8)', borderColor: '#dc3545', borderWidth: 1, borderRadius: 4, stack: 'stk' },
                { label: 'Condiciones Inseguras', data: condiciones, backgroundColor: 'rgba(255,193,7,0.8)', borderColor: '#ffc107', borderWidth: 1, stack: 'stk' }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: { legend: { position: 'top', labels: { boxWidth: 12, font: { size: 11 } } }, tooltip: { mode: 'index' } },
            scales: { x: { stacked: true, grid: { display: false } }, y: { stacked: true, beginAtZero: true, ticks: { stepSize: 1 } } }
        }
    });
};

window.renderEstadoHallazgos = function (canvasId, abiertos, revision, implementacion, cerrados, vencidos) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const total = abiertos + revision + implementacion + cerrados;
    const centerPlugin = {
        id: 'center_' + canvasId,
        afterDraw(chart) {
            const { ctx: c, chartArea: { left, top, width, height } } = chart;
            c.save();
            const cx = left + width / 2, cy = top + height / 2;
            c.font = 'bold 22px Montserrat, sans-serif';
            c.fillStyle = '#183963'; c.textAlign = 'center'; c.textBaseline = 'middle';
            c.fillText(total, cx, cy - 9);
            c.font = '10px Montserrat, sans-serif';
            c.fillStyle = '#6c757d';
            c.fillText('Hallazgos', cx, cy + 10);
            c.restore();
        }
    };
    _charts[canvasId] = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Abiertos', 'En Revisión', 'En Implementación', 'Cerrados', 'Vencidos'],
            datasets: [{ data: [abiertos, revision, implementacion, cerrados, vencidos], backgroundColor: ['#dc3545','#ffc107','#0dcaf0','#6ABF4B','#6c757d'], borderColor: '#fff', borderWidth: 2 }]
        },
        options: {
            responsive: true, maintainAspectRatio: false, cutout: '67%',
            plugins: {
                legend: { position: 'right', labels: { boxWidth: 11, padding: 9, font: { size: 11 } } },
                tooltip: { callbacks: { label: c => ' ' + c.label + ': ' + c.parsed } }
            }
        },
        plugins: [centerPlugin]
    });
};

window.renderTendenciaInsp = function (canvasId, meses, inspecciones, hallazgos) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: meses,
            datasets: [
                { label: 'Inspecciones', data: inspecciones, borderColor: '#183963', backgroundColor: 'rgba(24,57,99,0.08)', tension: 0.4, fill: true, pointBackgroundColor: '#183963', pointRadius: 5, pointHoverRadius: 7 },
                { label: 'Hallazgos', data: hallazgos, borderColor: '#dc3545', backgroundColor: 'rgba(220,53,69,0.04)', tension: 0.4, fill: false, pointBackgroundColor: '#dc3545', pointRadius: 5, pointHoverRadius: 7, borderDash: [5, 3] }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: { legend: { position: 'top', labels: { boxWidth: 12, font: { size: 11 } } } },
            scales: { y: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' } }, x: { ticks: { color: '#6c757d' }, grid: { display: false } } }
        }
    });
};

window.renderInspPorMes = function (canvasId, meses, valores) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: meses,
            datasets: [{
                label: 'Inspecciones',
                data: valores,
                backgroundColor: valores.map((_, i) => i === valores.length - 1 ? 'rgba(106,191,75,0.85)' : 'rgba(24,57,99,0.75)'),
                borderColor:     valores.map((_, i) => i === valores.length - 1 ? '#6ABF4B' : '#183963'),
                borderWidth: 1, borderRadius: 6, borderSkipped: false
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: { legend: { display: false }, tooltip: { callbacks: { label: c => ' ' + c.parsed.y + ' inspecciones' } } },
            scales: { y: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' } }, x: { ticks: { color: '#495057', font: { weight: '600' } }, grid: { display: false } } }
        }
    });
};

window.renderParetoHallazgos = function (canvasId, labels, valores, acumulados) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [
                { type: 'bar',  label: 'Hallazgos',   data: valores,    backgroundColor: 'rgba(24,57,99,0.8)', borderColor: '#183963', borderWidth: 1, borderRadius: 4, yAxisID: 'y' },
                { type: 'line', label: '% Acumulado', data: acumulados, borderColor: '#dc3545', backgroundColor: 'transparent', pointBackgroundColor: '#dc3545', pointRadius: 4, tension: 0.1, yAxisID: 'y2' }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: { legend: { position: 'top', labels: { boxWidth: 12, font: { size: 11 } } } },
            scales: {
                y:  { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' }, position: 'left' },
                y2: { min: 0, max: 100, ticks: { callback: v => v + '%', color: '#dc3545' }, position: 'right', grid: { drawOnChartArea: false } }
            }
        }
    });
};

window.renderRankingInsp = function (canvasId, labels, valores, titulo) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const maxV = Math.max(...valores, 1);
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: titulo, data: valores,
                backgroundColor: valores.map(v => { const p = v / maxV; return p > 0.66 ? 'rgba(220,53,69,0.8)' : p > 0.33 ? 'rgba(255,193,7,0.8)' : 'rgba(24,57,99,0.75)'; }),
                borderRadius: 4, borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y', responsive: true, maintainAspectRatio: false,
            plugins: { legend: { display: false }, tooltip: { callbacks: { label: c => ' ' + c.parsed.x + ' hallazgos' } } },
            scales: { x: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' } }, y: { ticks: { font: { size: 11 }, color: '#495057' }, grid: { display: false } } }
        }
    });
};

// --- Capacitaciones Charts ---

window.renderCapMes = function (canvasId, meses, planificadas, ejecutadas, porcentajes) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: meses,
            datasets: [
                {
                    label: 'Planificadas', data: planificadas, type: 'bar',
                    backgroundColor: 'rgba(24,57,99,0.18)', borderColor: '#183963',
                    borderWidth: 1.5, borderRadius: 4, order: 2
                },
                {
                    label: 'Ejecutadas', data: ejecutadas, type: 'bar',
                    backgroundColor: 'rgba(106,191,75,0.75)', borderColor: '#6ABF4B',
                    borderWidth: 0, borderRadius: 4, order: 2
                },
                {
                    label: '% Cumplimiento', data: porcentajes, type: 'line',
                    borderColor: '#ffc107', backgroundColor: 'rgba(255,193,7,0.08)',
                    borderWidth: 2.5, pointRadius: 4, pointBackgroundColor: '#ffc107',
                    tension: 0.35, fill: true, yAxisID: 'y2', order: 1
                }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            interaction: { mode: 'index', intersect: false },
            plugins: {
                legend: { position: 'top', labels: { font: { family: 'Montserrat', size: 11 }, boxWidth: 10, padding: 12 } },
                tooltip: { callbacks: { label: c => c.datasetIndex === 2 ? ` ${c.parsed.y}%` : ` ${c.parsed.y} cap.` } }
            },
            scales: {
                x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#6c757d' } },
                y: { beginAtZero: true, position: 'left', ticks: { stepSize: 1, color: '#6c757d', font: { size: 10 } }, grid: { color: 'rgba(0,0,0,.05)' } },
                y2: { beginAtZero: true, max: 100, position: 'right', ticks: { callback: v => v + '%', color: '#ffc107', font: { size: 10 } }, grid: { display: false } }
            }
        }
    });
};

window.renderCapHH = function (canvasId, meses, horas) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const maxIdx = horas.indexOf(Math.max(...horas));
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: meses,
            datasets: [{
                label: 'Horas Hombre',
                data: horas,
                backgroundColor: horas.map((_, i) => i === maxIdx ? 'rgba(106,191,75,0.8)' : 'rgba(24,57,99,0.65)'),
                borderRadius: 4, borderSkipped: false
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: { callbacks: { label: c => ` ${c.parsed.y.toFixed(0)} HH` } }
            },
            scales: {
                x: { grid: { display: false }, ticks: { font: { size: 10 }, color: '#6c757d' } },
                y: { beginAtZero: true, ticks: { color: '#6c757d', font: { size: 10 } }, grid: { color: 'rgba(0,0,0,.05)' } }
            }
        }
    });
};

window.renderCapArea = function (canvasId, areas, valores) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: areas,
            datasets: [{
                label: '% Cumplimiento',
                data: valores,
                backgroundColor: valores.map(v => v >= 90 ? 'rgba(106,191,75,0.75)' : v >= 70 ? 'rgba(255,193,7,0.75)' : 'rgba(220,53,69,0.75)'),
                borderRadius: 4, borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y', responsive: true, maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: { callbacks: { label: c => ` ${c.parsed.x.toFixed(0)}%` } }
            },
            scales: {
                x: { beginAtZero: true, max: 100, ticks: { callback: v => v + '%', color: '#6c757d', font: { size: 10 } }, grid: { color: 'rgba(0,0,0,.05)' } },
                y: { ticks: { font: { size: 11 }, color: '#495057' }, grid: { display: false } }
            }
        }
    });
};

window.renderCapTemas = function (canvasId, temas, valores) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    const maxV = Math.max(...valores, 1);
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: temas,
            datasets: [{
                label: 'Capacitaciones',
                data: valores,
                backgroundColor: valores.map(v => v / maxV >= 0.8 ? 'rgba(24,57,99,0.8)' : v / maxV >= 0.5 ? 'rgba(24,57,99,0.55)' : 'rgba(24,57,99,0.35)'),
                borderRadius: 4, borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y', responsive: true, maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: { callbacks: { label: c => ` ${c.parsed.x} cap.` } }
            },
            scales: {
                x: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d', font: { size: 10 } }, grid: { color: 'rgba(0,0,0,.05)' } },
                y: { ticks: { font: { size: 11 }, color: '#495057' }, grid: { display: false } }
            }
        }
    });
};

// --- Leaflet Map Interop ---
window.leafletMap = {
    _map: null,
    _marker: null,

    init: function (elementId, lat, lng, dotNetRef) {
        // Destroy previous instance if exists (handles Blazor navigation back)
        if (this._map) {
            this._map.remove();
            this._map = null;
            this._marker = null;
        }

        const el = document.getElementById(elementId);
        if (!el) return;

        const defaultLat  = (lat !== null && lat !== undefined) ? lat  : 4.6;
        const defaultLng  = (lng !== null && lng !== undefined) ? lng  : -74.1;
        const defaultZoom = (lat !== null && lat !== undefined) ? 13   : 5;

        this._map = L.map(elementId).setView([defaultLat, defaultLng], defaultZoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 19
        }).addTo(this._map);

        if (lat !== null && lat !== undefined && lng !== null && lng !== undefined) {
            this._marker = L.marker([lat, lng]).addTo(this._map)
                .bindPopup('Ubicación del proyecto').openPopup();
        }

        this._map.on('click', (e) => {
            const { lat: clat, lng: clng } = e.latlng;
            this.setMarker(clat, clng);
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnMapClick', clat, clng);
            }
        });
    },

    setMarker: function (lat, lng) {
        if (!this._map) return;
        if (this._marker) {
            this._marker.setLatLng([lat, lng]);
        } else {
            this._marker = L.marker([lat, lng]).addTo(this._map);
        }
        this._marker.bindPopup(`Lat: ${lat.toFixed(6)}, Lng: ${lng.toFixed(6)}`).openPopup();
        this._map.setView([lat, lng], Math.max(this._map.getZoom(), 13));
    },

    destroy: function () {
        if (this._map) {
            this._map.remove();
            this._map = null;
            this._marker = null;
        }
    }
};

// --- Column Resize for WBS Table ---
window.wbsResize = {
    init: function (tableId) {
        const table = document.getElementById(tableId);
        if (!table) return;

        const ths = Array.from(table.querySelectorAll('thead tr:first-child th'));
        const storageKey = 'wbs-col-widths';

        // Remove existing handles before re-adding (Blazor re-renders call this again)
        table.querySelectorAll('.wbs-resize-handle').forEach(h => h.remove());

        // Load saved widths and apply them
        let saved = {};
        try { saved = JSON.parse(localStorage.getItem(storageKey) || '{}'); } catch (e) {}
        ths.forEach((th, i) => {
            if (saved[i] !== undefined) th.style.width = saved[i] + 'px';
        });

        // Attach resize handle to each header cell
        ths.forEach((th, index) => {
            th.style.position = 'relative';

            const handle = document.createElement('div');
            handle.className = 'wbs-resize-handle';
            handle.style.cssText =
                'position:absolute;right:0;top:0;height:100%;width:6px;' +
                'cursor:col-resize;user-select:none;z-index:10;';

            handle.addEventListener('mousedown', function (e) {
                e.preventDefault();
                e.stopPropagation();

                const startX     = e.clientX;
                const startWidth = th.offsetWidth;
                const minWidth   = index === 0 ? 300 : 60;

                document.body.style.cursor     = 'col-resize';
                document.body.style.userSelect = 'none';

                function onMove(mv) {
                    const newW = Math.max(minWidth, startWidth + mv.clientX - startX);
                    th.style.width = newW + 'px';
                }

                function onUp() {
                    document.body.style.cursor     = '';
                    document.body.style.userSelect = '';
                    document.removeEventListener('mousemove', onMove);
                    document.removeEventListener('mouseup',   onUp);

                    // Persist all column widths to localStorage
                    const widths = {};
                    ths.forEach((t, i) => { widths[i] = t.offsetWidth; });
                    try { localStorage.setItem(storageKey, JSON.stringify(widths)); } catch (ex) {}
                }

                document.addEventListener('mousemove', onMove);
                document.addEventListener('mouseup',   onUp);
            });

            th.appendChild(handle);
        });
    }
};

// ═══════════════════════════════════════════════════════════════
// Gráficos ISO 9001 — Dashboard Ejecutivo de Auditoría
// ═══════════════════════════════════════════════════════════════
window._iso9001Charts = {};

window.renderISO9001Charts = function (globalPct, clausulaLabels, clausulaData, estadosData, seguimientoData) {
    const C_AZUL  = '#102963';
    const C_VERDE = '#6ABF48';
    const C_AMBER = '#F5B301';
    const C_ROJO  = '#E53935';
    const C_GRIS  = '#D1D5DB';
    const C_MUTED = '#9CA3AF';
    const FONT    = 'Montserrat, sans-serif';

    Object.values(window._iso9001Charts).forEach(c => { if (c) c.destroy(); });
    window._iso9001Charts = {};

    // ── 1. Barras horizontales gruesas — Cumplimiento por cláusula ──────────
    const ctxC = document.getElementById('chartClausula');
    if (ctxC) {
        const colores = clausulaData.map(v => v >= 85 ? C_VERDE : v >= 70 ? C_AMBER : v > 0 ? C_ROJO : C_GRIS);
        window._iso9001Charts.clausula = new Chart(ctxC, {
            type: 'bar',
            data: {
                labels: clausulaLabels,
                datasets: [{
                    label: '% Cumplimiento',
                    data: clausulaData,
                    backgroundColor: colores,
                    borderRadius: 6,
                    maxBarThickness: 28
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                layout: { padding: { right: 44 } },
                scales: {
                    x: {
                        min: 0, max: 100,
                        ticks: { callback: v => v + '%', font: { size: 10 } },
                        grid: { color: '#f3f4f6' }
                    },
                    y: {
                        ticks: { font: { family: FONT, size: 11, weight: '600' }, color: '#374151' },
                        grid: { display: false }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => '  ' + ctx.parsed.x.toFixed(1) + '%' } }
                }
            },
            plugins: [{
                id: 'clausulaLabels',
                afterDraw(chart) {
                    const { ctx } = chart;
                    ctx.save();
                    chart.data.datasets.forEach((ds, di) => {
                        chart.getDatasetMeta(di).data.forEach((bar, i) => {
                            const val = ds.data[i];
                            if (val == null) return;
                            const pos = bar.tooltipPosition();
                            ctx.font = 'bold 10px ' + FONT;
                            ctx.fillStyle = '#374151';
                            ctx.textAlign = 'left';
                            ctx.textBaseline = 'middle';
                            ctx.fillText(val.toFixed(0) + '%', pos.x + 8, pos.y);
                        });
                    });
                    ctx.restore();
                }
            }]
        });
    }

    // ── 2. Donut ejecutivo — Distribución de estados ─────────────────────────
    const ctxE = document.getElementById('chartEstados');
    if (ctxE && estadosData && estadosData.length >= 4) {
        const total = estadosData.reduce((a, b) => a + b, 0);
        if (total > 0) {
            window._iso9001Charts.estados = new Chart(ctxE, {
                type: 'doughnut',
                data: {
                    labels: ['Cumple', 'En Proceso', 'No Cumple', 'Sin Evaluar'],
                    datasets: [{
                        data: estadosData,
                        backgroundColor: [C_VERDE, C_AMBER, C_ROJO, C_MUTED],
                        borderWidth: 3,
                        borderColor: '#fff',
                        hoverBorderWidth: 4
                    }]
                },
                options: {
                    cutout: '68%',
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                font: { family: FONT, size: 11, weight: '600' },
                                boxWidth: 12, boxHeight: 12, padding: 14,
                                generateLabels: chart => {
                                    const ds = chart.data.datasets[0];
                                    return chart.data.labels.map((lbl, i) => ({
                                        text: lbl + '  ' + ds.data[i] + '  (' + (ds.data[i] / total * 100).toFixed(0) + '%)',
                                        fillStyle: ds.backgroundColor[i],
                                        strokeStyle: 'transparent',
                                        lineWidth: 0,
                                        hidden: false,
                                        index: i
                                    }));
                                }
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: ctx => '  ' + ctx.label + ': ' + ctx.parsed +
                                             '  (' + (ctx.parsed / total * 100).toFixed(1) + '%)'
                            }
                        }
                    }
                },
                plugins: [{
                    id: 'donutCenter',
                    afterDraw(chart) {
                        const { ctx, chartArea: { width, height, left, top } } = chart;
                        ctx.save();
                        const cx = left + width / 2, cy = top + height / 2;
                        ctx.font = 'bold 30px ' + FONT;
                        ctx.fillStyle = C_AZUL;
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.fillText(total, cx, cy - 12);
                        ctx.font = '500 11px ' + FONT;
                        ctx.fillStyle = C_MUTED;
                        ctx.fillText('requisitos', cx, cy + 14);
                        ctx.restore();
                    }
                }]
            });
        }
    }

    // ── 3. Barras verticales — Seguimiento de acciones ───────────────────────
    const ctxS = document.getElementById('chartSeguimiento');
    if (ctxS && seguimientoData && seguimientoData.length >= 3) {
        window._iso9001Charts.seguimiento = new Chart(ctxS, {
            type: 'bar',
            data: {
                labels: ['Ejecutado', 'En Proceso', 'Pendiente'],
                datasets: [{
                    data: seguimientoData,
                    backgroundColor: [C_VERDE, C_AMBER, C_ROJO],
                    borderRadius: 10,
                    maxBarThickness: 90
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: { padding: { top: 28 } },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { font: { family: FONT, size: 12, weight: '700' }, color: '#374151' }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: { font: { family: FONT, size: 10 }, color: '#9CA3AF' },
                        grid: { color: '#f3f4f6' }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => '  ' + ctx.parsed.y + ' acciones' } }
                }
            },
            plugins: [{
                id: 'topValues',
                afterDraw(chart) {
                    const { ctx } = chart;
                    ctx.save();
                    chart.data.datasets.forEach((ds, di) => {
                        chart.getDatasetMeta(di).data.forEach((bar, i) => {
                            const val = ds.data[i];
                            if (!val) return;
                            const pos = bar.tooltipPosition();
                            ctx.font = 'bold 13px ' + FONT;
                            ctx.fillStyle = '#374151';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';
                            ctx.fillText(val, pos.x, bar.y - 4);
                        });
                    });
                    ctx.restore();
                }
            }]
        });
    }
};

// Barra simple de un solo dataset (Dashboard SST, etc.)
window.renderBarChartSingle = function (canvasId, labels, data, color) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    _charts[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: color + 'bb',
                borderColor: color,
                borderWidth: 1,
                borderRadius: 5,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1, color: '#6c757d' }, grid: { color: 'rgba(0,0,0,.05)' } },
                x: { ticks: { color: '#495057' }, grid: { display: false } }
            }
        }
    });
};

// Descarga de archivos (reutilizable)
window.downloadFile = window.downloadFile || function (fileName, contentType, base64) {
    const link = document.createElement('a');
    link.href = 'data:' + contentType + ';base64,' + base64;
    link.download = fileName;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// ═══════════════════════════════════════════════════════════════
// Módulo de Costos — Dashboard Charts
// ═══════════════════════════════════════════════════════════════
const _costoCharts = {};

window.renderCostosCharts = function (proyectoId, d) {
    const pfx = proyectoId;
    const fmt = v => '$' + Math.round(v).toLocaleString('es-CO');

    function mk(id, config) {
        const el = document.getElementById(id);
        if (!el) return;
        if (_costoCharts[id]) { _costoCharts[id].destroy(); }
        _costoCharts[id] = new Chart(el, config);
    }

    // 1. Barras agrupadas: Presupuesto vs Ejecutado vs Compromisos por categoría
    mk('chart-comparativo-' + pfx, {
        type: 'bar',
        data: {
            labels: d.categoriaLabels,
            datasets: [
                { label: 'Presupuesto',   data: d.presupCat,      backgroundColor: 'rgba(24,57,99,0.75)',  borderColor: '#183963', borderWidth: 1, borderRadius: 4 },
                { label: 'Ejecutado',     data: d.ejecutadoCat,   backgroundColor: 'rgba(106,191,75,0.8)', borderColor: '#6ABF4B', borderWidth: 1, borderRadius: 4 },
                { label: 'Compromisos',   data: d.compromisosCat, backgroundColor: 'rgba(253,126,20,0.7)', borderColor: '#fd7e14', borderWidth: 1, borderRadius: 4 }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: {
                legend: { position: 'top', labels: { boxWidth: 10, font: { size: 11 } } },
                tooltip: { callbacks: { label: c => ' ' + c.dataset.label + ': ' + fmt(c.parsed.y) } }
            },
            scales: {
                y: { beginAtZero: true, ticks: { callback: v => '$' + (v / 1e6).toFixed(0) + 'M', color: '#6c757d' }, grid: { color: 'rgba(0,0,0,.05)' } },
                x: { ticks: { color: '#495057', font: { weight: '600', size: 10 } }, grid: { display: false } }
            }
        }
    });

    // 2. Doughnut: distribución por tipo de costo
    if (d.tipoLabels && d.tipoLabels.length > 0) {
        const coloresTipo = ['#183963','#6ABF4B','#fd7e14','#ffc107','#dc3545','#0d6efd'];
        mk('chart-tipos-' + pfx, {
            type: 'doughnut',
            data: {
                labels: d.tipoLabels,
                datasets: [{
                    data: d.tipoValores,
                    backgroundColor: d.tipoLabels.map((_, i) => coloresTipo[i % coloresTipo.length]),
                    borderColor: '#fff', borderWidth: 2
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false, cutout: '60%',
                plugins: {
                    legend: { position: 'right', labels: { boxWidth: 12, font: { size: 11 } } },
                    tooltip: { callbacks: { label: c => ' ' + c.label + ': ' + fmt(c.parsed) } }
                }
            }
        });
    }

    // 3. Línea: evolución mensual de costos reales
    if (d.mesesLabels && d.mesesLabels.length > 0) {
        mk('chart-mensual-' + pfx, {
            type: 'line',
            data: {
                labels: d.mesesLabels,
                datasets: [{
                    label: 'Costo Real Mensual',
                    data: d.mesesValores,
                    borderColor: '#183963',
                    backgroundColor: 'rgba(24,57,99,0.08)',
                    tension: 0.4, fill: true,
                    pointBackgroundColor: '#6ABF4B', pointRadius: 5
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: c => ' ' + fmt(c.parsed.y) } }
                },
                scales: {
                    y: { beginAtZero: true, ticks: { callback: v => '$' + (v / 1e6).toFixed(1) + 'M', color: '#6c757d' }, grid: { color: 'rgba(0,0,0,.05)' } },
                    x: { ticks: { color: '#6c757d', font: { size: 10 } }, grid: { display: false } }
                }
            }
        });
    }

    // 4. Doughnut: estado de compromisos
    if (d.estadoLabels && d.estadoLabels.length > 0) {
        const coloresEstado = { 'Pendiente': '#ffc107', 'Aprobado': '#0d6efd', 'En Proceso': '#fd7e14', 'Pagado': '#6ABF4B', 'Vencido': '#dc3545', 'Cancelado': '#6c757d' };
        mk('chart-compromisos-' + pfx, {
            type: 'doughnut',
            data: {
                labels: d.estadoLabels,
                datasets: [{
                    data: d.estadoConteo,
                    backgroundColor: d.estadoLabels.map(l => coloresEstado[l] || '#9E9E9E'),
                    borderColor: '#fff', borderWidth: 2
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false, cutout: '60%',
                plugins: {
                    legend: { position: 'right', labels: { boxWidth: 12, font: { size: 11 } } },
                    tooltip: { callbacks: { label: c => ' ' + c.label + ': ' + c.parsed + ' compromisos' } }
                }
            }
        });
    }
};

window.destroyCostosCharts = function (proyectoId) {
    const pfx = proyectoId;
    ['chart-comparativo-','chart-tipos-','chart-mensual-','chart-compromisos-'].forEach(pre => {
        const k = pre + pfx;
        if (_costoCharts[k]) { _costoCharts[k].destroy(); delete _costoCharts[k]; }
    });
};
