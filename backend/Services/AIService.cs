public class AIService
{
    public string PhanTichBMI(double bmi)
    {
        if (bmi < 18.5) return "Gầy";
        if (bmi < 25) return "Bình thường";
        if (bmi < 30) return "Thừa cân";
        return "Béo phì";
    }

    public double TinhNguyCoTieuDuong(double bmi, double duongHuyet, int tuoi)
    {
        double nguyCo = 0;
        
        if (bmi > 25) nguyCo += 0.3;
        if (duongHuyet > 100) nguyCo += 0.4;
        if (tuoi > 45) nguyCo += 0.2;
        
        return Math.Min(nguyCo, 1.0);
    }

    public double TinhNguyCoCaoHuyetAp(string huyetAp, double bmi, int tuoi)
    {
        double nguyCo = 0;
        
        var parts = huyetAp.Split('/');
        if (parts.Length == 2)
        {
            if (int.Parse(parts[0]) > 140) nguyCo += 0.4;
            if (int.Parse(parts[1]) > 90) nguyCo += 0.3;
        }
        
        if (bmi > 25) nguyCo += 0.2;
        if (tuoi > 40) nguyCo += 0.1;
        
        return Math.Min(nguyCo, 1.0);
    }

    public List<string> GoiYThucDon(string trangThaiBMI, bool tieuDuong)
    {
        var danhSach = new List<string>();
        
        if (trangThaiBMI == "Béo phì" || trangThaiBMI == "Thừa cân")
        {
            danhSach.Add("Salad rau xanh");
            danhSach.Add("Ức gà nướng");
            danhSach.Add("Cá hồi nướng");
        }
        
        if (tieuDuong)
        {
            danhSach.Add("Yến mạch");
            danhSach.Add("Rau củ luộc");
        }
        
        return danhSach;
    }
}
