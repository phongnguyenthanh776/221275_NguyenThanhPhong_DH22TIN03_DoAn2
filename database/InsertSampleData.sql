USE QuanLySucKhoeThongMinh;
GO

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM dbo.GoiYSucKhoeAI;
DELETE FROM dbo.CanhBaoSucKhoe;
DELETE FROM dbo.NguyCoBenhLy;
DELETE FROM dbo.ThucDon;
DELETE FROM dbo.ThoiQuenSinhHoat;
DELETE FROM dbo.LichSuBMI;
DELETE FROM dbo.ChiSoSucKhoe;
DELETE FROM dbo.MonAn;
DELETE FROM dbo.ThongTinCaNhan;
DELETE FROM dbo.NguoiDung;
GO

-- Insert dữ liệu mẫu MonAn
INSERT INTO dbo.MonAn (TenMonAn, Calo, ChatDam, ChatBeo, ChatBot) VALUES
(N'Cơm trắng', 130, 2.7, 0.3, 28),
(N'Ức gà nướng', 165, 31, 3.6, 0),
(N'Cá hồi nướng', 206, 22, 13, 0),
(N'Salad rau xanh', 35, 2.5, 0.5, 7),
(N'Trứng luộc', 155, 13, 11, 1.1),
(N'Yến mạch', 389, 16.9, 6.9, 66),
(N'Chuối', 89, 1.1, 0.3, 23),
(N'Sữa tươi không đường', 42, 3.4, 1, 5),
(N'Bông cải xanh luộc', 55, 3.7, 0.6, 11),
(N'Cà rốt luộc', 41, 0.9, 0.2, 10),
(N'Thịt bò nạc', 250, 26, 17, 0),
(N'Đậu phụ', 76, 8, 4.8, 1.9),
(N'Khoai lang luộc', 86, 1.6, 0.1, 20),
(N'Táo', 52, 0.3, 0.2, 14),
(N'Cam', 47, 0.9, 0.1, 12),
(N'Phở bò', 350, 15, 5, 50),
(N'Bánh mì thịt', 400, 18, 12, 45),
(N'Bún chả', 450, 20, 15, 55),
(N'Gỏi cuốn', 150, 8, 3, 25),
(N'Canh chua cá', 120, 12, 4, 10);
GO

-- Insert người dùng mẫu
INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhau, Email, VaiTro) VALUES
(N'admin', N'admin123', N'admin@example.com', N'Admin'),
(N'user1', N'123456', N'user1@example.com', N'User'),
(N'user2', N'123456', N'user2@example.com', N'User');
GO

PRINT N'✅ Đã insert dữ liệu mẫu thành công!';
GO
