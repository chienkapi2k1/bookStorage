using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class DSSach 
    {
        public DSSach(int maSach, string theLoai, string tacGia, string nXB, string tenSach, string tomTat, int? tongSoTrang, int? soLuong, int? tap, int? tongSoTap, double? giaTriSach, double? giamGia, string gioiThieu, int? trangThai, string anh)
        {
            MaSach = maSach;
            TheLoai = theLoai;
            TacGia = tacGia;
            NXB = nXB;
            TenSach = tenSach;
            TomTat = tomTat;
            TongSoTrang = tongSoTrang;
            SoLuong = soLuong;
            Tap = tap;
            TongSoTap = tongSoTap;
            GiaTriSach = giaTriSach;
            GiamGia = giamGia;
            GioiThieu = gioiThieu;
            TrangThai = trangThai;
            Anh = anh;
        }

        public int MaSach { get; set; }
        public string TheLoai { get; set; }
        public string TacGia { get; set; }
        public string NXB { get; set; }
        public string TenSach { get; set; }
        public string TomTat { get; set; }
        public Nullable<int> TongSoTrang { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<int> Tap { get; set; }
        public Nullable<int> TongSoTap { get; set; }
        public Nullable<double> GiaTriSach { get; set; }
        public Nullable<double> GiamGia { get; set; }
        public string GioiThieu { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public string Anh { get; set; }
    }
}