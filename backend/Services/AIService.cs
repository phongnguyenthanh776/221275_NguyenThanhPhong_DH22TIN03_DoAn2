using System.Text;

public class AIService
{
    // Phân tích BMI
    public string PhanTichBMI(double bmi)
    {
        if (bmi < 16) return "Gầy độ III (rất nguy hiểm)";
        if (bmi < 17) return "Gầy độ II";
        if (bmi < 18.5) return "Gầy độ I";
        if (bmi < 25) return "Bình thường";
        if (bmi < 30) return "Thừa cân";
        if (bmi < 35) return "Béo phì độ I";
        if (bmi < 40) return "Béo phì độ II";
        return "Béo phì độ III (rất nguy hiểm)";
    }

    // Tính điểm nguy cơ tiểu đường (0-100)
    public double TinhNguyCoTieuDuong(double bmi, double duongHuyet, int tuoi, bool coTienSuGiaDinh = false)
    {
        double diem = 0;

        // Điểm từ BMI (0-30 điểm)
        if (bmi < 25) diem += 0;
        else if (bmi < 27) diem += 10;
        else if (bmi < 30) diem += 20;
        else diem += 30;

        // Điểm từ đường huyết (0-40 điểm)
        if (duongHuyet < 100) diem += 0;
        else if (duongHuyet < 110) diem += 15;
        else if (duongHuyet < 126) diem += 25; // Tiền tiểu đường
        else diem += 40; // Tiểu đường

        // Điểm từ tuổi (0-20 điểm)
        if (tuoi < 30) diem += 0;
        else if (tuoi < 40) diem += 5;
        else if (tuoi < 50) diem += 10;
        else if (tuoi < 60) diem += 15;
        else diem += 20;

        // Tiền sử gia đình (0-10 điểm)
        if (coTienSuGiaDinh) diem += 10;

        // Chuẩn hóa về thang 0-100
        return Math.Min(diem, 100);
    }

    // Tính điểm nguy cơ cao huyết áp (0-100)
    public double TinhNguyCoCaoHuyetAp(string huyetAp, double bmi, int tuoi, bool hutThuoc = false, bool uongRuou = false)
    {
        double diem = 0;

        // Parse huyết áp
        var parts = huyetAp.Split('/');
        if (parts.Length == 2)
        {
            int tam_thu = int.Parse(parts[0]);
            int tam_truong = int.Parse(parts[1]);

            // Điểm từ huyết áp tâm thu (0-40 điểm)
            if (tam_thu < 120) diem += 0;
            else if (tam_thu < 130) diem += 10; // Hơi cao
            else if (tam_thu < 140) diem += 20; // Tiền cao huyết áp
            else if (tam_thu < 160) diem += 30; // Cao huyết áp độ 1
            else diem += 40; // Cao huyết áp độ 2

            // Điểm từ huyết áp tâm trương (0-20 điểm)
            if (tam_truong < 80) diem += 0;
            else if (tam_truong < 85) diem += 5;
            else if (tam_truong < 90) diem += 10;
            else if (tam_truong < 100) diem += 15;
            else diem += 20;
        }

        // Điểm từ BMI (0-20 điểm)
        if (bmi < 25) diem += 0;
        else if (bmi < 30) diem += 10;
        else diem += 20;

        // Điểm từ tuổi (0-10 điểm)
        if (tuoi < 40) diem += 0;
        else if (tuoi < 55) diem += 5;
        else diem += 10;

        // Yếu tố lối sống (0-10 điểm)
        if (hutThuoc) diem += 5;
        if (uongRuou) diem += 5;

        return Math.Min(diem, 100);
    }

    // Phân tích nhịp tim
    public string PhanTichNhipTim(int nhipTim, int tuoi)
    {
        int min = 60;
        int max = 100;

        // Điều chỉnh theo tuổi
        if (tuoi < 30) { min = 60; max = 100; }
        else if (tuoi < 50) { min = 65; max = 95; }
        else { min = 70; max = 90; }

        if (nhipTim < min - 10) return "Chậm bất thường (Bradycardia)";
        if (nhipTim < min) return "Hơi chậm";
        if (nhipTim <= max) return "Bình thường";
        if (nhipTim < max + 20) return "Hơi nhanh";
        return "Nhanh bất thường (Tachycardia)";
    }

    // Gợi ý thực đơn dựa trên tình trạng sức khỏe
    public List<string> GoiYThucDon(string trangThaiBMI, bool coNguyCoTieuDuong, bool coNguyCoCaoHuyetAp)
    {
        var danhSach = new List<string>();

        // Gợi ý chung
        danhSach.Add("💧 Uống đủ 2-2.5 lít nước mỗi ngày");

        // Dựa trên BMI
        if (trangThaiBMI.Contains("Gầy"))
        {
            danhSach.Add("🥑 Tăng cường năng lượng: Bơ, hạt điều, óc chó");
            danhSach.Add("🥩 Protein cao: Thịt nạc, cá hồi, trứng");
            danhSach.Add("🍠 Carbs tốt: Yến mạch, khoai lang, gạo lứt");
            danhSach.Add("🥛 Sữa, phô mai, sữa chua để tăng cân lành mạnh");
        }
        else if (trangThaiBMI.Contains("Thừa cân") || trangThaiBMI.Contains("Béo phì"))
        {
            danhSach.Add("🥗 Salad rau xanh với dầu olive");
            danhSach.Add("🐟 Cá nướng (cá hồi, cá thu) giàu omega-3");
            danhSach.Add("🍗 Ức gà luộc hoặc nướng (không da)");
            danhSach.Add("🥦 Rau củ luộc: Bông cải xanh, cà rốt, su hào");
            danhSach.Add("🍎 Trái cây ít đường: táo, bưởi, cam");
            danhSach.Add("❌ Tránh: Đồ chiên, nước ngọt, bánh ngọt");
        }
        else // Bình thường
        {
            danhSach.Add("🍚 Cơm gạo lứt hoặc yến mạch");
            danhSach.Add("🥗 Salad rau củ đầy màu sắc");
            danhSach.Add("🍗 Thịt nạc, cá, đậu phụ");
            danhSach.Add("🥜 Hạt dinh dưỡng: hạnh nhân, óc chó");
        }

        // Nếu có nguy cơ tiểu đường
        if (coNguyCoTieuDuong)
        {
            danhSach.Add("⚠️ NGUY CƠ TIỂU ĐƯỜNG:");
            danhSach.Add("  • Hạn chế đường, tinh bột trắng");
            danhSach.Add("  • Ưu tiên carbs phức: gạo lứt, yến mạch");
            danhSach.Add("  • Tăng rau xanh và chất xơ");
            danhSach.Add("  • Chia nhỏ bữa ăn (5-6 bữa/ngày)");
            danhSach.Add("  • Tránh nước ngọt, trà sữa có đường");
        }

        // Nếu có nguy cơ cao huyết áp
        if (coNguyCoCaoHuyetAp)
        {
            danhSach.Add("⚠️ NGUY CƠ CAO HUYẾT ÁP:");
            danhSach.Add("  • Giảm muối xuống dưới 5g/ngày");
            danhSach.Add("  • Tăng kali: chuối, khoai tây, rau bina");
            danhSach.Add("  • Ăn nhiều tỏi, hành tây");
            danhSach.Add("  • Tránh đồ ăn mặn, đồ chế biến sẵn");
            danhSach.Add("  • Hạn chế caffeine và rượu bia");
        }

        return danhSach;
    }

    // Đề xuất lịch tập luyện
    public List<string> GoiYTapLuyen(double bmi, int tuoi, string trangThaiSucKhoe = "bình thường")
    {
        var danhSach = new List<string>();

        danhSach.Add("🏃 KẾ HOẠCH TẬP LUYỆN:");

        if (bmi < 18.5) // Gầy
        {
            danhSach.Add("  • Tập tăng cơ: Gym 3-4 lần/tuần");
            danhSach.Add("  • Cardio nhẹ: Đi bộ 20-30 phút/ngày");
            danhSach.Add("  • Yoga hoặc Pilates cho linh hoạt");
        }
        else if (bmi > 25) // Thừa cân/béo phì
        {
            danhSach.Add("  • Cardio: Đi bộ nhanh 30-45 phút/ngày");
            danhSach.Add("  • Bơi lội 2-3 lần/tuần (ít tác động lên khớp)");
            danhSach.Add("  • Đạp xe 20-30 phút");
            danhSach.Add("  • Tránh tập quá sức ban đầu");
        }
        else // Bình thường
        {
            danhSach.Add("  • Mix cardio và tập tạ");
            danhSach.Add("  • Chạy bộ 20-30 phút 3-4 lần/tuần");
            danhSach.Add("  • Gym hoặc tập tại nhà");
            danhSach.Add("  • Yoga/Stretching cho recovery");
        }

        // Điều chỉnh theo tuổi
        if (tuoi > 50)
        {
            danhSach.Add("  ⚠️ Lưu ý: Ưu tiên bài tập ít tác động");
            danhSach.Add("  • Đi bộ, bơi thay vì chạy");
            danhSach.Add("  • Tập luyện sức mạnh với trọng lượng nhẹ");
        }

        danhSach.Add("\n  ✅ Mục tiêu: 150 phút/tuần vận động vừa phải");

        return danhSach;
    }

    // Tạo báo cáo AI chi tiết
    public string TaoBaoCaoAI(
        double bmi, 
        string huyetAp, 
        double? duongHuyet, 
        int? nhipTim, 
        int tuoi,
        bool hutThuoc = false,
        bool uongRuou = false)
    {
        var baoCao = new StringBuilder();

        baoCao.AppendLine("╔══════════════════════════════════════════════════════════╗");
        baoCao.AppendLine("║     📊 BÁO CÁO PHÂN TÍCH SỨC KHỎE TỪ AI                 ║");
        baoCao.AppendLine("╚══════════════════════════════════════════════════════════╝");
        baoCao.AppendLine();

        // 1. Phân tích BMI
        string trangThaiBMI = PhanTichBMI(bmi);
        baoCao.AppendLine($"📈 CHỈ SỐ BMI: {bmi:F1}");
        baoCao.AppendLine($"   Trạng thái: {trangThaiBMI}");
        
        if (bmi < 18.5)
            baoCao.AppendLine("   ⚠️ Cảnh báo: Cần tăng cân để đạt sức khỏe tối ưu");
        else if (bmi > 25)
            baoCao.AppendLine("   ⚠️ Cảnh báo: Nên giảm cân để giảm nguy cơ bệnh lý");
        else
            baoCao.AppendLine("   ✅ Tuyệt vời! BMI ở mức lý tưởng");
        
        baoCao.AppendLine();

        // 2. Phân tích huyết áp
        if (!string.IsNullOrEmpty(huyetAp))
        {
            var parts = huyetAp.Split('/');
            if (parts.Length == 2)
            {
                int tam_thu = int.Parse(parts[0]);
                int tam_truong = int.Parse(parts[1]);

                baoCao.AppendLine($"💓 HUYẾT ÁP: {huyetAp} mmHg");
                
                if (tam_thu < 120 && tam_truong < 80)
                    baoCao.AppendLine("   ✅ Huyết áp bình thường");
                else if (tam_thu < 140 && tam_truong < 90)
                    baoCao.AppendLine("   ⚠️ Tiền cao huyết áp - Cần theo dõi");
                else
                    baoCao.AppendLine("   🚨 Cao huyết áp - Nên khám bác sĩ");
                
                baoCao.AppendLine();
            }
        }

        // 3. Phân tích đường huyết
        if (duongHuyet.HasValue)
        {
            baoCao.AppendLine($"🩸 ĐƯỜNG HUYẾT: {duongHuyet.Value} mg/dL");
            
            if (duongHuyet.Value < 100)
                baoCao.AppendLine("   ✅ Bình thường");
            else if (duongHuyet.Value < 126)
                baoCao.AppendLine("   ⚠️ Tiền tiểu đường - Cần chú ý chế độ ăn");
            else
                baoCao.AppendLine("   🚨 Nguy cơ tiểu đường cao - Khám bác sĩ ngay");
            
            baoCao.AppendLine();
        }

        // 4. Phân tích nhịp tim
        if (nhipTim.HasValue)
        {
            string trangThaiTim = PhanTichNhipTim(nhipTim.Value, tuoi);
            baoCao.AppendLine($"❤️ NHỊP TIM: {nhipTim.Value} bpm");
            baoCao.AppendLine($"   Trạng thái: {trangThaiTim}");
            baoCao.AppendLine();
        }

        // 5. Đánh giá tổng thể
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");
        baoCao.AppendLine("🎯 ĐÁNH GIÁ TỔNG QUAN");
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");

        // Tính nguy cơ tổng thể
        double nguyCoTieuDuong = 0;
        double nguyCoCaoHuyetAp = 0;

        if (duongHuyet.HasValue)
        {
            nguyCoTieuDuong = TinhNguyCoTieuDuong(bmi, duongHuyet.Value, tuoi);
            baoCao.AppendLine($"📊 Nguy cơ tiểu đường: {nguyCoTieuDuong:F0}%");
            
            if (nguyCoTieuDuong < 30)
                baoCao.AppendLine("   Mức độ: Thấp ✅");
            else if (nguyCoTieuDuong < 60)
                baoCao.AppendLine("   Mức độ: Trung bình ⚠️");
            else
                baoCao.AppendLine("   Mức độ: Cao 🚨");
        }

        if (!string.IsNullOrEmpty(huyetAp))
        {
            nguyCoCaoHuyetAp = TinhNguyCoCaoHuyetAp(huyetAp, bmi, tuoi, hutThuoc, uongRuou);
            baoCao.AppendLine($"📊 Nguy cơ cao huyết áp: {nguyCoCaoHuyetAp:F0}%");
            
            if (nguyCoCaoHuyetAp < 30)
                baoCao.AppendLine("   Mức độ: Thấp ✅");
            else if (nguyCoCaoHuyetAp < 60)
                baoCao.AppendLine("   Mức độ: Trung bình ⚠️");
            else
                baoCao.AppendLine("   Mức độ: Cao 🚨");
        }

        baoCao.AppendLine();

        // 6. Khuyến nghị
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");
        baoCao.AppendLine("💡 KHUYẾN NGHỊ TỪ AI");
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");

        // Thực đơn
        var thucDon = GoiYThucDon(trangThaiBMI, nguyCoTieuDuong > 50, nguyCoCaoHuyetAp > 50);
        baoCao.AppendLine("\n🍎 DINH DƯỠNG:");
        foreach (var item in thucDon)
        {
            baoCao.AppendLine($"  {item}");
        }

        // Tập luyện
        var tapLuyen = GoiYTapLuyen(bmi, tuoi);
        baoCao.AppendLine();
        foreach (var item in tapLuyen)
        {
            baoCao.AppendLine($"  {item}");
        }

        // Lời khuyên chung
        baoCao.AppendLine("\n💊 LỐI SỐNG:");
        baoCao.AppendLine("  • Ngủ đủ 7-8 tiếng/đêm");
        baoCao.AppendLine("  • Giảm stress: Thiền, yoga, đọc sách");
        baoCao.AppendLine("  • Tránh thuốc lá và hạn chế rượu bia");
        baoCao.AppendLine("  • Khám sức khỏe định kỳ 3-6 tháng/lần");

        if (hutThuoc)
            baoCao.AppendLine("  🚨 Nên bỏ thuốc lá ngay để cải thiện sức khỏe!");
        
        if (uongRuou)
            baoCao.AppendLine("  ⚠️ Hạn chế rượu bia tối đa!");

        baoCao.AppendLine();
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");
        baoCao.AppendLine($"📅 Ngày phân tích: {DateTime.Now:dd/MM/yyyy HH:mm}");
        baoCao.AppendLine("⚕️ Lưu ý: Đây chỉ là phân tích AI, không thay thế ý kiến bác sĩ");
        baoCao.AppendLine("═══════════════════════════════════════════════════════════");

        return baoCao.ToString();
    }
}