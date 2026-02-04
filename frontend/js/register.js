document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const tenDangNhap = document.getElementById('tenDangNhap').value.trim();
    const email = document.getElementById('email').value.trim();
    const matKhau = document.getElementById('matKhau').value;
    const xacNhanMatKhau = document.getElementById('xacNhanMatKhau').value;

    // Validate
    if (tenDangNhap.length < 3) {
        alert('❌ Tên đăng nhập phải có ít nhất 3 ký tự!');
        document.getElementById('tenDangNhap').focus();
        return;
    }

    // Kiểm tra tên đăng nhập chỉ chứa chữ cái, số và dấu gạch dưới
    const usernameRegex = /^[a-zA-Z0-9_]+$/;
    if (!usernameRegex.test(tenDangNhap)) {
        alert('❌ Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới!');
        document.getElementById('tenDangNhap').focus();
        return;
    }

    // Validate email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        alert('❌ Email không hợp lệ!');
        document.getElementById('email').focus();
        return;
    }

    if (matKhau.length < 6) {
        alert('❌ Mật khẩu phải có ít nhất 6 ký tự!');
        document.getElementById('matKhau').focus();
        return;
    }

    if (matKhau !== xacNhanMatKhau) {
        alert('❌ Mật khẩu xác nhận không khớp!');
        document.getElementById('xacNhanMatKhau').focus();
        return;
    }

    const data = {
        tenDangNhap: tenDangNhap,
        email: email,
        matKhau: matKhau,
        vaiTro: 'User'
    };

    console.log('Đang đăng ký:', { tenDangNhap, email });

    // Disable button để tránh click nhiều lần
    const submitBtn = e.target.querySelector('button[type="submit"]');
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Đang xử lý...';

    try {
        const response = await fetch('http://localhost:5000/api/NguoiDung/register', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (response.ok) {
            console.log('Đăng ký thành công:', result);
            alert('✅ Đăng ký thành công!\n\nTài khoản: ' + tenDangNhap + '\nVui lòng đăng nhập.');
            window.location.href = 'login.html';
        } else {
            console.log('Lỗi:', result);
            alert('❌ ' + (result.message || 'Đăng ký thất bại!'));
            submitBtn.disabled = false;
            submitBtn.innerHTML = '✅ Đăng ký';
        }
    } catch (error) {
        console.error('Error:', error);
        alert('❌ Lỗi kết nối! Vui lòng kiểm tra:\n- Backend đang chạy\n- Kết nối internet\n\nChi tiết: ' + error.message);
        submitBtn.disabled = false;
        submitBtn.innerHTML = '✅ Đăng ký';
    }
});

// Thêm real-time validation
document.getElementById('tenDangNhap').addEventListener('input', (e) => {
    const value = e.target.value;
    const usernameRegex = /^[a-zA-Z0-9_]*$/;
    
    if (!usernameRegex.test(value)) {
        e.target.classList.add('is-invalid');
    } else {
        e.target.classList.remove('is-invalid');
    }
});

document.getElementById('xacNhanMatKhau').addEventListener('input', (e) => {
    const matKhau = document.getElementById('matKhau').value;
    const xacNhan = e.target.value;
    
    if (xacNhan && matKhau !== xacNhan) {
        e.target.classList.add('is-invalid');
    } else {
        e.target.classList.remove('is-invalid');
    }
});
