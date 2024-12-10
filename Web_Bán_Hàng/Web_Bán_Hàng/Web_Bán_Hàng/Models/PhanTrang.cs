namespace Web_Bán_Hàng.Models
{
    public class PhanTrang
    {
        public int TongSoMuc {  get; set; } 
        public int KichThuocTrang { get; set; }
        public int TrangHienTai {  get; set; }
        public int TongSoTrang {  get; set; }
        public int TrangDauTien {  get; set; }
        public int TrangCuoi {  get; set; }
        public PhanTrang()
        {

        }
        public PhanTrang(int tongsomuc, int trang, int kichthuoctrang)
        {
            int tongsotrang = (int)Math.Ceiling((decimal)tongsomuc / kichthuoctrang);
            int tranghientai = trang;
            int trangbatdau = tranghientai - 5;
            int trangketthuc = trangbatdau + 4;
            if(trangbatdau <= 0)
            {
                trangketthuc = trangketthuc - (trangbatdau - 1);
                trangbatdau = 1;
            }
            if(trangketthuc > tongsotrang)
            {
                trangketthuc = tongsotrang;
                if(trangketthuc > 10)
                {
                    trangbatdau = trangketthuc - 9;
                }

            }
            TongSoMuc = tongsomuc;
            TrangHienTai = tranghientai;
            KichThuocTrang = kichthuoctrang;
            TongSoTrang = tongsotrang;
            TrangDauTien = trangbatdau;
            TrangCuoi = trangketthuc;






        }

    }
}

