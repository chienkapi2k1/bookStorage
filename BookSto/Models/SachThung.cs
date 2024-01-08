using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class SachThung
    {
        public SachThung(int maThung, int maSach, string tenSach)
        {
            MaThung = maThung;
            MaSach = maSach;
            TenSach = tenSach;
        }

        public int MaThung { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
    }
}