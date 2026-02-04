let currentUser = null;

document.addEventListener('DOMContentLoaded', async () => {
    currentUser = checkLogin();
    if (currentUser) {
        document.getElementById('reportDate').textContent = 
            new Date().toLocaleDateString('vi-VN', { 
                year: 'numeric', 
                month: 'long', 
                day: 'numeric' 
            });
        await loadReportData();
    }
});

async function loadReportData() {
    await Promise.all([
        loadPersonalInfo(),
        loadHealthMetrics(),
        loadBMIAnalysis(),
        loadAIRecommendations()
    ]);
}

async function loadPersonalInfo() {
    try {
        const response = await fetch(`http://localhost:5000/api/ThongTinCaNhan/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        document.getElementById('personalInfo').innerHTML = `
            <table class="table">
                <tr><th>Họ tên:</th><td>${data.hoTen || '--'}</td></tr>
                <tr><th>Tuổi:</th><td>${data.tuoi || '--'}</td></tr>
                <tr><th>Giới tính:</th><td>${data.gioiTinh || '--'}</td></tr>
                <tr><th>Chiều cao:</th><td>${data.chieuCao || '--'} cm</td></tr>
                <tr><th>Cân nặng:</th><td>${data.canNang || '--'} kg</td></tr>
            </table>
        `;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadHealthMetrics() {
    try {
        const response = await fetch(`http://localhost:5000/api/SucKhoe/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        let html = '<table class="table table-striped"><thead><tr>';
        html += '<th>Ngày</th><th>Cân nặng</th><th>Huyết áp</th><th>Nhịp tim</th><th>Đường huyết</th>';
        html += '</tr></thead><tbody>';

        data.slice(0, 10).forEach(item => {
            html += `<tr>
                <td>${new Date(item.ngayDo).toLocaleDateString('vi-VN')}</td>
                <td>${item.canNang || '--'} kg</td>
                <td>${item.huyetAp || '--'}</td>
                <td>${item.nhipTim || '--'} bpm</td>
                <td>${item.duongHuyet || '--'} mg/dL</td>
            </tr>`;
        });

        html += '</tbody></table>';
        document.getElementById('healthMetrics').innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadBMIAnalysis() {
    try {
        const response = await fetch(`http://localhost:5000/api/BMI/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        if (data.length > 0) {
            const latest = data[0];
            document.getElementById('bmiAnalysis').innerHTML = `
                <h4>BMI hiện tại: ${latest.giaTriBMI.toFixed(1)}</h4>
                <p class="lead">Trạng thái: <strong>${latest.trangThai}</strong></p>
                <p>Ngày tính: ${new Date(latest.ngayTinh).toLocaleDateString('vi-VN')}</p>
            `;
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadAIRecommendations() {
    try {
        const response = await fetch(`http://localhost:5000/api/AI/goiy/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        if (data.length > 0) {
            const latest = data[0];
            document.getElementById('aiRecommendations').innerHTML = `
                <pre style="white-space: pre-wrap;">${latest.noiDungGoiY}</pre>
                <small class="text-muted">Ngày tạo: ${new Date(latest.ngayTao).toLocaleString('vi-VN')}</small>
            `;
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
