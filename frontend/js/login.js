document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const tenDangNhap = document.getElementById('tenDangNhap').value.trim();
    const matKhau = document.getElementById('matKhau').value;

    if (!tenDangNhap || !matKhau) {
        alert('❌ Vui lòng nhập đầy đủ thông tin!');
        return;
    }

    const data = {
        tenDangNhap: tenDangNhap,
        matKhau: matKhau
    };

    console.log('Đang đăng nhập:', data);

    try {
        const response = await fetch('http://localhost:5000/api/NguoiDung/login', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        console.log('Response status:', response.status);

        if (response.ok) {
            const user = await response.json();
            console.log('Đăng nhập thành công:', user);
            
            localStorage.setItem('user', JSON.stringify(user));
            alert('✅ Đăng nhập thành công!');
            window.location.href = 'dashboard.html';
        } else {
            const error = await response.json();
            console.log('Lỗi:', error);
            alert('❌ ' + (error.message || 'Sai tên đăng nhập hoặc mật khẩu!'));
        }
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi kết nối! Chi tiết: ' + error.message);
    }
});
