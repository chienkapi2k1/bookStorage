using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class DSViTri
    {
        public DSViTri(int maViTri, int? maThung, string tenThung, int? soLuongSach, int? maViTriCha, string tenViTri, int? loaiViTri, int? trangThai)
        {
            MaViTri = maViTri;
            MaThung = maThung;
            TenThung = tenThung;
            SoLuongSach = soLuongSach;
            MaViTriCha = maViTriCha;
            TenViTri = tenViTri;
            LoaiViTri = loaiViTri;
            TrangThai = trangThai;
        }

        public int MaViTri { get; set; }
        public Nullable<int> MaThung { get; set; }
        public  string TenThung { get; set; }
        public Nullable<int> SoLuongSach { get; set; }
        public Nullable<int> MaViTriCha { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> LoaiViTri { get; set; }
        public Nullable<int> TrangThai { get; set; }
    }
}