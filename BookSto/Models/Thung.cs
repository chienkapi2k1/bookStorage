using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class Thung
    {
        public Thung(sach s, int? mathung, int maSach, int? sosachtrongthung, int tongsothung, string tenSach, string tacgia, double? gia, string anh, double? giamgia)
        {
            Sach = s;
            MaThung = mathung;
            TongSoThung = tongsothung;
            SoSachTrongThung = sosachtrongthung;
            MaSach = maSach;
            TenSach = tenSach;
            TacGia = tacgia;
            Gia = gia;
            Anh = anh;
            GiamGia = giamgia;
        }
        public sach Sach  { get; set; }
        public int? MaThung { get; set; }
        public int TongSoThung { get; set; }
        public int? SoSachTrongThung { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string TacGia { get; set; }
        public double? Gia { get; set; }
        public string Anh { get; set; }
        public double? GiamGia { get; set; }
    }
}