using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSto.Models
{
    public class TenTacGia
    {
        public TenTacGia(string tentacgia)
        {
            this.tentacgia = tentacgia;
        }
        public string tentacgia { get; set; }
    }
}
