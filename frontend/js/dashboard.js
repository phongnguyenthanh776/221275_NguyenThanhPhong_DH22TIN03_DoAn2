let currentUser = null;
let trendChartInstance = null;
let bmiChartInstance = null;

document.addEventListener('DOMContentLoaded', function() {
    currentUser = checkLogin();
    if (currentUser) {
        displayWelcome();
        const overviewItem = document.querySelector('.list-group-item[onclick*="overview"]');
        showSection('overview', overviewItem ? { target: overviewItem } : null);
    }
});

function displayWelcome() {
    const navbar = document.querySelector('.navbar-brand');
    if (navbar && currentUser) {
        navbar.textContent = '🏥 Xin chào, ' + currentUser.tenDangNhap + '!';
    }
}

function showSection(section, evt) {
    document.querySelectorAll('.list-group-item').forEach(function(item) {
        item.classList.remove('active');
    });

    const target = (evt && evt.target) ? evt.target : (window.event && window.event.target ? window.event.target : null);
    if (target) {
        target.classList.add('active');
    }

    const content = document.getElementById('content');
    content.innerHTML = '<div class="text-center mt-5"><div class="spinner-border text-primary"></div><p class="mt-3">Đang tải...</p></div>';

    setTimeout(function() {
        switch(section) {
            case 'overview':
                loadOverviewSection();
                break;
            case 'profile':
                loadProfile();
                break;
            case 'health':
                loadHealthMetrics();
                break;
            case 'bmi':
                loadBMIHistory();
                break;
            case 'habits':
                loadHabits();
                break;
            case 'menu':
                loadMenu();
                break;
            case 'warnings':
                loadWarnings();
                break;
            case 'ai':
                loadAISuggestions();
                break;
            default:
                loadOverviewSection();
        }
    }, 100);
}

function loadOverviewSection() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <div class="row mb-4">
            <div class="col-12">
                <h3>📊 Tổng quan sức khỏe</h3>
                <p class="text-muted">Xem nhanh các chỉ số gần nhất của bạn</p>
            </div>
        </div>
        <div class="row" id="statsCards">
            <div class="col-12 text-center">
                <div class="spinner-border text-primary"></div>
                <p class="mt-2">Đang tải dữ liệu...</p>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col-md-8">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <h5>📈 Xu hướng 7 ngày</h5>
                    </div>
                    <div class="card-body" id="trendChart">
                        <div class="text-center"><div class="spinner-border text-primary"></div></div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-header bg-warning">
                        <h5>⚠️ Cảnh báo</h5>
                    </div>
                    <div class="card-body" id="warningsSummary">
                        <div class="text-center"><div class="spinner-border text-warning"></div></div>
                    </div>
                </div>
            </div>
        </div>
    `;

    loadOverviewData();
}

function loadOverviewData() {
    console.log('Current user:', currentUser);
    console.log('User ID:', currentUser.maNguoiDung);
    
    const userId = currentUser.maNguoiDung;
    const apiUrl = 'http://localhost:5000/api/SucKhoe/tong-quan/' + userId;
    
    console.log('Calling API:', apiUrl);
    
    fetch(apiUrl)
        .then(function(response) {
            console.log('Response status:', response.status);
            if (!response.ok) {
                throw new Error('HTTP error! status: ' + response.status);
            }
            return response.json();
        })
        .then(function(data) {
            console.log('Overview data received:', data);
            displayStatsCards(data);
            displayTrendChart(data.lichSu7Ngay);
            displayWarningsSummary(data.soCanhBaoChuaXem);
        })
        .catch(function(error) {
            console.error('Error loading overview:', error);
            document.getElementById('statsCards').innerHTML = `
                <div class="col-12">
                    <div class="alert alert-warning">
                        <h5>⚠️ Chưa có dữ liệu</h5>
                        <p>Vui lòng cập nhật thông tin cá nhân và nhập chỉ số sức khỏe.</p>
                        <p class="text-danger">Lỗi: ` + error.message + `</p>
                        <button class="btn btn-primary" onclick="showSection('profile')">Cập nhật ngay</button>
                    </div>
                </div>
            `;
            document.getElementById('trendChart').innerHTML = '<p class="text-muted text-center">Chưa có dữ liệu</p>';
            document.getElementById('warningsSummary').innerHTML = '<p class="text-muted text-center">Chưa có cảnh báo</p>';
        });
}

function displayStatsCards(data) {
    const statsDiv = document.getElementById('statsCards');
    const chiSo = data.chiSoGanNhat;
    const bmi = data.bmiGanNhat;
    
    let html = '';
    
    // BMI Card
    const bmiColor = getBMIColor(bmi ? bmi.giaTriBMI : null);
    html += '<div class="col-md-3 mb-3"><div class="card ' + bmiColor + ' text-white"><div class="card-body text-center">';
    html += '<h6>BMI</h6>';
    html += '<h2>' + (bmi ? bmi.giaTriBMI.toFixed(1) : '--') + '</h2>';
    html += '<p class="mb-0">' + (bmi ? bmi.trangThai : 'Chưa có') + '</p>';
    if (bmi) html += '<small>' + new Date(bmi.ngayTinh).toLocaleDateString('vi-VN') + '</small>';
    html += '</div></div></div>';
    
    // Weight Card
    html += '<div class="col-md-3 mb-3"><div class="card bg-success text-white"><div class="card-body text-center">';
    html += '<h6>Cân nặng</h6>';
    html += '<h2>' + (chiSo && chiSo.canNang ? chiSo.canNang.toFixed(1) : '--') + '</h2>';
    html += '<p class="mb-0">kg</p>';
    html += '<small>' + (data.xuHuongCanNang || '') + '</small>';
    html += '</div></div></div>';
    
    // Blood Pressure Card
    const bpColor = getBloodPressureColor(chiSo ? chiSo.huyetAp : null);
    html += '<div class="col-md-3 mb-3"><div class="card ' + bpColor + ' text-white"><div class="card-body text-center">';
    html += '<h6>Huyết áp</h6>';
    html += '<h2>' + (chiSo && chiSo.huyetAp ? chiSo.huyetAp : '--') + '</h2>';
    html += '<p class="mb-0">mmHg</p>';
    if (chiSo) html += '<small>' + new Date(chiSo.ngayDo).toLocaleDateString('vi-VN') + '</small>';
    html += '</div></div></div>';
    
    // Glucose Card
    const glucoseColor = getGlucoseColor(chiSo ? chiSo.duongHuyet : null);
    html += '<div class="col-md-3 mb-3"><div class="card ' + glucoseColor + ' text-white"><div class="card-body text-center">';
    html += '<h6>Đường huyết</h6>';
    html += '<h2>' + (chiSo && chiSo.duongHuyet ? chiSo.duongHuyet.toFixed(0) : '--') + '</h2>';
    html += '<p class="mb-0">mg/dL</p>';
    if (chiSo) html += '<small>' + new Date(chiSo.ngayDo).toLocaleDateString('vi-VN') + '</small>';
    html += '</div></div></div>';
    
    html += '<div class="col-12"><div class="alert alert-info">';
    html += '<strong>📅 Cập nhật lần cuối:</strong> ' + data.ngayCapNhatCuoi;
    html += '</div></div>';
    
    statsDiv.innerHTML = html;
}

function getBMIColor(bmi) {
    if (!bmi) return 'bg-secondary';
    if (bmi < 18.5) return 'bg-warning';
    if (bmi < 25) return 'bg-success';
    if (bmi < 30) return 'bg-warning';
    return 'bg-danger';
}

function getBloodPressureColor(bp) {
    if (!bp) return 'bg-secondary';
    const parts = bp.split('/');
    if (parts.length !== 2) return 'bg-secondary';
    const systolic = parseInt(parts[0]);
    const diastolic = parseInt(parts[1]);
    if (systolic < 120 && diastolic < 80) return 'bg-success';
    if (systolic < 140 && diastolic < 90) return 'bg-warning';
    return 'bg-danger';
}

function getGlucoseColor(glucose) {
    if (!glucose) return 'bg-secondary';
    if (glucose < 100) return 'bg-success';
    if (glucose < 126) return 'bg-warning';
    return 'bg-danger';
}

function displayTrendChart(lichSu7Ngay) {
    const chartDiv = document.getElementById('trendChart');
    if (!lichSu7Ngay || lichSu7Ngay.length === 0) {
        chartDiv.innerHTML = '<p class="text-muted text-center">Chưa có dữ liệu. Hãy nhập chỉ số sức khỏe để xem biểu đồ.</p>';
        return;
    }
    chartDiv.innerHTML = '<canvas id="trendChartCanvas"></canvas>';
    const ctx = document.getElementById('trendChartCanvas');

    if (trendChartInstance) {
        trendChartInstance.destroy();
    }

    trendChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: lichSu7Ngay.reverse().map(function(item) {
                return new Date(item.ngayDo).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' });
            }),
            datasets: [{
                label: 'Cân nặng (kg)',
                data: lichSu7Ngay.map(function(item) { return item.canNang || null; }),
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.4
            }]
        },
        options: { responsive: true }
    });
}

function displayWarningsSummary(soCanhBao) {
    const warningsDiv = document.getElementById('warningsSummary');
    if (soCanhBao === 0) {
        warningsDiv.innerHTML = '<div class="text-center"><h2 class="text-success">✅</h2><p class="mb-0">Không có cảnh báo</p></div>';
    } else {
        warningsDiv.innerHTML = '<div class="text-center"><h2 class="text-danger">' + soCanhBao + '</h2><p class="mb-3">Cảnh báo chưa xem</p><button class="btn btn-warning btn-sm" onclick="showSection(\'warnings\')">Xem chi tiết</button></div>';
    }
}

function loadProfile() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>👤 Thông tin cá nhân</h3>
        <div class="card">
            <div class="card-body">
                <form id="profileForm">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label>Họ tên</label>
                            <input type="text" class="form-control" id="hoTen" required>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label>Tuổi</label>
                            <input type="number" class="form-control" id="tuoi" required>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label>Giới tính</label>
                            <select class="form-control" id="gioiTinh">
                                <option value="Nam">Nam</option>
                                <option value="Nữ">Nữ</option>
                            </select>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Chiều cao (cm)</label>
                            <input type="number" class="form-control" id="chieuCao" step="0.1" required>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Cân nặng (kg)</label>
                            <input type="number" class="form-control" id="canNang" step="0.1" required>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-primary">💾 Lưu thông tin</button>
                </form>
            </div>
        </div>
    `;
    
    fetch('http://localhost:5000/api/ThongTinCaNhan/' + currentUser.maNguoiDung)
        .then(function(r) { 
            if (!r.ok) return null;
            return r.json(); 
        })
        .then(function(data) {
            if (data) {
                document.getElementById('hoTen').value = data.hoTen || '';
                document.getElementById('tuoi').value = data.tuoi || '';
                document.getElementById('gioiTinh').value = data.gioiTinh || 'Nam';
                document.getElementById('chieuCao').value = data.chieuCao || '';
                document.getElementById('canNang').value = data.canNang || '';
            }
        })
        .catch(function(e) { console.error(e); });
    
    setTimeout(function() {
        document.getElementById('profileForm').addEventListener('submit', function(e) {
            e.preventDefault();
            saveProfile();
        });
    }, 500);
}

function saveProfile() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        hoTen: document.getElementById('hoTen').value,
        tuoi: parseInt(document.getElementById('tuoi').value),
        gioiTinh: document.getElementById('gioiTinh').value,
        chieuCao: parseFloat(document.getElementById('chieuCao').value),
        canNang: parseFloat(document.getElementById('canNang').value)
    };
    
    fetch('http://localhost:5000/api/ThongTinCaNhan', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(function(r) { return r.json(); })
    .then(function() {
        alert('✅ Đã lưu thông tin!');
    })
    .catch(function(e) {
        console.error(e);
        alert('❌ Lỗi khi lưu!');
    });
}

function loadHealthMetrics() {
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
                            <label>Cân nặng (kg)</label>
                            <input type="number" step="0.1" class="form-control" id="canNangCS" required>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Huyết áp (VD: 120/80)</label>
                            <input type="text" class="form-control" id="huyetAp" placeholder="120/80">
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Nhịp tim (bpm)</label>
                            <input type="number" class="form-control" id="nhipTim">
                        </div>
                        <div class="col-md-3 mb-3">
                            <label>Đường huyết (mg/dL)</label>
                            <input type="number" step="0.1" class="form-control" id="duongHuyet">
                        </div>
                    </div>
                    <button type="submit" class="btn btn-success">💾 Lưu chỉ số</button>
                </form>
            </div>
        </div>
        <div class="card">
            <div class="card-header"><h5>Lịch sử chỉ số</h5></div>
            <div class="card-body"><div id="healthHistory"></div></div>
        </div>
    `;

    loadHealthHistory();
    
    setTimeout(function() {
        document.getElementById('healthForm').addEventListener('submit', function(e) {
            e.preventDefault();
            saveHealthMetrics();
        });
    }, 500);
}

function saveHealthMetrics() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        ngayDo: new Date().toISOString(),
        canNang: parseFloat(document.getElementById('canNangCS').value),
        huyetAp: document.getElementById('huyetAp').value,
        nhipTim: parseInt(document.getElementById('nhipTim').value) || null,
        duongHuyet: parseFloat(document.getElementById('duongHuyet').value) || null
    };

    fetch('http://localhost:5000/api/SucKhoe/chiso', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(function(r) { return r.json(); })
    .then(function() {
        alert('✅ Đã lưu chỉ số sức khỏe!');
        document.getElementById('healthForm').reset();
        loadHealthHistory();
    })
    .catch(function(e) {
        console.error(e);
        alert('❌ Lỗi khi lưu chỉ số!');
    });
}

function loadHealthHistory() {
    fetch('http://localhost:5000/api/SucKhoe/lichsu/' + currentUser.maNguoiDung)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            const historyDiv = document.getElementById('healthHistory');
            if (data.length === 0) {
                historyDiv.innerHTML = '<p class="text-muted">Chưa có dữ liệu</p>';
                return;
            }

            let html = '<table class="table table-striped"><thead><tr>';
            html += '<th>Ngày</th><th>Cân nặng</th><th>Huyết áp</th><th>Nhịp tim</th><th>Đường huyết</th>';
            html += '</tr></thead><tbody>';

            data.forEach(function(item) {
                html += '<tr>';
                html += '<td>' + new Date(item.ngayDo).toLocaleDateString('vi-VN') + '</td>';
                html += '<td>' + (item.canNang || '-') + ' kg</td>';
                html += '<td>' + (item.huyetAp || '-') + '</td>';
                html += '<td>' + (item.nhipTim || '-') + ' bpm</td>';
                html += '<td>' + (item.duongHuyet || '-') + ' mg/dL</td>';
                html += '</tr>';
            });

            html += '</tbody></table>';
            historyDiv.innerHTML = html;
        })
        .catch(function(e) { console.error(e); });
}

function loadBMIHistory() {
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
                            <label>Chiều cao (cm)</label>
                            <input type="number" step="0.1" class="form-control" id="chieuCaoBMI" required>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label>Cân nặng (kg)</label>
                            <input type="number" step="0.1" class="form-control" id="canNangBMI" required>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-info">🔢 Tính BMI</button>
                </form>
                <div id="bmiResult" class="mt-3"></div>
            </div>
        </div>
        <div class="card">
            <div class="card-header"><h5>Lịch sử BMI</h5></div>
            <div class="card-body">
                <canvas id="bmiChart" style="max-height: 300px;"></canvas>
                <div id="bmiHistory" class="mt-3"></div>
            </div>
        </div>
    `;

    loadBMIData();
    
    setTimeout(function() {
        document.getElementById('bmiForm').addEventListener('submit', function(e) {
            e.preventDefault();
            calculateBMI();
        });
    }, 500);
}

function calculateBMI() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        chieuCao: parseFloat(document.getElementById('chieuCaoBMI').value),
        canNang: parseFloat(document.getElementById('canNangBMI').value)
    };

    fetch('http://localhost:5000/api/BMI/tinh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(function(r) { return r.json(); })
    .then(function(result) {
        const resultDiv = document.getElementById('bmiResult');
        let color = 'success';
        if (result.trangThai.includes('Béo phì') || result.trangThai.includes('Gầy')) color = 'danger';
        else if (result.trangThai.includes('Thừa cân')) color = 'warning';

        resultDiv.innerHTML = '<div class="alert alert-' + color + '">' +
            '<h5>BMI của bạn: ' + result.bmi + '</h5>' +
            '<p>Trạng thái: <strong>' + result.trangThai + '</strong></p></div>';

        loadBMIData();
    })
    .catch(function(e) {
        console.error(e);
        alert('❌ Lỗi khi tính BMI!');
    });
}

function loadBMIData() {
    fetch('http://localhost:5000/api/BMI/lichsu/' + currentUser.maNguoiDung)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            const historyDiv = document.getElementById('bmiHistory');
            if (data.length === 0) {
                historyDiv.innerHTML = '<p class="text-muted">Chưa có dữ liệu BMI</p>';
                return;
            }

            let html = '<table class="table table-striped"><thead><tr>';
            html += '<th>Ngày</th><th>BMI</th><th>Trạng thái</th>';
            html += '</tr></thead><tbody>';

            data.forEach(function(item) {
                html += '<tr>';
                html += '<td>' + new Date(item.ngayTinh).toLocaleDateString('vi-VN') + '</td>';
                html += '<td>' + item.giaTriBMI + '</td>';
                html += '<td><span class="badge bg-info">' + item.trangThai + '</span></td>';
                html += '</tr>';
            });

            html += '</tbody></table>';
            historyDiv.innerHTML = html;

            drawBMIChart(data);
        })
        .catch(function(e) { console.error(e); });
}

function drawBMIChart(data) {
    const ctx = document.getElementById('bmiChart');
    if (!ctx) return;

    const labels = data.reverse().map(function(item) {
        return new Date(item.ngayTinh).toLocaleDateString('vi-VN');
    });
    const values = data.map(function(item) { return item.giaTriBMI; });

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

function loadHabits() {
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
                            <label>Số giờ ngủ trung bình/ngày</label>
                            <input type="number" step="0.5" class="form-control" id="soGioNgu" min="0" max="24">
                        </div>
                        <div class="col-md-6 mb-3">
                            <label>Thời gian tập luyện (phút/ngày)</label>
                            <input type="number" class="form-control" id="thoiGianTapLuyen" min="0">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="hutThuoc">
                                <label class="form-check-label" for="hutThuoc">🚬 Hút thuốc</label>
                            </div>
                        </div>
                        <div class="col-md-6 mb-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="uongRuouBia">
                                <label class="form-check-label" for="uongRuouBia">🍺 Uống rượu/bia</label>
                            </div>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-warning">💾 Lưu thói quen</button>
                </form>
            </div>
        </div>
    `;

    loadHabitsData();
    
    setTimeout(function() {
        document.getElementById('habitsForm').addEventListener('submit', function(e) {
            e.preventDefault();
            saveHabits();
        });
    }, 500);
}

function loadHabitsData() {
    fetch('http://localhost:5000/api/ThoiQuen/' + currentUser.maNguoiDung)
        .then(function(r) {
            if (!r.ok) return null;
            return r.json();
        })
        .then(function(data) {
            if (!data) return;
            document.getElementById('soGioNgu').value = data.soGioNgu || '';
            document.getElementById('thoiGianTapLuyen').value = data.thoiGianTapLuyen || '';
            document.getElementById('hutThuoc').checked = data.hutThuoc || false;
            document.getElementById('uongRuouBia').checked = data.uongRuouBia || false;
        })
        .catch(function(e) { console.log('No habits data yet'); });
}

function saveHabits() {
    const data = {
        maNguoiDung: currentUser.maNguoiDung,
        soGioNgu: parseFloat(document.getElementById('soGioNgu').value) || null,
        thoiGianTapLuyen: parseInt(document.getElementById('thoiGianTapLuyen').value) || null,
        hutThuoc: document.getElementById('hutThuoc').checked,
        uongRuouBia: document.getElementById('uongRuouBia').checked
    };

    fetch('http://localhost:5000/api/ThoiQuen', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    .then(function(r) { return r.json(); })
    .then(function() {
        alert('✅ Đã lưu thói quen sinh hoạt!');
    })
    .catch(function(e) {
        console.error(e);
        alert('❌ Lỗi khi lưu thói quen!');
    });
}

function loadMenu() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>🍎 Gợi ý thực đơn lành mạnh</h3>
        <div class="card mb-3">
            <div class="card-header bg-success text-white">
                <h5>🤖 Tạo thực đơn từ AI</h5>
            </div>
            <div class="card-body">
                <button class="btn btn-success btn-lg" onclick="generateMenuSmart()">✨ Tạo thực đơn thông minh</button>
                <button class="btn btn-outline-primary ms-2" onclick="showAllFoods()">📋 Xem tất cả món ăn</button>
                <div id="menuSuggestions" class="mt-4"></div>
            </div>
        </div>
    `;
}

function generateMenuSmart() {
    document.getElementById('menuSuggestions').innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
    
    fetch('http://localhost:5000/api/MonAn/goiy/' + currentUser.maNguoiDung)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            let html = '<div class="alert alert-success"><h5>💡 Lời khuyên chung từ AI:</h5><ul>';
            if (data.goiYChung && data.goiYChung.length > 0) {
                data.goiYChung.forEach(function(item) {
                    html += '<li>' + item + '</li>';
                });
            }
            html += '</ul></div>';
            
            if (data.cacMonAnKhac && data.cacMonAnKhac.length > 0) {
                html += '<h5 class="mt-3">🍴 Các món ăn phù hợp:</h5>';
                html += '<table class="table table-hover"><thead><tr>';
                html += '<th>Món ăn</th><th>Calo</th><th>Đạm (g)</th><th>Béo (g)</th><th>Bot (g)</th>';
                html += '</tr></thead><tbody>';
                
                data.cacMonAnKhac.forEach(function(mon) {
                    html += '<tr><td>' + mon.tenMonAn + '</td>';
                    html += '<td>' + mon.calo + '</td>';
                    html += '<td>' + mon.chatDam + '</td>';
                    html += '<td>' + mon.chatBeo + '</td>';
                    html += '<td>' + mon.chatBot + '</td></tr>';
                });
                
                html += '</tbody></table>';
            }
            
            document.getElementById('menuSuggestions').innerHTML = html;
        })
        .catch(function(e) {
            console.error(e);
            document.getElementById('menuSuggestions').innerHTML = 
                '<div class="alert alert-warning">Chưa có dữ liệu để gợi ý thực đơn. Vui lòng cập nhật thông tin sức khỏe trước.</div>';
        });
}

function showAllFoods() {
    fetch('http://localhost:5000/api/MonAn')
        .then(function(r) { return r.json(); })
        .then(function(foods) {
            let html = '<h5>📋 Tất cả món ăn trong hệ thống</h5>';
            html += '<table class="table table-striped"><thead><tr>';
            html += '<th>#</th><th>Món ăn</th><th>Calo</th><th>Đạm</th><th>Béo</th><th>Bot</th>';
            html += '</tr></thead><tbody>';
            
            foods.forEach(function(food, index) {
                html += '<tr><td>' + (index + 1) + '</td>';
                html += '<td><strong>' + food.tenMonAn + '</strong></td>';
                html += '<td>' + food.calo + '</td>';
                html += '<td>' + food.chatDam + 'g</td>';
                html += '<td>' + food.chatBeo + 'g</td>';
                html += '<td>' + food.chatBot + 'g</td></tr>';
            });
            
            html += '</tbody></table>';
            document.getElementById('menuSuggestions').innerHTML = html;
        })
        .catch(function(e) {
            console.error(e);
            alert('Lỗi khi tải danh sách món ăn!');
        });
}

function loadWarnings() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>⚠️ Cảnh báo sức khỏe</h3>
        <div class="card">
            <div class="card-body"><div id="warningsList"></div></div>
        </div>
    `;

    fetch('http://localhost:5000/api/CanhBao/' + currentUser.maNguoiDung)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            const warningsDiv = document.getElementById('warningsList');
            if (data.length === 0) {
                warningsDiv.innerHTML = '<p class="text-success">✅ Không có cảnh báo</p>';
                return;
            }

            let html = '';
            data.forEach(function(item) {
                const alertType = item.trangThai === 'Chưa xem' ? 'warning' : 'secondary';
                html += '<div class="alert alert-' + alertType + '">';
                html += '<strong>' + new Date(item.ngayCanhBao).toLocaleDateString('vi-VN') + '</strong>';
                html += '<p>' + item.noiDung + '</p>';
                html += '<small class="text-muted">Trạng thái: ' + item.trangThai + '</small>';
                html += '</div>';
            });

            warningsDiv.innerHTML = html;
        })
        .catch(function(e) { console.error(e); });
}

function loadAISuggestions() {
    const content = document.getElementById('content');
    content.innerHTML = `
        <h3>🤖 Tư vấn AI</h3>
        <div class="row">
            <div class="col-md-8">
                <button class="btn btn-success btn-lg mb-3 w-100" onclick="analyzeHealth()">🔍 Phân tích sức khỏe bằng AI</button>
                <div id="aiAnalysis"></div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-header"><h5>📋 Lịch sử phân tích</h5></div>
                    <div class="card-body" style="max-height: 600px; overflow-y: auto;"><div id="aiHistory"></div></div>
                </div>
            </div>
        </div>
    `;

    loadAIHistory();
}

function loadAIHistory() {
    fetch('http://localhost:5000/api/AI/goiy/' + currentUser.maNguoiDung)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            const historyDiv = document.getElementById('aiHistory');
            if (!historyDiv) return;
            
            if (data.length === 0) {
                historyDiv.innerHTML = '<p class="text-muted">Chưa có lịch sử phân tích</p>';
                return;
            }

            let html = '';
            data.forEach(function(item) {
                html += '<div class="card mb-2"><div class="card-body">';
                html += '<small class="text-muted">' + new Date(item.ngayTao).toLocaleString('vi-VN') + '</small>';
                html += '<pre style="font-size: 11px; max-height: 100px; overflow-y: auto;">' + item.noiDungGoiY.substring(0, 150) + '...</pre>';
                html += '</div></div>';
            });

            historyDiv.innerHTML = html;
        })
        .catch(function(e) { console.error(e); });
}

function analyzeHealth() {
    document.getElementById('aiAnalysis').innerHTML = '<div class="text-center"><div class="spinner-border"></div><p>🤖 AI đang phân tích...</p></div>';
    
    fetch('http://localhost:5000/api/AI/phantich/' + currentUser.maNguoiDung, {
        method: 'POST'
    })
    .then(function(r) {
        if (!r.ok) {
            return r.text().then(function(t) {
                throw new Error('HTTP ' + r.status + ': ' + (t || 'Không có phản hồi'));
            });
        }
        return r.json();
    })
    .then(function(data) {
        const aiText = data.baoCaoAI || '';
        let html = '<div class="card bg-light"><div class="card-body">';
        html += '<h5>📊 Kết quả phân tích từ AI Rule-Based</h5>';
        html += '<div class="alert alert-info">ℹ️ Phân tích dựa trên thuật toán y khoa chuẩn (không phụ thuộc API bên ngoài).</div>';
        html += '<p><strong>BMI:</strong> ' + data.bmi + ' - ' + data.trangThaiBMI + '</p>';
        html += '<hr><pre style="white-space: pre-wrap;">' + aiText + '</pre>';
        html += '</div></div>';
        
        document.getElementById('aiAnalysis').innerHTML = html;
        loadAIHistory();
    })
    .catch(function(e) {
        console.error(e);
        document.getElementById('aiAnalysis').innerHTML = 
            '<div class="alert alert-danger">❌ Lỗi phân tích: ' + e.message + '</div>';
    });
}
