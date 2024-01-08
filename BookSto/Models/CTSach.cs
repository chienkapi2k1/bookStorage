using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class CTSach
    {
        public CTSach (int? mathung, sach sach, int? sosachtrongthung, int tongsothung, string tentacgia, string tennxb, string tentheloai)
        {
            MaThung = mathung ;
            Sach = sach;
            SoSachTrongThung = sosachtrongthung;
            TongSoThung = tongsothung;
            TenTacGia = TenTacGia;
            TenNXB = tennxb;
            TenTheLoai = tentheloai;
        }
        public int? MaThung { get; set; }
        public sach Sach { get; set; }
        public int? SoSachTrongThung { get; set; }
        public int TongSoThung { get; set; }
        public string TenTacGia { get; set; }
        public string TenNXB { get; set; }
        public string TenTheLoai { get; set; }
    }
}