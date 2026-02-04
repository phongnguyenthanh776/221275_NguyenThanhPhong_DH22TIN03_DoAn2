const API_URL = 'http://localhost:5000/api';

function getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
}

function logout() {
    localStorage.removeItem('user');
    window.location.href = '../index.html';
}

function checkLogin() {
    const user = getCurrentUser();
    if (!user) {
        window.location.href = 'login.html';
        return null;
    }
    return user;
}
