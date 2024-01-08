using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class TaiKhoan
    {
        public TaiKhoan(string taikhoan)
        {
            this.taikhoan = taikhoan;
        }
        public string taikhoan { get; set; }
    }
}