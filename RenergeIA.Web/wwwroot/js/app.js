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
