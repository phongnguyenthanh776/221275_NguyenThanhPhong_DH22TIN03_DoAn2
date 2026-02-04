USE QuanLySucKhoeThongMinh;
GO

PRINT '====================================';
PRINT 'XOA DU LIEU TRUNG LAP';
PRINT '====================================';

-- Xem các tài khoản trùng lặp
SELECT TenDangNhap, COUNT(*) as SoLuong
FROM dbo.NguoiDung
GROUP BY TenDangNhap
HAVING COUNT(*) > 1;

-- Xóa các bản ghi trùng lặp, giữ lại bản ghi có MaNguoiDung nhỏ nhất
WITH CTE AS (
    SELECT 
        MaNguoiDung,
        TenDangNhap,
        ROW_NUMBER() OVER (PARTITION BY TenDangNhap ORDER BY MaNguoiDung) AS RowNum
    FROM dbo.NguoiDung
)
DELETE FROM CTE WHERE RowNum > 1;

PRINT 'Da xoa du lieu trung lap!';

-- Xem kết quả sau khi xóa
SELECT * FROM dbo.NguoiDung ORDER BY MaNguoiDung;
GO
