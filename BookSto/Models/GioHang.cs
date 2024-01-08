using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class GioHang
    {
        BookStoEntities2 db = new BookStoEntities2();
        public GioHang(int MaThung)
        {
            this.ts = db.thungsaches.SingleOrDefault(n => n.MaThung == MaThung);
            this.GiaThung = ts.sach.GiaTriSach * ts.SoLuongSach - ts.sach.GiaTriSach * ts.SoLuongSach * ts.sach.GiamGia / 100; // giá bán 1 thùng sách bằng: giá sách * số sách trong thùng
            this.TonKho = db.thungsaches.Where(n => n.MaSach == ts.MaSach && n.SoLuongSach == ts.SoLuongSach).ToList().Count();
            this.SoThung = 1;
        }
        public thungsach ts { get; set; }
        public double? GiaThung { get; set; }
        public int TonKho { get; set; }
        public int SoThung { get; set; }
        public double? ThanhTien
        {
            get { return GiaThung * SoThung; }
        }
    }
}