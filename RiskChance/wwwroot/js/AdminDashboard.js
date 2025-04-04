// Load Bar Chart - Số lượng Startup theo tháng
async function loadStartupChart() {
    try {
        const response = await fetch('/Admins/Dashboard/GetStartupData');
        if (!response.ok) throw new Error('Lỗi khi tải dữ liệu Startup theo tháng.');

        const data = await response.json();

        console.log(data);

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
                }
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

        console.log(data);

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
                }
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

        console.log(data);

        const chartData = data.map(x => ({ x: new Date(x.date), y: x.count }));
        const ctx = document.getElementById('accessLogChart').getContext('2d');

        const gradient = ctx.createLinearGradient(0, 0, 0, 300);
        gradient.addColorStop(0, 'rgba(0, 123, 255, 0.4)');
        gradient.addColorStop(1, 'rgba(0, 123, 255, 0.05)');

        new Chart(ctx, {
            type: 'line',
            data: {
                datasets: [{
                    label: 'Lượt truy cập Founder & Investor',
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
                        labels: {
                            font: { size: 14 },
                            boxWidth: 20,
                            color: '#333'
                        }
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
        //console.log(data); // Kiểm tra dữ liệu trả về

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

            daGuiData.push(item.status === 0 ? item.count : 0);
            daDuyetData.push(item.status === 1 ? item.count : 0);
            biTuChoiData.push(item.status === 2 ? item.count : 0);
        });

        const ctx = document.getElementById('contractStatusChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Đã Gửi',
                        data: daGuiData,
                        backgroundColor: 'rgba(255, 99, 132, 0.7)',
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Đã Duyệt',
                        data: daDuyetData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Bị Từ Chối',
                        data: biTuChoiData,
                        backgroundColor: 'rgba(75, 192, 192, 0.7)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: { ticks: { font: { size: 10 } } },
                    y: { beginAtZero: true, stacked: true, ticks: { stepSize: 1, font: { size: 10 } } }
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