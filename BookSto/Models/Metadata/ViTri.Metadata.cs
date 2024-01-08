using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookSto.Models
{
    [MetadataType(typeof(ViTriMetadata))]
    public partial class vitri
    {
        internal sealed class ViTriMetadata
        {
            public int MaViTri { get; set; }
            public Nullable<int> MaThung { get; set; }
            public Nullable<int> MaViTriCha { get; set; }

            public string TenViTri { get; set; }
            public Nullable<int> LoaiViTri { get; set; }
            public string Nha { get; set; }
            public Nullable<int> Ke { get; set; }
            public Nullable<int> Tang { get; set; }
            /*[Required(ErrorMessage = "Vui lòng nhập tên vị trí")]
            [RegularExpression("^([a-zA-Z0-9{1}])$", ErrorMessage = "Định dạng không hợp lệ.")]*/
            public Nullable<int> O { get; set; } // dùng tạm
            public Nullable<int> TrangThai { get; set; }
        }
    }
}