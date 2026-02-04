let currentUser = null;
let bmiChartInstance = null;

document.addEventListener('DOMContentLoaded', async () => {
    currentUser = checkLogin();
    if (currentUser) {
        await loadOverviewData();
    }
});

async function loadOverviewData() {
    await Promise.all([
        loadCurrentStats(),
        loadBMIChart(),
        loadRecentWarnings(),
        loadSavedGoals()
    ]);
}

async function loadCurrentStats() {
    try {
        // Lấy chỉ số mới nhất
        const response = await fetch(`http://localhost:5000/api/SucKhoe/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        if (data.length > 0) {
            const latest = data[0];
            document.getElementById('currentWeight').textContent = latest.canNang || '--';
            document.getElementById('currentBP').textContent = latest.huyetAp || '--';
            document.getElementById('currentGlucose').textContent = latest.duongHuyet || '--';
        }

        // Lấy BMI mới nhất
        const bmiResponse = await fetch(`http://localhost:5000/api/BMI/lichsu/${currentUser.maNguoiDung}`);
        const bmiData = await bmiResponse.json();
        
        if (bmiData.length > 0) {
            const latestBMI = bmiData[0];
            document.getElementById('currentBMI').textContent = latestBMI.giaTriBMI.toFixed(1);
            document.getElementById('bmiStatus').textContent = latestBMI.trangThai;
        }
    } catch (error) {
        console.error('Error loading stats:', error);
    }
}

async function loadBMIChart() {
    try {
        const response = await fetch(`http://localhost:5000/api/BMI/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        if (data.length === 0) {
            document.querySelector('#bmiChart').parentElement.innerHTML = 
                '<p class="text-muted text-center">Chưa có dữ liệu BMI</p>';
            return;
        }

        const labels = data.reverse().map(item => 
            new Date(item.ngayTinh).toLocaleDateString('vi-VN')
        );
        const values = data.map(item => item.giaTriBMI);

        const ctx = document.getElementById('bmiChart');
        
        if (bmiChartInstance) {
            bmiChartInstance.destroy();
        }

        bmiChartInstance = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'BMI',
                    data: values,
                    borderColor: 'rgb(13, 110, 253)',
                    backgroundColor: 'rgba(13, 110, 253, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: true
                    },
                    title: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        min: Math.min(...values) - 2,
                        max: Math.max(...values) + 2
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error loading BMI chart:', error);
    }
}

async function loadRecentWarnings() {
    try {
        const response = await fetch(`http://localhost:5000/api/CanhBao/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const warningsDiv = document.getElementById('recentWarnings');
        
        if (data.length === 0) {
            warningsDiv.innerHTML = '<p class="text-success">✅ Không có cảnh báo</p>';
            return;
        }

        let html = '';
        data.slice(0, 5).forEach(warning => {
            const alertType = warning.trangThai === 'Chưa xem' ? 'warning' : 'secondary';
            html += `
                <div class="alert alert-${alertType} alert-sm mb-2">
                    <small><strong>${new Date(warning.ngayCanhBao).toLocaleDateString('vi-VN')}</strong></small>
                    <p class="mb-0 small">${warning.noiDung}</p>
                </div>
            `;
        });
        
        warningsDiv.innerHTML = html;
    } catch (error) {
        console.error('Error loading warnings:', error);
    }
}

async function generateMenu() {
    try {
        const response = await fetch(`http://localhost:5000/api/MonAn/goiy/${currentUser.maNguoiDung}`);
        const suggestions = await response.json();
        
        const resultDiv = document.getElementById('menuResult');
        let html = '<div class="alert alert-success"><h6>🍽️ Thực đơn gợi ý:</h6><ul class="mb-0">';
        
        suggestions.forEach(item => {
            html += `<li>${item}</li>`;
        });
        
        html += '</ul></div>';
        resultDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
        alert('Lỗi khi tạo thực đơn!');
    }
}

async function loadSavedGoals() {
    const saved = localStorage.getItem(`goals_${currentUser.maNguoiDung}`);
    if (saved) {
        const goals = JSON.parse(saved);
        document.getElementById('targetWeight').value = goals.weight || '';
        document.getElementById('targetBMI').value = goals.bmi || '';
    }
}

function saveGoals() {
    const goals = {
        weight: document.getElementById('targetWeight').value,
        bmi: document.getElementById('targetBMI').value
    };
    
    localStorage.setItem(`goals_${currentUser.maNguoiDung}`, JSON.stringify(goals));
    alert('✅ Đã lưu mục tiêu!');
}
