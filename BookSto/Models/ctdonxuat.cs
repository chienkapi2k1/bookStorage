//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookSto.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ctdonxuat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ctdonxuat()
        {
            this.thungsaches = new HashSet<thungsach>();
        }
    
        public int MaCTDonXuat { get; set; }
        public int MaDB { get; set; }
        public int MaSach { get; set; }
        public int SoLuong { get; set; }
        public int GiaXuat { get; set; }
    
        public virtual donxuat donxuat { get; set; }
        public virtual sach sach { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<thungsach> thungsaches { get; set; }
    }
}
