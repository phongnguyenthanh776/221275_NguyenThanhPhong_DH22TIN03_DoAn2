-- Tạo bảng LichSuChat
CREATE TABLE [dbo].[LichSuChat] (
    [MaChat] INT PRIMARY KEY IDENTITY(1,1),
    [MaNguoiDung] INT NOT NULL,
    [TinNhanNguoiDung] NVARCHAR(MAX) NOT NULL,
    [TinNhanAI] NVARCHAR(MAX) NOT NULL,
    [NgayTao] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LichSuChat_NguoiDung] FOREIGN KEY ([MaNguoiDung]) 
        REFERENCES [dbo].[NguoiDung]([MaNguoiDung]) ON DELETE CASCADE
);

-- Tạo index để tìm kiếm nhanh theo MaNguoiDung
CREATE INDEX [IX_LichSuChat_MaNguoiDung] ON [dbo].[LichSuChat]([MaNguoiDung]);
CREATE INDEX [IX_LichSuChat_NgayTao] ON [dbo].[LichSuChat]([NgayTao]);
