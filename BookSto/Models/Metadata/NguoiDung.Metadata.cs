using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookSto.Models
{
    [MetadataTypeAttribute(typeof(NguoiDungMetadata))]
    public partial class nguoidung
    {
        internal sealed class NguoiDungMetadata
        {
            public int MaNguoiDung { get; set; }
            public string TaiKhoan { get; set; }
            public string MatKhau { get; set; }
            public string Role { get; set; }
            [StringLength(255)]
            
            public string HoTen { get; set; }
            public Nullable<int> Tuoi { get; set; }
            public string GioiTinh { get; set; }
            [StringLength(255)]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
            public string Email { get; set; }
            [StringLength(255)]
            
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Không phải là số điện thoại hợp lệ")]
            public string SDT { get; set; }
            [StringLength(255)]
            
            public string DiaChi { get; set; }
            public Nullable<int> TrangThai { get; set; }
            public string GhiChu { get; set; }
        }
    }
}