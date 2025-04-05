// Load Bar Chart - Số lượng Startup theo tháng
async function loadStartupChart() {
    try {
        const response = await fetch('/Admins/Dashboard/GetStartupData');
        if (!response.ok) throw new Error('Lỗi khi tải dữ liệu Startup theo tháng.');

        const data = await response.json();

        //console.log(data);

        const labels = data.map(d => 'Tháng ' + d.month);
        const counts = data.map(d => d.count);

        const ctx = document.getElementById('startupChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Số lượng Startup',
                    data: counts,
                    backgroundColor: 'rgba(54, 162, 235, 0.7)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1,
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        ticks: { font: { size: 10 } }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1, font: { size: 10 } }
                    }
                },
                plugins: {
                    legend: { labels: { font: { size: 12 } } }
                },
                maintainAspectRatio: false
            }
        });
    } catch (error) {
        console.error(error);
    }
}

// Load Pie Chart - Số lượng Startup theo lĩnh vực
async function loadLinhVucChart() {
    try {
        const response = await fetch('/Admins/Dashboard/GetStartupCountByLinhVuc');
        if (!response.ok) throw new Error('Lỗi khi tải dữ liệu lĩnh vực.');

        const data = await response.json();
        const labels = data.map(item => item.linhVuc);

        //console.log(data);

        const counts = data.map(item => item.startupCount);

        const backgroundColors = ['#ff6384', '#36a2eb', '#ffcd56', '#4bc0c0', '#9966ff', '#ff9f40', '#c9cbcf'];

        const ctx = document.getElementById('startupLinhVucChart').getContext('2d');
        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Số lượng Startup theo lĩnh vực',
                    data: counts,
                    backgroundColor: backgroundColors.slice(0, labels.length)
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'right' },
                },
                maintainAspectRatio: false
            }
        });
    } catch (error) {
        console.error(error);
    }
}

// Line Chart - Lượt truy cập theo ngày
async function loadAccessLogChart() {
    try {
        const response = await fetch('/Admins/Dashboard/GetAccessLogsByDay');
        if (!response.ok) throw new Error('Lỗi khi tải dữ liệu lĩnh vực.');
        const data = await response.json();

        //console.log(data);

        const chartData = data.map(x => ({ x: new Date(x.date), y: x.count }));
        const ctx = document.getElementById('accessLogChart').getContext('2d');

        const gradient = ctx.createLinearGradient(0, 0, 0, 300);
        gradient.addColorStop(0, 'rgba(0, 123, 255, 0.4)');
        gradient.addColorStop(1, 'rgba(0, 123, 255, 0.05)');

        new Chart(ctx, {
            type: 'line',
            data: {
                datasets: [{
                    data: chartData,
                    borderColor: '#007bff',
                    backgroundColor: gradient,
                    fill: true,
                    tension: 0.4,
                    pointRadius: 3,
                    pointBackgroundColor: '#007bff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        type: 'time',
                        time: {
                            unit: 'day',
                            displayFormats: { day: 'dd/MM' },
                            tooltipFormat: 'dd/MM/yyyy'
                        },
                        title: { display: false },
                        ticks: { font: { size: 12 } }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: { font: { size: 12 }, stepSize: 1 }
                    }
                },
                plugins: {
                    tooltip: { mode: 'index', intersect: false },
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error(error);
    }
}

// Stacked Bar Chart - Số lượng hợp đồng theo trạng thái
async function loadContractStatusChart() {
    try {
        const response = await fetch('/Admins/Dashboard/GetContractStatistics');
        const data = await response.json();
        console.log(data); // Kiểm tra dữ liệu trả về

        const labels = [];
        const daGuiData = [];
        const daDuyetData = [];
        const biTuChoiData = [];

        // Xử lý dữ liệu
        data.forEach(item => {
            const monthLabel = `Tháng ${item.month} - ${item.year}`;
            if (!labels.includes(monthLabel)) {
                labels.push(monthLabel);
            }

            if (item.status === 0) {
                daGuiData.push(item.count);
            }
            else if (item.status === 1) {
                daDuyetData.push(item.count);
            }
            else if (item.status === 2) {
                biTuChoiData.push(item.count);
            }
        });

        const ctx = document.getElementById('contractStatusChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Denied',
                        data: biTuChoiData,
                        
                        backgroundColor: 'rgba(255, 99, 132, 0.7)',
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Updating',
                        data: daGuiData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Signed',
                        data: daDuyetData,
                        backgroundColor: 'rgba(75, 192, 192, 0.7)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        bottom: 20
                    }
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            font: { size: 12 },
                            boxWidth: 12
                        }
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            font: { size: 10 }
                        },
                        stacked: true
                    },
                    y: {
                        beginAtZero: true,
                        stacked: true,
                        ticks: {
                            stepSize: 1,
                            font: { size: 10 }
                        }
                    }
                }
            }
        });

    } catch (error) {
        console.error(error);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    loadStartupChart();
    loadLinhVucChart();
    loadAccessLogChart();
    loadContractStatusChart();
});