using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSto.Models
{
    public class MatKhau
    {
        [Required(ErrorMessage = "Mời nhập mật khẩu mới")]
        public String newPassword { get; set; }

        [Required(ErrorMessage = "Mời nhập mật khẩu cũ")]
        public String oldPassword { get; set; }

        [NotMapped]
        [Compare("newPassword", ErrorMessage = " Xác nhận mật khẩu sai")]
        public String confirmPassword { get; set; }
    }
}