let currentUser = null;

document.addEventListener('DOMContentLoaded', () => {
    currentUser = checkLogin();
    if (currentUser) {
        displayWelcome();
        showSection('profile');
    }
});

function displayWelcome() {
    const navbar = document.querySelector('.navbar-brand');
    if (navbar && currentUser) {
        navbar.innerHTML = `
            🏥 Xin chào, ${currentUser.tenDangNhap}! 
            <button class="btn btn-sm btn-light ms-3" onclick="openReport()">
                📄 Xuất báo cáo
            </button>
        `;
    }
}

function openReport() {
    window.open('report.html', '_blank', 'width=800,height=600');
}

async function showSection(section) {
    const content = document.getElementById('content');
    
    switch(section) {
        case 'profile':
            await loadProfile();
            break;
        case 'health':
            await loadHealthMetrics();
            break;
        case 'bmi':
            await loadBMIHistory();
            break;
        case 'habits':
            await loadHabits();
            break;
        case 'menu':
            await loadMenu();
            break;
        case 'warnings':
            await loadWarnings();
            break;
        case 'ai':
            await loadAISuggestions();
            break;
    }
}

async function loadProfile() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <div class="row mb-4">
            <div class="col-12">
                <h3>👤 Thông tin cá nhân</h3>
                <p class="text-muted">Cập nhật thông tin để hệ thống có thể tư vấn chính xác hơn</p>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <form id="profileForm">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Họ tên <span class="text-danger">*</span></label>
                            <input type="text" class="form-control" id="hoTen" required>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Tuổi <span class="text-danger">*</span></label>
                            <input type="number" class="form-control" id="tuoi" min="1" max="120" required>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Giới tính</label>
                            <select class="form-control" id="gioiTinh">
                                <option value="Nam">Nam</option>
                                <option value="Nữ">Nữ</option>
                                <option value="Khác">Khác</option>
                            </select>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Chiều cao (cm) <span class="text-danger">*</span></label>
                            <input type="number" class="form-control" id="chieuCao" min="50" max="250" step="0.1" required>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Cân nặng (kg) <span class="text-danger">*</span></label>
                            <input type="number" class="form-control" id="canNang" min="10" max="300" step="0.1" required>
                        </div>
                    </div>
                    <div class="d-flex gap-2">
                        <button type="submit" class="btn btn-primary">
                            💾 Lưu thông tin
                        </button>
                        <button type="button" class="btn btn-secondary" onclick="showSection('health')">
                            ➡️ Tiếp theo: Nhập chỉ số sức khỏe
                        </button>
                    </div>
                </form>
            </div>
        </div>
    `;

    // Load existing data
    try {
        const response = await fetch(`${API_URL}/ThongTinCaNhan/${currentUser.maNguoiDung}`);
        if (response.ok) {
            const data = await response.json();
            if (data) {
                document.getElementById('hoTen').value = data.hoTen || '';
                document.getElementById('tuoi').value = data.tuoi || '';
                document.getElementById('gioiTinh').value = data.gioiTinh || 'Nam';
                document.getElementById('chieuCao').value = data.chieuCao || '';
                document.getElementById('canNang').value = data.canNang || '';
            }
        }
    } catch (error) {
        console.error('Error:', error);
    }

    document.getElementById('profileForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        await saveProfile();
    });
}

async function saveProfile() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        hoTen: document.getElementById('hoTen').value,
        tuoi: parseInt(document.getElementById('tuoi').value),
        gioiTinh: document.getElementById('gioiTinh').value,
        chieuCao: parseFloat(document.getElementById('chieuCao').value),
        canNang: parseFloat(document.getElementById('canNang').value)
    };

    try {
        // Check if profile exists
        const checkResponse = await fetch(`${API_URL}/ThongTinCaNhan/${currentUser.maNguoiDung}`);
        
        let response;
        if (checkResponse.ok && checkResponse.status !== 204) {
            const existing = await checkResponse.json();
            if (existing && existing.maThongTin) {
                // Update existing
                data.maThongTin = existing.maThongTin;
                response = await fetch(`${API_URL}/ThongTinCaNhan/${existing.maThongTin}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(data)
                });
            }
        } else {
            // Create new
            response = await fetch(`${API_URL}/ThongTinCaNhan`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
        }

        if (response.ok) {
            alert('✅ Đã lưu thông tin cá nhân!');
        } else {
            alert('❌ Lỗi khi lưu thông tin!');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi kết nối!');
    }
}

async function loadHealthMetrics() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>📊 Chỉ số sức khỏe</h3>
        <div class="card mb-3">
            <div class="card-header bg-success text-white">
                <h5>Nhập chỉ số mới</h5>
            </div>
            <div class="card-body">
                <form id="healthForm">
                    <div class="row">
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Cân nặng (kg)</label>
                            <input type="number" step="0.1" class="form-control" id="canNangCS" required>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Huyết áp (VD: 120/80)</label>
                            <input type="text" class="form-control" id="huyetAp" placeholder="120/80">
                        </div>
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Nhịp tim (bpm)</label>
                            <input type="number" class="form-control" id="nhipTim">
                        </div>
                        <div class="col-md-3 mb-3">
                            <label class="form-label">Đường huyết (mg/dL)</label>
                            <input type="number" step="0.1" class="form-control" id="duongHuyet">
                        </div>
                    </div>
                    <button type="submit" class="btn btn-success">💾 Lưu chỉ số</button>
                </form>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h5>Lịch sử chỉ số</h5>
            </div>
            <div class="card-body">
                <div id="healthHistory"></div>
            </div>
        </div>
    `;

    document.getElementById('healthForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        await saveHealthMetrics();
    });

    await loadHealthHistory();
}

async function saveHealthMetrics() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        ngayDo: new Date().toISOString(),
        canNang: parseFloat(document.getElementById('canNangCS').value),
        huyetAp: document.getElementById('huyetAp').value,
        nhipTim: parseInt(document.getElementById('nhipTim').value) || null,
        duongHuyet: parseFloat(document.getElementById('duongHuyet').value) || null
    };

    try {
        const response = await fetch(`${API_URL}/SucKhoe/chiso`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            alert('✅ Đã lưu chỉ số sức khỏe!');
            document.getElementById('healthForm').reset();
            await loadHealthHistory();
        }
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi khi lưu chỉ số!');
    }
}

async function loadHealthHistory() {
    try {
        const response = await fetch(`${API_URL}/SucKhoe/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const historyDiv = document.getElementById('healthHistory');
        if (data.length === 0) {
            historyDiv.innerHTML = '<p class="text-muted">Chưa có dữ liệu</p>';
            return;
        }

        let html = '<table class="table table-striped"><thead><tr>';
        html += '<th>Ngày</th><th>Cân nặng</th><th>Huyết áp</th><th>Nhịp tim</th><th>Đường huyết</th>';
        html += '</tr></thead><tbody>';

        data.forEach(item => {
            html += '<tr>';
            html += `<td>${new Date(item.ngayDo).toLocaleDateString('vi-VN')}</td>`;
            html += `<td>${item.canNang || '-'} kg</td>`;
            html += `<td>${item.huyetAp || '-'}</td>`;
            html += `<td>${item.nhipTim || '-'} bpm</td>`;
            html += `<td>${item.duongHuyet || '-'} mg/dL</td>`;
            html += '</tr>';
        });

        html += '</tbody></table>';
        historyDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadBMIHistory() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>📈 Lịch sử BMI</h3>
        <div class="card mb-3">
            <div class="card-header bg-info text-white">
                <h5>Tính BMI mới</h5>
            </div>
            <div class="card-body">
                <form id="bmiForm">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Chiều cao (cm)</label>
                            <input type="number" step="0.1" class="form-control" id="chieuCaoBMI" required>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Cân nặng (kg)</label>
                            <input type="number" step="0.1" class="form-control" id="canNangBMI" required>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-info">🔢 Tính BMI</button>
                </form>
                <div id="bmiResult" class="mt-3"></div>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h5>Lịch sử BMI</h5>
            </div>
            <div class="card-body">
                <canvas id="bmiChart"></canvas>
                <div id="bmiHistory" class="mt-3"></div>
            </div>
        </div>
    `;

    document.getElementById('bmiForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        await calculateBMI();
    });

    await loadBMIData();
}

async function calculateBMI() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        chieuCao: parseFloat(document.getElementById('chieuCaoBMI').value),
        canNang: parseFloat(document.getElementById('canNangBMI').value)
    };

    try {
        const response = await fetch(`${API_URL}/BMI/tinh`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        
        const resultDiv = document.getElementById('bmiResult');
        let color = 'success';
        if (result.trangThai === 'Béo phì' || result.trangThai === 'Gầy') color = 'danger';
        else if (result.trangThai === 'Thừa cân') color = 'warning';

        resultDiv.innerHTML = `
            <div class="alert alert-${color}">
                <h5>BMI của bạn: ${result.bmi}</h5>
                <p>Trạng thái: <strong>${result.trangThai}</strong></p>
            </div>
        `;

        await loadBMIData();
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi khi tính BMI!');
    }
}

async function loadBMIData() {
    try {
        const response = await fetch(`${API_URL}/BMI/lichsu/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const historyDiv = document.getElementById('bmiHistory');
        if (data.length === 0) {
            historyDiv.innerHTML = '<p class="text-muted">Chưa có dữ liệu BMI</p>';
            return;
        }

        let html = '<table class="table table-striped"><thead><tr>';
        html += '<th>Ngày</th><th>BMI</th><th>Trạng thái</th>';
        html += '</tr></thead><tbody>';

        data.forEach(item => {
            html += '<tr>';
            html += `<td>${new Date(item.ngayTinh).toLocaleDateString('vi-VN')}</td>`;
            html += `<td>${item.giaTriBMI}</td>`;
            html += `<td><span class="badge bg-info">${item.trangThai}</span></td>`;
            html += '</tr>';
        });

        html += '</tbody></table>';
        historyDiv.innerHTML = html;

        // Vẽ biểu đồ
        drawBMIChart(data);
    } catch (error) {
        console.error('Error:', error);
    }
}

function drawBMIChart(data) {
    const ctx = document.getElementById('bmiChart');
    if (!ctx) return;

    const labels = data.reverse().map(item => new Date(item.ngayTinh).toLocaleDateString('vi-VN'));
    const values = data.map(item => item.giaTriBMI);

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'BMI',
                data: values,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ BMI theo thời gian'
                }
            }
        }
    });
}

async function loadMenu() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>🍎 Gợi ý thực đơn</h3>
        <button class="btn btn-primary mb-3" onclick="getSuggestions()">🤖 Nhận gợi ý từ AI</button>
        <div id="suggestions"></div>

        <div class="card mt-3">
            <div class="card-header">
                <h5>Danh sách món ăn</h5>
            </div>
            <div class="card-body">
                <div id="foodList"></div>
            </div>
        </div>
    `;

    await loadFoodList();
}

async function getSuggestions() {
    try {
        const response = await fetch(`${API_URL}/MonAn/goiy/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const suggestionsDiv = document.getElementById('suggestions');
        let html = '<div class="alert alert-success"><h5>🤖 Gợi ý từ AI:</h5><ul>';
        
        data.forEach(item => {
            html += `<li>${item}</li>`;
        });
        
        html += '</ul></div>';
        suggestionsDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadFoodList() {
    try {
        const response = await fetch(`${API_URL}/MonAn`);
        const data = await response.json();
        
        const foodDiv = document.getElementById('foodList');
        let html = '<table class="table table-striped"><thead><tr>';
        html += '<th>Món ăn</th><th>Calo</th><th>Đạm</th><th>Béo</th><th>Bot</th>';
        html += '</tr></thead><tbody>';

        data.forEach(item => {
            html += '<tr>';
            html += `<td>${item.tenMonAn}</td>`;
            html += `<td>${item.calo}</td>`;
            html += `<td>${item.chatDam}g</td>`;
            html += `<td>${item.chatBeo}g</td>`;
            html += `<td>${item.chatBot}g</td>`;
            html += '</tr>';
        });

        html += '</tbody></table>';
        foodDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadWarnings() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>⚠️ Cảnh báo sức khỏe</h3>
        <div class="card">
            <div class="card-body">
                <div id="warningsList"></div>
            </div>
        </div>
    `;

    try {
        const response = await fetch(`${API_URL}/CanhBao/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const warningsDiv = document.getElementById('warningsList');
        if (data.length === 0) {
            warningsDiv.innerHTML = '<p class="text-success">✅ Không có cảnh báo</p>';
            return;
        }

        let html = '';
        data.forEach(item => {
            const alertType = item.trangThai === 'Chưa xem' ? 'warning' : 'secondary';
            html += `
                <div class="alert alert-${alertType}">
                    <strong>${new Date(item.ngayCanhBao).toLocaleDateString('vi-VN')}</strong>
                    <p>${item.noiDung}</p>
                    <small class="text-muted">Trạng thái: ${item.trangThai}</small>
                </div>
            `;
        });

        warningsDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadAISuggestions() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>🤖 Tư vấn AI</h3>
        <button class="btn btn-primary mb-3" onclick="analyzeHealth()">🔍 Phân tích sức khỏe</button>
        <div id="aiAnalysis"></div>

        <div class="card mt-3">
            <div class="card-header">
                <h5>Lịch sử tư vấn</h5>
            </div>
            <div class="card-body">
                <div id="aiHistory"></div>
            </div>
        </div>
    `;

    await loadAIHistory();
}

async function analyzeHealth() {
    try {
        const response = await fetch(`${API_URL}/AI/phantich/${currentUser.maNguoiDung}`, {
            method: 'POST'
        });
        
        const data = await response.json();
        
        const analysisDiv = document.getElementById('aiAnalysis');
        analysisDiv.innerHTML = `
            <div class="card bg-light">
                <div class="card-body">
                    <h5>📊 Kết quả phân tích</h5>
                    <p><strong>BMI:</strong> ${data.bmi} - ${data.trangThaiBMI}</p>
                    <p><strong>Nguy cơ tiểu đường:</strong> ${data.nguyCoTieuDuong}%</p>
                    <p><strong>Nguy cơ cao huyết áp:</strong> ${data.nguyCoCaoHuyetAp}%</p>
                    <hr>
                    <pre>${data.goiY}</pre>
                </div>
            </div>
        `;

        await loadAIHistory();
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Chưa đủ dữ liệu để phân tích!');
    }
}

async function loadAIHistory() {
    try {
        const response = await fetch(`${API_URL}/AI/goiy/${currentUser.maNguoiDung}`);
        const data = await response.json();
        
        const historyDiv = document.getElementById('aiHistory');
        if (data.length === 0) {
            historyDiv.innerHTML = '<p class="text-muted">Chưa có lịch sử tư vấn</p>';
            return;
        }

        let html = '';
        data.forEach(item => {
            html += `
                <div class="card mb-2">
                    <div class="card-body">
                        <small class="text-muted">${new Date(item.ngayTao).toLocaleString('vi-VN')}</small>
                        <pre>${item.noiDungGoiY}</pre>
                    </div>
                </div>
            `;
        });

        historyDiv.innerHTML = html;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function loadHabits() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>🏃 Thói quen sinh hoạt</h3>
        <div class="card">
            <div class="card-header bg-warning text-dark">
                <h5>Cập nhật thói quen hàng ngày</h5>
            </div>
            <div class="card-body">
                <form id="habitsForm">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Số giờ ngủ trung bình/ngày</label>
                            <input type="number" step="0.5" class="form-control" id="soGioNgu" min="0" max="24">
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Thời gian tập luyện (phút/ngày)</label>
                            <input type="number" class="form-control" id="thoiGianTapLuyen" min="0">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="hutThuoc">
                                <label class="form-check-label" for="hutThuoc">
                                    🚬 Hút thuốc
                                </label>
                            </div>
                        </div>
                        <div class="col-md-6 mb-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="uongRuouBia">
                                <label class="form-check-label" for="uongRuouBia">
                                    🍺 Uống rượu/bia
                                </label>
                            </div>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-warning">💾 Lưu thói quen</button>
                </form>
            </div>
        </div>

        <div class="card mt-3">
            <div class="card-header">
                <h5>📊 Đánh giá thói quen</h5>
            </div>
            <div class="card-body">
                <div id="habitsAnalysis"></div>
            </div>
        </div>
    `;

    // Load existing data
    try {
        const response = await fetch(`${API_URL}/ThoiQuen/${currentUser.maNguoiDung}`);
        if (response.ok) {
            const data = await response.json();
            document.getElementById('soGioNgu').value = data.soGioNgu || '';
            document.getElementById('thoiGianTapLuyen').value = data.thoiGianTapLuyen || '';
            document.getElementById('hutThuoc').checked = data.hutThuoc || false;
            document.getElementById('uongRuouBia').checked = data.uongRuouBia || false;
        }
    } catch (error) {
        console.error('Error:', error);
    }

    document.getElementById('habitsForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        await saveHabits();
    });
}

async function saveHabits() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        soGioNgu: parseFloat(document.getElementById('soGioNgu').value) || null,
        thoiGianTapLuyen: parseInt(document.getElementById('thoiGianTapLuyen').value) || null,
        hutThuoc: document.getElementById('hutThuoc').checked,
        uongRuouBia: document.getElementById('uongRuouBia').checked
    };

    try {
        const response = await fetch(`${API_URL}/ThoiQuen`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            alert('✅ Đã lưu thói quen sinh hoạt!');
            analyzeHabits(data);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi khi lưu thói quen!');
    }
}

function analyzeHabits(data) {
    const analysisDiv = document.getElementById('habitsAnalysis');
    let html = '<div class="alert alert-info"><h5>💡 Đánh giá từ AI:</h5><ul>';

    if (data.soGioNgu < 7) {
        html += '<li class="text-danger">⚠️ Bạn ngủ ít hơn 7 giờ/ngày. Hãy cố gắng ngủ đủ giấc!</li>';
    } else if (data.soGioNgu >= 7 && data.soGioNgu <= 9) {
        html += '<li class="text-success">✅ Thời gian ngủ của bạn rất tốt!</li>';
    }

    if (!data.thoiGianTapLuyen || data.thoiGianTapLuyen < 30) {
        html += '<li class="text-warning">⚠️ Nên tập thể dục ít nhất 30 phút/ngày</li>';
    } else {
        html += '<li class="text-success">✅ Thói quen tập luyện rất tốt!</li>';
    }

    if (data.hutThuoc) {
        html += '<li class="text-danger">⚠️ Hút thuốc có hại cho sức khỏe. Hãy cố gắng bỏ thuốc!</li>';
    }

    if (data.uongRuouBia) {
        html += '<li class="text-warning">⚠️ Hạn chế rượu bia để bảo vệ gan và sức khỏe tổng thể</li>';
    }

    html += '</ul></div>';
    analysisDiv.innerHTML = html;
}
