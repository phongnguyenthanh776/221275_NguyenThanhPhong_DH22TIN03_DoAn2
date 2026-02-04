USE QuanLySucKhoeThongMinh;
GO

-- Xóa dữ liệu cũ
DELETE FROM dbo.ThucDon;
DELETE FROM dbo.MonAn;
GO

-- Thêm món ăn chi tiết hơn
INSERT INTO dbo.MonAn (TenMonAn, Calo, ChatDam, ChatBeo, ChatBot) VALUES
-- Bữa sáng
(N'Yến mạch với sữa tươi', 389, 16.9, 6.9, 66),
(N'Trứng luộc', 155, 13, 11, 1.1),
(N'Chuối', 89, 1.1, 0.3, 23),
(N'Sữa tươi không đường', 42, 3.4, 1, 5),
(N'Bánh mì nguyên cám', 247, 13, 3.4, 41),
(N'Cam vắt', 47, 0.9, 0.1, 12),

-- Bữa trưa - Món chính
(N'Cơm gạo lứt', 111, 2.6, 0.9, 23),
(N'Ức gà nướng', 165, 31, 3.6, 0),
(N'Cá hồi nướng', 206, 22, 13, 0),
(N'Thịt bò nạc xào', 250, 26, 17, 0),
(N'Cá thu nướng', 180, 25, 9, 0),

-- Rau củ
(N'Salad rau xanh', 35, 2.5, 0.5, 7),
(N'Bông cải xanh luộc', 55, 3.7, 0.6, 11),
(N'Cà rốt luộc', 41, 0.9, 0.2, 10),
(N'Rau muống xào tỏi', 30, 2.8, 0.4, 5.4),
(N'Canh chua cá', 120, 12, 4, 10),

-- Đồ ăn nhẹ
(N'Táo', 52, 0.3, 0.2, 14),
(N'Dưa hấu', 30, 0.6, 0.2, 8),
(N'Hạnh nhân', 579, 21, 50, 22),
(N'Sữa chua không đường', 59, 3.5, 0.4, 4.7),
(N'Đậu phụ luộc', 76, 8, 4.8, 1.9),

-- Bữa tối
(N'Súp gà nấm', 85, 7, 3, 8),
(N'Cá diêu hồng hấp', 85, 18, 1, 0),
(N'Đậu que xào', 44, 2.4, 0.2, 10),
(N'Khoai lang luộc', 86, 1.6, 0.1, 20),
(N'Phở gà không mỡ', 280, 18, 4, 45),

-- Món Việt Nam khác
(N'Bún chả', 450, 20, 15, 55),
(N'Gỏi cuốn', 150, 8, 3, 25),
(N'Bánh cuốn', 98, 3.2, 0.5, 20),
(N'Chè đậu đỏ không đường', 120, 5, 1, 24),
(N'Nước ép cà rốt', 94, 2.2, 0.4, 22);

GO

PRINT N'✅ Đã thêm ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' món ăn!';
GO

-- Xem kết quả
SELECT * FROM dbo.MonAn ORDER BY Calo;
GO
