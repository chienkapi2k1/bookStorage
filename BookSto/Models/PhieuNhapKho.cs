using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class PhieuNhapKho
    {
/*        public PhieuNhapKho(int maDN, int? maKhachHang, string tenKhachHang, int maNhanVien, string tenNhanVien, DateTime? ngayTao, int? tongTien, string ghiChu, int? trangThai)
        {
            MaDN = maDN;
            MaKhachHang = maKhachHang;
            TenKhachHang = tenKhachHang;
            MaNhanVien = maNhanVien;
            TenNhanVien = tenNhanVien;
            NgayTao = ngayTao;
            TongTien = tongTien;
            GhiChu = ghiChu;
            TrangThai = trangThai;
        }*/

        public ICollection<ChiTietPNK> chiTietPNK { get; set; }
        public nguoidung nhanVien { get; set; }
        public nguoidung khachHang { get; set; }
        public int MaDN { get; set; }
        public Nullable<int> MaKhachHang { get; set; }
        public int MaNhanVien { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> TongTien { get; set; }
        public string GhiChu { get; set; }
        public Nullable<int> TrangThai { get; set; }
    }
}