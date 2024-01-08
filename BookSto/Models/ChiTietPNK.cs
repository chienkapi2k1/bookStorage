using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class ChiTietPNK
    {
        BookStoEntities2  db = new BookStoEntities2();
        public ChiTietPNK(int MaSach, int SoThung, int? SoSachTrongThung, long GiaNhap1Thung)
        {
            this.MaSach = MaSach;
            sach sanpham = db.saches.SingleOrDefault(n => n.MaSach == MaSach);
            this.TenSach = sanpham.TenSach;
            this.Anh = sanpham.Anh;
            this.SoThung = SoThung;
            this.SoSachTrongThung = SoSachTrongThung;
            this.GiaNhap1Thung = GiaNhap1Thung;
        }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string Anh { get; set; }
        public int SoThung { get; set; } // số lượng thùng nhập về
        public int? SoSachTrongThung { get; set; }
        public long GiaNhap1Thung { get; set; }
        public long ThanhTien
        {
            get { return GiaNhap1Thung * SoThung; }
        }

    }
}