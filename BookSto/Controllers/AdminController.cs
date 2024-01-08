using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BookSto.Models;
using System.Net;
using System.Web.WebPages;
using System.Data.Entity.Validation;


namespace BookSto.Controllers
{
    public class AdminController : Controller
    {
        BookStoEntities2 db = new BookStoEntities2();

        #region Đăng Nhập Đăng Ký Đăng Xuất

        [HttpGet]
        public ActionResult DangKy(int? nv)
        {
            TempData["nv"] = nv;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(nguoidung khachhang, FormCollection f)
        {
            if (ModelState.IsValid)     // validate thỏa mãn mới đưa vào db
            {
                // check tai khoan ton tai
                List<TaiKhoan> checktaikhoan = new List<TaiKhoan>();
                var D = db.nguoidungs.Select(s => s.TaiKhoan).Distinct().ToList();
                foreach (var item in D)
                {
                    checktaikhoan.Add(new TaiKhoan(item));      // lay danh sach tai khoan
                }
                foreach (var item in checktaikhoan)
                {
                    if (khachhang.TaiKhoan == item.taikhoan)
                    {
                        ViewBag.thongbaotk = "- Tên đăng nhập đã tồn tại !!!";
                        return View();
                    }
                }
                string xnmatkhau = f["XacnhanMatKhau"].ToString();
                string matkhau = f["MatKhau"].ToString();
                if (xnmatkhau != matkhau)
                {
                    ViewBag.thongbaomk = "- Xác nhận mật khẩu không khớp !! Vui lòng nhập lại";
                    return View();
                }

                if(TempData["nv"] as int? == 1)
                {
                    khachhang.Role = "NHANVIEN";
                }
                else
                {
                    khachhang.Role = "KHACHHANG";
                }
                khachhang.TrangThai = 1; // hoạt động 
                khachhang.GioiTinh = 1;
                db.nguoidungs.Add(khachhang);
                db.SaveChanges();
                if(TempData["nv"] as int? == 1)
                {
                    return RedirectToAction("BangDieuKhien", "Admin");
                }
                return RedirectToAction("DangNhap", "Admin");
            }
            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string taikhoan = f["TaiKhoan"].ToString();
            string matkhau = f["MatKhau"].ToString();
            nguoidung kh = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == taikhoan && n.MatKhau == matkhau && n.TrangThai == 1);
            if (kh != null)
            {
                ViewBag.ThongBao = "Đăng nhập thành công!!!";
                set_sestion(kh);
                if (kh.Role == "ADMIN")
                {
                    return RedirectToAction("BangDieuKhien", "Admin");
                }                
                if (kh.Role == "NHANVIEN")
                {
                    return RedirectToAction("DSSach", "Admin");
                }
                return RedirectToAction("TrangChu", "Sach");
            }
            ViewBag.DangNhapThatBai = "Đăng nhập thất bại! Vui lòng kiểm tra lại Tài khoản và Mật khẩu của bạn";
            return View();
        }

        public void set_sestion(nguoidung kh)
        {
            Session["Role"] = kh.Role;
            Session["MaNguoiDung"] = kh.MaNguoiDung;
            Session["TaiKhoan"] = kh.TaiKhoan;
            Session["HoTen"] = kh.HoTen;
            Session["TrangThai"] = kh.TrangThai;
            Session["DiaChi"] = kh.DiaChi;
            Session["SDT"] = kh.SDT;
            Session["Email"] = kh.Email;
        }
        public ActionResult DangXuat()
        {
            if (Session["TaiKhoan"] != null)
            {
                Session.Clear();
                Session.RemoveAll();
                return RedirectToAction("DangNhap", "Admin");
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["TaiKhoan"] != null)
            {
                var s = Session["TaiKhoan"].ToString();
                nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == s && n.TrangThai == 1); // V
                return View(nguoidung);
            }
            return RedirectToAction("DangNhap", "Admin");
        }

        [HttpGet]
        public ActionResult ChinhSuaTaiKhoan()
        {
            if(Session["MaNguoiDung"] != null)
            {
                int? id = Session["MaNguoiDung"] as int?;
                nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.MaNguoiDung == id); // V 
                return View(nguoidung);
            }
            return RedirectToAction("DangNhap", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaTaiKhoan(nguoidung nd)
        {
            if (Session["TaiKhoan"] != null)
            {
                var s = Session["TaiKhoan"].ToString();
                nguoidung nguoidung_db = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == s); // V
                nguoidung_db.HoTen = nd.HoTen;
                nguoidung_db.GioiTinh = nd.GioiTinh;
                nguoidung_db.Tuoi = nd.Tuoi;
                nguoidung_db.Email = nd.Email;
                nguoidung_db.SDT = nd.SDT;
                nguoidung_db.DiaChi = nd.DiaChi;
                db.Entry(nguoidung_db).State = EntityState.Modified;
                set_sestion(nguoidung_db);
                db.SaveChanges();
                switch (Session["Role"].ToString())
                {
                    case "ADMIN":
                        return RedirectToAction("BangDieuKhien", "Admin");
                    case "NHANVIEN":
                        return RedirectToAction("DSSach", "Admin");
                    case "KHACHHANG":
                        return RedirectToAction("TrangChu", "Sach");
                }
            }
            return View(nd);
        }
        public ActionResult DoiMatKhau()
        {
            if (Session["TaiKhoan"] != null)
            {
                return View();
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(MatKhau matkhau)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (ModelState.IsValid)
                {
                    var a = Session["TaiKhoan"].ToString();
                    nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == a); // V
                    if (matkhau.oldPassword == nguoidung.MatKhau && matkhau.oldPassword != "" && matkhau.newPassword != "")
                    {
                        nguoidung.MatKhau = matkhau.newPassword;
                        db.Entry(nguoidung).Property(u => u.MatKhau).IsModified = true;// Lưu thay đổi chỉ cho trường mật khẩu
                        db.SaveChanges();
                        return RedirectToAction("ThongTinTaiKhoan", "NguoiDung");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật Khẩu cũ sai");
                        return View();
                    }
                }
                return View();
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        public ActionResult DSTaiKhoan()
        {
            if (Session["TaiKhoan"] != null && Session["Role"].ToString() == "ADMIN")
            {
                List<nguoidung> lst_tk = db.nguoidungs.OrderBy(n => n.Role == "ADMIN" ? 1 : n.Role == "NHANVIEN" ? 2 : n.Role == "KHACHHANG" ? 3 : 0).ToList();
                return View(lst_tk);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        #endregion

        #region Bảng điều khiển
        public ActionResult BangDieuKhien()
        {
            if (Session["Role"] != null && Session["Role"].ToString() == "ADMIN")
            {
                List<nguoidung> kh = db.nguoidungs.Where(n => n.Role == "KHACHHANG").ToList();
                TempData["kh"] = kh.Count();
                List<sach> s = db.saches.Where(n => n.TrangThai == 1).ToList();
                TempData["s"] = s.Count();
                List<donxuat> dx = db.donxuats.Where(n => n.TrangThai == 4).ToList();
                TempData["dh"] = dx.Count();
                long ds_month_now = 0;
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                dx = dx.Where(n => n.NgayTao >= firstDayOfMonth && n.NgayTao <= lastDayOfMonth && n.TrangThai == 4).ToList();
                foreach (donxuat item in dx)
                {
                    ds_month_now += (long)item.TongTien;
                }
                TempData["doanhso"] = FormatNumber(ds_month_now);
                List<donnhap> dn = db.donnhaps.Where(n => n.NgayTao >= firstDayOfMonth && n.NgayTao <= lastDayOfMonth && n.TrangThai == 1).ToList();
                double chiphi = 0;
                foreach (donnhap item in dn)
                {
                    chiphi += (long)item.TongTien;
                }
                TempData["chiphi"] = Math.Round(chiphi / ds_month_now * 100, 1);
                TempData["loinhuan"] = Math.Round((ds_month_now - chiphi) / ds_month_now * 100, 1);
                DateTime month_ago_1 = firstDayOfMonth.AddMonths(-1);
                DateTime month_ago_30 = lastDayOfMonth.AddMonths(-1);

                DateTime week_now_7 = now;
                DateTime week_now_1 = week_now_7.AddDays(-7);

                DateTime week_ago_7 = week_now_1;
                DateTime week_ago_1 = week_ago_7.AddDays(-7);

                DateTime day_now_24 = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
                DateTime day_now_1 = day_now_24.AddDays(-1);

                DateTime day_ago_24 = day_now_1;
                DateTime day_ago_1 = day_ago_24.AddDays(-1);
                long ds_month_ago = 0;
                long ds_week_now = 0;
                long ds_week_ago = 0;
                long ds_day_now = 0;
                long ds_day_ago = 0;
                List<donxuat> dx_month = db.donxuats.Where(n => n.TrangThai != 1 && n.TrangThai != 5).ToList();
                foreach (donxuat item in dx_month)
                {
                    if (item.NgayTao >= month_ago_1 && item.NgayTao <= month_ago_30)
                    {
                        ds_month_ago += (long)item.TongTien;
                    }
                    if (item.NgayTao >= week_now_1 && item.NgayTao <= week_now_7)
                    {
                        ds_week_now += (long)item.TongTien;
                    }
                    if (item.NgayTao >= week_ago_1 && item.NgayTao <= week_ago_7)
                    {
                        ds_week_ago += (long)item.TongTien;
                    }
                    if (item.NgayTao >= day_now_1 && item.NgayTao <= day_now_24)
                    {
                        ds_day_now += (long)item.TongTien;
                    }
                    if (item.NgayTao >= day_ago_1 && item.NgayTao <= day_ago_24)
                    {
                        ds_day_ago += (long)item.TongTien;
                    }
                }
                Tuple<decimal, long, int> month = percent(ds_month_ago, ds_month_now);
                Tuple<decimal, long, int> week = percent(ds_week_ago, ds_week_now);
                Tuple<decimal, long, int> day = percent(ds_day_ago, ds_day_now);

                TempData["month"] = month;
                TempData["week"] = week;
                TempData["day"] = day;
                List<donxuat> ds_donxuat = db.donxuats.Include(n => n.ctdonxuats)
                    .OrderBy(n => n.TrangThai)
                    .ThenByDescending(n => n.MaDB).Take(10).ToList();
                return View(ds_donxuat);
            }
            return RedirectToAction("TrangChu", "Sach");
        }
        public Tuple<decimal, long, int> percent(long soCu, long soMoi)
        {
            decimal percent = 0;
            decimal kq = 0;
            long chenhlech = Math.Abs(soCu - soMoi);
            if (soCu == 0)
            {
                return new Tuple<decimal, long, int>(100, chenhlech, 1);
            }
            if (soMoi == 0)
            {
                return new Tuple<decimal, long, int>(100, chenhlech, - 1);
            }
            percent =(decimal)(soMoi - soCu) /(decimal)soCu * 100;
            kq = Math.Round(percent, 1);
            if (soMoi > soCu) // ds tăng
            {
                return new Tuple<decimal, long, int>(kq, chenhlech, 1);
            }
            return new Tuple<decimal, long, int>(kq, chenhlech, -1);
        }

        public string FormatNumber(long number)
        {
            if (number >= 1000000000)
            {
                return (number / 1000000000.0).ToString("0.##") + " B";
            }
            else if (number >= 1000000)
            {
                return (number / 1000000.0).ToString("0.##") + " M";
            }
            else if (number >= 1000)
            {
                return (number / 1000.0).ToString("0.##") + " K";
            }
            else
            {
                return number.ToString();
            }
        }

        #endregion
        
        #region Quản lí nhà xuất bản 

        public ActionResult DSNXB(string TenNXB)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    //timkiem
                    if (TenNXB != null && TenNXB != "")
                    {
                        TenNXB = TenNXB.Trim();
                        ViewBag.TenNXB = TenNXB.Trim();
                    }
                    else
                    {
                        TenNXB = "";
                    }
                    List<nhaxb> lstnxb = db.nhaxbs.Where(n => n.TenNXB.Contains(TenNXB)).ToList();
                    return View(lstnxb);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpGet]
        public ActionResult ThemNXB()
        {
            if (Session["Role"] != null)
            {
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNXB(nhaxb nxb)
        {
            if (ModelState.IsValid)
            {
                db.nhaxbs.Add(nxb);
                db.SaveChanges();
                return RedirectToAction("DSNXB", "Admin");
            }
            return View();
        }

        public ActionResult ChinhSuaNXB(int? MaNXB)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaNXB.ToString() != null))
            {
                if (MaNXB == null)
                {
                    return HttpNotFound();
                }
                nhaxb nxb = db.nhaxbs.Find(MaNXB);
                if (nxb == null)
                {
                    return HttpNotFound();
                }
                return View(nxb);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaNXB([Bind(Include = "MaNXB, TenNXB")] nhaxb nxb)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nxb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSNXB", "Admin");
            }
            return RedirectToAction("ChinhSuaNXB", "Admin");
        }

        #endregion

        #region Quản lí thể loại

        public ActionResult DSTheLoai(string TenTheLoai)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    //timkiem
                    if (TenTheLoai != null && TenTheLoai != "")
                    {
                        TenTheLoai = TenTheLoai.Trim();
                        ViewBag.TenTheLoai = TenTheLoai.Trim();
                    }
                    else
                    {
                        TenTheLoai = "";
                    }
                    List<theloai> lstTheLoai = db.theloais.Where(n => n.TenTheLoai.Contains(TenTheLoai)).OrderByDescending(n => n.TrangThai).ThenByDescending(n => n.MaTheLoai).ToList();
                    ViewBag.ListTheLoai = lstTheLoai;
                    return View(lstTheLoai);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpGet]
        public ActionResult ThemTheLoai()
        {
            if (Session["Role"] != null)
            {
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemTheLoai(theloai theloai)
        {
            if (ModelState.IsValid)
            {
                theloai.TrangThai = 1;
                db.theloais.Add(theloai);
                db.SaveChanges();
                return RedirectToAction("DSTheLoai", "Admin");
            }
            return View();
        }

        public ActionResult ChinhSuaTheLoai(int? MaTheLoai)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaTheLoai.ToString() != null))
            {
                if (MaTheLoai == null)
                {
                    return HttpNotFound();
                }
                theloai theloai = db.theloais.Find(MaTheLoai);
                if (theloai == null)
                {
                    return HttpNotFound();
                }
                return View(theloai);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaTheLoai([Bind(Include = "MaTheLoai, TenTheLoai, TrangThai, GioiThieu")] theloai theloai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(theloai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSTheLoai", "Admin");
            }
            return RedirectToAction("ChinhSuaTheLoai", "Admin");
        }

        #endregion

        #region Quản lí tác giả
        public ActionResult DSTacGia(string TenTacGia)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    //timkiem
                    if (TenTacGia != null && TenTacGia != "")
                    {
                        TenTacGia = TenTacGia.Trim();
                        ViewBag.TenTacGia = TenTacGia.Trim();
                    }
                    else
                    {
                        TenTacGia = "";
                    }
                    List<tacgia> lstTacGia = db.tacgias.Where(n => n.TenTacGia.Contains(TenTacGia)).OrderByDescending(n => n.TrangThai).ThenByDescending(n => n.MaTacGia).ToList();
                    ViewBag.ListTacGia = lstTacGia;
                    return View(lstTacGia);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpGet]
        public ActionResult ThemTacGia()
        {
            if (Session["Role"] != null)
            {
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemTacGia(tacgia tacgia)
        {
            if (ModelState.IsValid)
            {
                tacgia.TrangThai = 1;
                db.tacgias.Add(tacgia);
                db.SaveChanges();
                return RedirectToAction("DSTacGia", "Admin");
            }
            return View();
        }

        public ActionResult ChinhSuaTacGia(int? MaTacGia)
        {
            if (Session["Role"] != null || MaTacGia.ToString() != null)
            {
                if (MaTacGia == null)
                {
                    return HttpNotFound();
                }
                tacgia tacgia = db.tacgias.Find(MaTacGia);
                if (tacgia == null)
                {
                    return HttpNotFound();
                }
                return View(tacgia);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaTacgia([Bind(Include = "MaTacGia, TenTacGia, TrangThai, GioiThieu")] tacgia tacgia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tacgia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSTacGia", "Admin");
            }
            return RedirectToAction("ChinhSuaTacGia", "Admin");
        }

        #endregion

        #region Quản Lý Sách
        public ActionResult DSSach(string tukhoa, string tentacgia) // từ khóa tìm kiếm mã sách hoặc tên sách
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    var dssach = db.saches.Select(n => n).ToList();
                    var dstacgia = db.tacgias.Select(n => n).ToList();
                    var dstheloai = db.theloais.Select(n => n).ToList();
                    var dsnxb = db.nhaxbs.Select(n => n).ToList();
                    var lstsach = (from s in dssach
                                   join tg in dstacgia on s.MaTacGia equals tg.MaTacGia
                                   join tl in dstheloai on s.MaTheLoai equals tl.MaTheLoai
                                   join nxb in dsnxb on s.MaNXB equals nxb.MaNXB
                                   select new
                                   {
                                       MaSach = s.MaSach,
                                       TheLoai = tl.TenTheLoai,
                                       TacGia = tg.TenTacGia,
                                       NXB = nxb.TenNXB,
                                       s.TenSach,
                                       s.TomTat,
                                       s.TongSoTrang,
                                       s.SoLuong,
                                       s.Tap,
                                       s.TongSoTap,
                                       s.GiaTriSach,
                                       s.GiamGia,
                                       s.GioiThieu,
                                       s.TrangThai,
                                       s.Anh,
                                   });
                    // thêm các sách vào list để đưa ra view
                    List<DSSach> lstsach_model = new List<DSSach>();
                    foreach (var item in lstsach)
                    {
                        DSSach ok = new DSSach(item.MaSach, item.TheLoai, item.TacGia, item.NXB, item.TenSach, item.TomTat, item.TongSoTrang, item.SoLuong, item.Tap, item.TongSoTap, item.GiaTriSach, item.GiamGia, item.GioiThieu, item.TrangThai, item.Anh);
                        lstsach_model.Add(ok);
                    }
                    ViewBag.lstsach = lstsach_model;
                    // lấy dữ liệu select option cho ô tìm kiếm
                    List<TenTacGia> tacGias = new List<TenTacGia>();
                    var lstTacgia = db.tacgias.Select(s => s.TenTacGia).Distinct().ToList();
                    foreach (var item in lstTacgia)
                    {
                        tacGias.Add(new TenTacGia(item));
                    }
                    ViewBag.ListTacGia = tacGias; // ds đưa vào select option

                    //timkiem
                    if ((tukhoa != null && tukhoa != "") || (tentacgia != null && tentacgia != ""))
                    {
                        tukhoa = tukhoa.Trim();
                        ViewBag.tukhoa = tukhoa.Trim();
                        tentacgia = tentacgia.Trim();
                        if (tentacgia == "-")
                        {
                            tentacgia = "";
                        }
                        TempData["TacGia"] = tentacgia.Trim();
                        if (tukhoa.IsInt()) // nếu từ khóa là số thì where mã ngược lại thì where tên sách
                        {
                            lstsach_model = lstsach_model
                                .Where(n => n.MaSach.ToString().Contains(tukhoa) && n.TacGia.Contains(tentacgia))
                                .OrderByDescending(n => n.MaSach).ToList();
                            return View(lstsach_model);
                        }
                        lstsach_model = lstsach_model
                            .Where(n => n.TenSach.Contains(tukhoa) && n.TacGia.Contains(tentacgia))
                            .ToList();
                    }
                    lstsach_model = lstsach_model.OrderByDescending(n => n.TrangThai).ThenByDescending(n => n.MaSach).ToList();
                    return View(lstsach_model);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpGet]
        public ActionResult ThemSach()
        {
            if (Session["Role"] != null)
            {
                List<tacgia> lst_tacgia = db.tacgias.Where(n => n.TrangThai == 1).OrderByDescending(n => n.MaTacGia).ToList();
                List<theloai> lst_theloai = db.theloais.Where(n => n.TrangThai == 1).OrderByDescending(n => n.MaTheLoai).ToList();
                List<nhaxb> lst_nhaxb = db.nhaxbs.OrderByDescending(n => n.MaNXB).ToList();
                ViewBag.tacgia = lst_tacgia;
                ViewBag.theloai = lst_theloai;
                ViewBag.nhaxb = lst_nhaxb;
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSach([Bind(Include = "MaSach, TenSach, MaTacGia, MaTheLoai, MaNXB, TomTat, TongSoTrang, SoLuong, Tap, TongSoTap, GiaTriSach, GiamGia, GioiThieu, TrangThai, Anh")] sach sach)
        {
            if (ModelState.IsValid)
            {
                sach.TrangThai = 1;
                db.saches.Add(sach);
                db.SaveChanges();
                return RedirectToAction("DSSach", "Admin");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ChinhSuaSach(int? MaSach)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaSach.ToString() != null))
            {
                if (MaSach == null)
                {
                    return HttpNotFound();
                }
                sach sach = db.saches.Where(n => n.MaSach == MaSach).FirstOrDefault(); // Find
                if (sach == null)
                {
                    return HttpNotFound();
                }
                List<tacgia> lst_tacgia = db.tacgias.Where(n => n.TrangThai == 1).OrderByDescending(n => n.MaTacGia).ToList();
                List<theloai> lst_theloai = db.theloais.Where(n => n.TrangThai == 1).OrderByDescending(n => n.MaTheLoai).ToList();
                List<nhaxb> lst_nhaxb = db.nhaxbs.OrderByDescending(n => n.MaNXB).ToList();
                ViewBag.tacgia1 = lst_tacgia;
                ViewBag.theloai1 = lst_theloai;
                ViewBag.nhaxb1 = lst_nhaxb;
                return View(sach);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaSach([Bind(Include = "MaSach, TenSach, MaTacGia, MaTheLoai, MaNXB, TomTat, TongSoTrang, SoLuong, Tap, TongSoTap, GiaTriSach, GiamGia, GioiThieu, TrangThai, Anh")] sach sach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSSach", "Admin");
            }
            return RedirectToAction("ChinhSuaSach", "Admin");
        }

        #endregion

        #region ViTri
        public ActionResult DSViTri(string TuKhoa)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    // lấy và add những vị trí khả dụng (có mã thùng)
                    var vitri = db.vitris.Select(n => n).ToList();
                    var thung = db.thungsaches.Select(n => n).ToList();
                    var sach = db.saches.Select(n => n).ToList();
                    var ViTriCoMaThung = (from vt in vitri
                                          join t in thung on vt.MaThung equals t.MaThung    // một vị trí khả dụng khi nó có mã thùng
                                          join s in sach on t.MaSach equals s.MaSach
                                          select new
                                          {
                                              vt.MaViTri,
                                              vt.MaThung,
                                              TenThung = s.TenSach,
                                              t.SoLuongSach,
                                              vt.MaViTriCha,
                                              vt.TenViTri,
                                              vt.LoaiViTri,
                                              vt.TrangThai
                                          }
                                    ).ToList();
                    var ds_ten = ViTriCoMaThung;
                    List<DSViTri> DS_ViTri = new List<DSViTri>();
                    // lấy và add những vị trí không khả dụng để chứa thùng
                    var ViTriMaThungNull = db.vitris.Where(n => n.MaThung == null).OrderByDescending(n => n.LoaiViTri).ToList();
                    foreach (var item in ViTriMaThungNull)
                    {
                        DSViTri ok = new DSViTri(item.MaViTri, item.MaThung, null, null, item.MaViTriCha, item.TenViTri, item.LoaiViTri, item.TrangThai);
                        DS_ViTri.Add(ok);
                    }
                    // add những vị trí trống
                    foreach (var item in ViTriCoMaThung)
                    {
                        DSViTri ok = new DSViTri(item.MaViTri, item.MaThung, item.TenThung, item.SoLuongSach, item.MaViTriCha, item.TenViTri, item.LoaiViTri, item.TrangThai);
                        DS_ViTri.Add(ok);
                    }
                    if (TuKhoa != null && TuKhoa != "")
                    {
                        TuKhoa = TuKhoa.Trim();
                        ViewBag.TuKhoa = TuKhoa.Trim();
                        DS_ViTri = DS_ViTri.Where(n => (n.MaThung != null && n.MaThung.ToString().ToLower().Contains(TuKhoa.ToLower())
                        || n.TenThung != null && n.TenThung.ToLower().Contains(TuKhoa.ToLower()))
                            || n.TenViTri.ToLower().Contains(TuKhoa.ToLower())
                            || n.MaViTri.ToString().ToLower().Contains(TuKhoa.ToLower()))
                            .OrderByDescending(n => n.LoaiViTri)
                            .ThenBy(n => n.MaViTri).ToList(); // ds đầy đủ vị trí
                    }
                    // sắp xếp lại theo loại : nhà -> kệ -> hàng -> ô
                    DS_ViTri = DS_ViTri.Where(n=>n.TrangThai != 2).OrderByDescending(v => v.TrangThai == 1 ? 1 : 0).ThenBy(n => n.TrangThai).ThenBy(n => n.TenViTri).ThenBy(n => n.MaViTriCha).ThenBy(n => n.MaViTri).ToList(); // ds đầy đủ vị trí
                    return View(DS_ViTri);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }
        [HttpGet]
        public ActionResult ThemViTri()
        {
            if (Session["Role"] != null)
            {
                //lấy ds mã thùng đưa vào ô select
                var thung = db.thungsaches.Select(n => n).ToList();
                var sach = db.saches.Select(n => n).Where(n => n.TrangThai == 1).ToList();
                var lstThung = (from t in thung
                                join s in sach on t.MaSach equals s.MaSach
                                select new
                                {
                                    t.MaThung,
                                    t.MaSach,
                                    s.TenSach
                                })
                                .ToList();
                List<SachThung> select_sachthung = new List<SachThung>();
                foreach (var item in lstThung)
                {
                    SachThung ok = new SachThung(item.MaThung, item.MaSach, item.TenSach);
                    select_sachthung.Add(ok);
                }
                TempData["thungsach"] = select_sachthung;
                // lấy ds mã vị trí cha và tên
                List<vitri> lst_ViTriCha = db.vitris
                    .Where(n => n.TrangThai == 2)   // trạng thái : 0 là trống, 1 là đầy, 2 là không khả dụng
                    .OrderBy(n => n.LoaiViTri)
                    .ThenBy(n => n.MaViTri)
                    .ToList();
                TempData["vitricha"] = lst_ViTriCha;
                // lấy ds tên theo loại vị trí đã có
                List<int?> lst_LoaiViTri = db.vitris.Select(n => n.LoaiViTri).Distinct().ToList();
                TempData["LoaiViTri"] = lst_LoaiViTri;
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        public ActionResult AddSlot()
        {
            List<vitri> lst_vt1 = db.vitris.Where(n=>n.LoaiViTri != 4).ToList();
            List<vitri> lst_vt2 = db.vitris.ToList();
            foreach (vitri item in lst_vt1)
            {
                switch (item.LoaiViTri)
                {
                    case 1:
                        for (int i = 1; i <= 10; i++)
                        {
                            int ok = 1; // 0 = đã tồn tại , 1 = chưa tồn tại => add dc
                            foreach (vitri check in lst_vt2)
                            {
                                if (check.Ke == i && check.MaViTriCha == item.MaViTri)
                                {
                                    ok = 0;
                                }
                            }
                            if(ok == 1)
                            {
                                vitri add = new vitri();
                                add.MaViTriCha = item.MaViTri;
                                add.LoaiViTri = item.LoaiViTri + 1;
                                add.Nha = item.Nha;
                                add.Ke = i;
                                add.TrangThai = 2;
                                add.TenViTri = item.TenViTri + " Kệ " + i;
                                db.vitris.Add(add);
                            }
                        }
                        break;
                    case 2:
                        for (int i = 1; i <= 8; i++)
                        {
                            int ok = 1; // 0 = đã tồn tại , 1 = chưa tồn tại => add dc
                            foreach (vitri check in lst_vt2)
                            {
                                if (check.Tang == i && check.MaViTriCha == item.MaViTri)
                                {
                                    ok = 0;
                                }
                            }
                            if (ok == 1)
                            {
                                vitri add = new vitri();
                                add.MaViTriCha = item.MaViTri;
                                add.LoaiViTri = item.LoaiViTri + 1;
                                add.Nha = item.Nha;
                                add.Ke = item.Ke;
                                add.Tang = i;
                                add.TrangThai = 2;
                                add.TenViTri = item.TenViTri + " Tầng " + i;
                                db.vitris.Add(add);
                            }
                        }
                        break;
                    case 3:
                        for (int i = 1; i <= 9; i++)
                        {
                            int ok = 1; // 0 = đã tồn tại , 1 = chưa tồn tại => add dc
                            foreach (vitri check in lst_vt2)
                            {
                                if (check.O == i && check.MaViTriCha == item.MaViTri)
                                {
                                    ok = 0;
                                    break;
                                }
                            }
                            if (ok == 1)
                            {
                                vitri add = new vitri();
                                add.MaViTriCha = item.MaViTri;
                                add.LoaiViTri = item.LoaiViTri + 1;
                                add.Nha = item.Nha;
                                add.Ke = item.Ke;
                                add.Tang = item.Tang;
                                add.O = i;
                                add.TrangThai = 0;
                                add.TenViTri = item.TenViTri + " Ô " + i;
                                db.vitris.Add(add);
                            }
                        }
                        break;
                }
                
            }
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var validationError in error.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }
            }
            return RedirectToAction("DSViTri", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemViTri(vitri new_vitri)
        {
            if (ModelState.IsValid)
            {
                // lấy tên cho vị trí và set các trường
                List<vitri> lst_ViTriCha = db.vitris
                    .Where(n => n.TrangThai == 2)   // trạng thái : 0 là trống, 1 là đầy, 2 là không khả dụng
                    .OrderBy(n => n.LoaiViTri)
                    .ThenBy(n => n.MaViTri)
                    .ToList();
                new_vitri.TenViTri = new_vitri.O.ToString().Trim();
                vitri vtcha = db.vitris.SingleOrDefault(n => n.MaViTri == new_vitri.MaViTriCha);
                if (new_vitri.MaViTriCha != null)
                {
                    new_vitri.LoaiViTri = vtcha.LoaiViTri + 1;
                    foreach (vitri item in lst_ViTriCha)
                    {
                        if (new_vitri.MaViTriCha == item.MaViTri)
                        {
                            switch (new_vitri.LoaiViTri)
                            {
                                case 2:
                                    new_vitri.Nha = item.Nha;
                                    new_vitri.Ke = new_vitri.TenViTri.AsInt();
                                    new_vitri.TenViTri = item.TenViTri + " Kệ " + new_vitri.Ke;
                                    break;
                                case 3:
                                    new_vitri.Nha = item.Nha;
                                    new_vitri.Ke = item.Ke;
                                    new_vitri.Tang = new_vitri.TenViTri.AsInt();
                                    new_vitri.TenViTri = item.TenViTri + " Tầng " + new_vitri.Tang;
                                    break;
                                case 4:
                                    new_vitri.Nha = item.Nha;
                                    new_vitri.Ke = item.Ke;
                                    new_vitri.Tang = item.Tang;
                                    new_vitri.O = new_vitri.TenViTri.AsInt();
                                    new_vitri.TenViTri = item.TenViTri + " Ô " + new_vitri.O;
                                    break;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    new_vitri.LoaiViTri = 1;
                    new_vitri.Nha = new_vitri.TenViTri;
                    new_vitri.TenViTri = "Nhà" + " " + new_vitri.Nha.ToUpper();
                }
                // check vị trí tồn tại hay chưa bằng tên
                List<vitri> ds_vitri = db.vitris.Select(n => n).ToList();
                foreach (vitri item in ds_vitri)
                {
                    if (new_vitri.TenViTri == item.TenViTri)
                    {
                        ViewBag.name_ThongBao = "Vị Trí : " + new_vitri.TenViTri + " đã tồn tại";
                        return View(new_vitri);
                    }
                }
                vitri vtmoi = new_vitri;
                db.vitris.Add(vtmoi);
                try
                {
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            Console.WriteLine($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                return RedirectToAction("DSViTri", "Admin");
            }
            return View(new_vitri);
        }

        [HttpGet]
        public ActionResult ChinhSuaViTri(int? MaViTri)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaViTri.ToString() != null))
            {
                //lấy ds mã thùng đưa vào ô select
                var thung = db.thungsaches.Select(n => n).ToList();
                var sach = db.saches.Select(n => n).Where(n => n.TrangThai == 1).ToList();
                var lstThung = (from t in thung
                                join s in sach on t.MaSach equals s.MaSach
                                select new
                                {
                                    t.MaThung,
                                    t.MaSach,
                                    s.TenSach
                                })
                                .ToList();
                List<SachThung> select_sachthung = new List<SachThung>();
                foreach (var item in lstThung)
                {
                    SachThung ok = new SachThung(item.MaThung, item.MaSach, item.TenSach);
                    select_sachthung.Add(ok);
                }
                TempData["thungsach_update"] = select_sachthung;
                // lấy ds mã vị trí cha và tên
                List<vitri> lst_ViTriCha = db.vitris
                    .Where(n => n.TrangThai == 2)   // trạng thái : 0 là trống, 1 là đầy, 2 là k khả dụng
                    .OrderBy(n => n.LoaiViTri)
                    .ThenBy(n => n.MaViTri)
                    .ToList();
                TempData["vitricha_update"] = lst_ViTriCha;
                // lấy ds tên theo loại vị trí đã có
                List<int?> lst_LoaiViTri = db.vitris.Select(n => n.LoaiViTri).Distinct().ToList();
                TempData["LoaiViTri_update"] = lst_LoaiViTri;
                if (MaViTri == null)
                {
                    return HttpNotFound();
                }
                vitri old_vitri = db.vitris.Where(n => n.MaViTri == MaViTri).FirstOrDefault(); // Find
                if (old_vitri == null)
                {
                    return HttpNotFound();
                }
                return View(old_vitri);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaViTri([Bind(Include = "MaViTri, MaThung, MaViTriCha, TenViTri, LoaiViTri, Nha, Ke, Tang, O, TrangThai")] vitri vitri)
        {
            if (ModelState.IsValid)
            {
                // lấy tên cho vị trí và set các trường
                List<vitri> lst_ViTriCha = db.vitris
                    .Where(n => n.TrangThai == 2)   // trạng thái : 0 là trống, 1 là đầy, 2 là không khả dụng
                    .OrderBy(n => n.LoaiViTri)
                    .ThenBy(n => n.MaViTri)
                    .ToList();
                vitri old_vitri = db.vitris.Where(n => n.MaViTri == vitri.MaViTri).FirstOrDefault(); // Find
                old_vitri.MaThung = vitri.MaThung;
                old_vitri.MaViTriCha = vitri.MaViTriCha;
                old_vitri.TenViTri = vitri.TenViTri;
                old_vitri.LoaiViTri = vitri.LoaiViTri;
                old_vitri.Nha = vitri.Nha;
                old_vitri.Ke = vitri.Ke;
                old_vitri.Tang = vitri.Tang;
                old_vitri.O = vitri.O;
                old_vitri.TrangThai = vitri.TrangThai;
                foreach (vitri item in lst_ViTriCha)
                {
                    if (old_vitri.MaViTriCha == item.MaViTri && old_vitri.MaViTriCha != null)
                    {
                        switch (old_vitri.LoaiViTri)
                        {
                            case 2:
                                old_vitri.Nha = item.Nha;
                                old_vitri.TenViTri = item.TenViTri + " Kệ " + old_vitri.Ke;
                                break;
                            case 3:
                                old_vitri.Nha = item.Nha;
                                old_vitri.Ke = item.Ke;
                                old_vitri.TenViTri = item.TenViTri + " Tầng " + old_vitri.Tang;
                                break;
                            case 4:
                                old_vitri.Nha = item.Nha;
                                old_vitri.Ke = item.Ke;
                                old_vitri.Tang = item.Tang;
                                old_vitri.TenViTri = item.TenViTri + " Ô " + old_vitri.O;
                                break;
                        }
                        break;
                    }
                    else
                    {
                        old_vitri.TenViTri = "Nhà " + old_vitri.Nha;
                    }
                }
                // check vị trí tồn tại hay chưa bằng tên
                List<vitri> ds_vitri = db.vitris.Where(x => x.MaViTri != old_vitri.MaViTri).ToList();
                foreach (vitri item in ds_vitri)
                {
                    if (old_vitri.TenViTri == item.TenViTri)
                    {
                        ViewBag.name_ThongBao = "Vị Trí : " + old_vitri.TenViTri + " đã tồn tại";
                        return View(old_vitri);
                    }
                }
                db.Entry(old_vitri).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSViTri", "Admin");
            }
            return RedirectToAction("ChinhSuaViTri", "Admin");
        }

        #endregion

        #region Thùng
        // tráng thái thùng (0: đơn nhập  hàng chưa xác nhận,1: thùng trong kho; 2: thùng đã bán; -1:đơn xuất chưa xác nhận)
        public ActionResult DSThung(string TuKhoa)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    List<thungsach> lstThung = db.thungsaches.OrderBy(n => n.TrangThai).ThenByDescending(n => n.MaThung).ToList();
                    if (TuKhoa != null && TuKhoa != "")
                    {
                        TuKhoa = TuKhoa.Trim();
                        ViewBag.TuKhoa = TuKhoa.Trim();
                        lstThung = lstThung.Where(n => n.MaThung.ToString().ToLower().Contains(TuKhoa.ToLower())
                        || n.sach.TenSach != null && n.sach.TenSach.ToLower().Contains(TuKhoa.ToLower())).ToList(); // ds đầy đủ vị trí
                    }
                    return View(lstThung);
                }
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        #endregion

        #region Phiếu nhập kho
        public ActionResult PhieuNhapKho(string TuKhoa)
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
                {
                    //timkiem
                    if (TuKhoa != null && TuKhoa != "")
                    {
                        TuKhoa = TuKhoa.Trim();
                        ViewBag.TuKhoa = TuKhoa.Trim();
                    }
                    else
                    {
                        TuKhoa = "";
                    }
                    List<donnhap> ds_pnk = db.donnhaps.Include(n => n.ctdonnhaps).Include(n => n.nguoidung).Include(n => n.nguoidung1)
                        .Where(n => n.MaNhanVien.ToString().Contains(TuKhoa) || n.nguoidung1.HoTen.Contains(TuKhoa)
                        || (n.MaKhachHang != null && n.nguoidung.HoTen != null
                        && (n.MaKhachHang.ToString().Contains(TuKhoa) || n.nguoidung.HoTen.Contains(TuKhoa)))
                        || n.MaDN.ToString().Contains(TuKhoa))
                        .OrderByDescending(n => n.TrangThai)
                        .ThenByDescending(n => n.MaDN).ToList();
                    return View(ds_pnk);
                }
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        [HttpGet]
        public ActionResult LapPhieuNhapKho()
        {
            if (Session["Role"] != null && (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN"))
            {
                List<nguoidung> khachhang = db.nguoidungs.Where(n => n.Role == "KHACHHANG").ToList();
                TempData["khachhang"] = khachhang;
                // select chọn sách
                List<sach> lst_sach = db.saches.Where(n => n.TrangThai == 1).Include(n => n.ctdonnhaps).OrderByDescending(n => n.MaSach).ToList();
                TempData["lst_sach"] = lst_sach;
                // tạo ss
                List<ChiTietPNK> lstCTDonNhap = Session["CTDonNhap"] as List<ChiTietPNK>; // khi đã có session thì != null
                if (lstCTDonNhap == null)
                {
                    lstCTDonNhap = new List<ChiTietPNK>();
                    Session["CTDonNhap"] = lstCTDonNhap;
                }
                return View(lstCTDonNhap);
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LapPhieuNhapKho(donnhap dn_new)
        {
            if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
            {
                if (ModelState.IsValid)
                {
                    if (dn_new.MaKhachHang == null)
                    {
                        return View(dn_new);
                    }
                    List<ChiTietPNK> lstCTDonNhap = Session["CTDonNhap"] as List<ChiTietPNK>;
                    if (Session["CTDonNhap"] != null)
                    {
                        Session.Remove("CTDonNhap");
                    }
                    dn_new.TongTien = 0;
                    if (lstCTDonNhap != null)
                    {
                        foreach (ChiTietPNK item in lstCTDonNhap)
                        {
                            dn_new.TongTien += (int?)item.ThanhTien;    // tính tổng tiền đơn nhập
                        }
                        dn_new.NgayTao = DateTime.Now;
                        db.donnhaps.Add(dn_new);
                        db.SaveChanges();
                        foreach (ChiTietPNK item in lstCTDonNhap)
                        {
                            ctdonnhap ctdn = new ctdonnhap();   // thêm ctdn vào db
                            ctdn.MaSach = item.MaSach;
                            ctdn.MaDN = dn_new.MaDN;
                            ctdn.SoLuong = item.SoThung;
                            ctdn.GiaNhap = (int)item.GiaNhap1Thung;
                            db.ctdonnhaps.Add(ctdn);
                            db.SaveChanges();
                            for (int i = 1; i <= item.SoThung; i++)
                            {
                                thungsach thung_moi = new thungsach();// thêm thùng sách mới nhập về vào db
                                thung_moi.MaSach = item.MaSach;
                                thung_moi.MaCTDonNhap = ctdn.MaCTDonNhap;
                                thung_moi.TrangThai = 0; // tráng thái thùng (0: đơn nhập  hàng chưa xác nhận,1: thùng trong kho; 2: thùng đã bán; 3:đơn xuất chưa xác nhận)
                                thung_moi.SoLuongSach = item.SoSachTrongThung;
                                db.thungsaches.Add(thung_moi);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        return View(lstCTDonNhap);
                    }
                    return RedirectToAction("PhieuNhapKho", "Admin");
                }
                return View();
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        public ActionResult ThemCTDN_Session(int MaSach, string Url, int SoThung, int? SoSachTrongThung, long GiaNhap1Thung)
        {
            sach sach = db.saches.Where(n => n.TrangThai == 1).SingleOrDefault(n => n.MaSach == MaSach);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<ChiTietPNK> lstCTDonNhap = Session["CTDonNhap"] as List<ChiTietPNK>;
            ChiTietPNK check = lstCTDonNhap.Find(n => n.MaSach == MaSach);
            if (check == null)
            {
                check = new ChiTietPNK(MaSach, SoThung, SoSachTrongThung, GiaNhap1Thung);
                lstCTDonNhap.Add(check);
            }
            return Redirect(Url);
        }

        public ActionResult XoaCTDN_Session(int MaSach, string Url)
        {
            sach sach = db.saches.Where(n => n.TrangThai == 1).SingleOrDefault(n => n.MaSach == MaSach); //...
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<ChiTietPNK> lstCTDonNhap = Session["CTDonNhap"] as List<ChiTietPNK>;
            //kiểm tra sản phẩm có trong giỏ hàng không
            ChiTietPNK ctpnk = lstCTDonNhap.SingleOrDefault(n => n.MaSach == MaSach);
            if (ctpnk != null)
            {
                lstCTDonNhap.RemoveAll(n => n.MaSach == MaSach);
            }
            return Redirect(Url);
        }

        [HttpGet]
        public ActionResult ChinhSuaPhieuNhapKho(int? MaDN)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaDN.ToString() != null))
            {
                if (MaDN == null)
                {
                    return HttpNotFound();
                }
                donnhap donnhap = db.donnhaps.Where(n => n.MaDN == MaDN).FirstOrDefault();
                if (donnhap == null)
                {
                    return HttpNotFound();
                }
                nguoidung nhanvien = db.nguoidungs.Where(n => n.MaNguoiDung == donnhap.MaNhanVien).FirstOrDefault();
                List<nguoidung> lst_khachhang = db.nguoidungs.Where(n => n.Role == "KHACHHANG").ToList();
                TempData["nhanvien"] = nhanvien;
                TempData["lst_khachhang"] = lst_khachhang;
                List<ctdonnhap> ctpnk = db.ctdonnhaps.Include(n => n.thungsaches).Where(n => n.MaDN == MaDN).ToList();
                ViewBag.ctpnk = ctpnk; //chi tiet phieu nhap
                return View(donnhap);
            }
            return RedirectToAction("DangNhap", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaPhieuNhapKho(donnhap dn)
        {
            if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
            {
                donnhap donnhap = db.donnhaps.Where(n => n.MaDN == dn.MaDN).SingleOrDefault();
                if (dn.TrangThai == 0) // hủy đơn
                {
                    List<thungsach> ds_ts = db.thungsaches.Include(n => n.ctdonnhap).Where(n => n.ctdonnhap.MaDN == donnhap.MaDN)
                        .ToList();
                    db.thungsaches.RemoveRange(ds_ts);
                    db.SaveChanges();
                    List<ctdonnhap> ctdn = db.ctdonnhaps.Where(n => n.MaDN == dn.MaDN).ToList();
                    donnhap.TrangThai = 0;
                    donnhap.MaKhachHang = dn.MaKhachHang;
                    donnhap.GhiChu = dn.GhiChu;
                    db.ctdonnhaps.RemoveRange(ctdn);
                    db.Entry(donnhap).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("PhieuNhapKho", "Admin");
                }
                if (dn.TrangThai == 1) // hoàn thành đơn => thêm hàng vào kho(vị trí)
                {
                    List<vitri> ds_vitritrong = db.vitris.Where(n => n.O != null).Where(n => n.TrangThai == 0)
                        .OrderBy(n => n.MaViTriCha).ThenBy(n => n.TenViTri).ToList(); // ds vị trí trống
                    List<thungsach> ds_ts = db.thungsaches.Include(n => n.ctdonnhap).Where(n => n.ctdonnhap.MaDN == donnhap.MaDN)
                        .ToList();
                    foreach (thungsach ts in ds_ts)
                    {
                        ts.TrangThai = 1;
                        db.Entry(ts).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (ds_ts.Count <= ds_vitritrong.Count)
                    {
                        var zippedList = ds_vitritrong.Zip(ds_ts, (x, y) => new { vitri = x, thung = y }); // foreach 2 list
                        foreach (var item in zippedList)
                        {
                            item.vitri.MaThung = item.thung.MaThung;
                            item.vitri.TrangThai = 1;
                            db.Entry(item.vitri).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        donnhap.MaKhachHang = dn.MaKhachHang;
                        donnhap.GhiChu = dn.GhiChu;
                        donnhap.TrangThai = 1;
                        db.Entry(donnhap).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("PhieuNhapKho", "Admin");
                    }
                    else
                    {
                        return View(donnhap); // vị trí trong kho không đủ chỗ
                    }

                }
                return RedirectToAction("PhieuNhapKho", "Admin");
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        public ActionResult XoaCTPNK(int MaCTDonNhap, string Url)
        {
            ctdonnhap ctdn = db.ctdonnhaps.SingleOrDefault(n => n.MaCTDonNhap == MaCTDonNhap);
            if (ctdn == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<thungsach> lst_ts = db.thungsaches.Where(n => n.MaCTDonNhap == MaCTDonNhap).ToList();
            if (lst_ts != null)
            {
                db.thungsaches.RemoveRange(lst_ts);
            }
            donnhap dn = db.donnhaps.SingleOrDefault(n => n.MaDN == ctdn.MaDN);
            dn.TongTien = dn.TongTien - ctdn.SoLuong * ctdn.GiaNhap;
            db.Entry(dn).State = EntityState.Modified;
            if (ctdn != null)
            {
                db.ctdonnhaps.Remove(ctdn);
            }
            db.SaveChanges();
            return Redirect(Url);
        }

        #endregion

        #region Đơn xuất kho
        public ActionResult DSDonXuat(string search)
        {
            if (Session["TaiKhoan"] != null && (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN"))
            {
                //timkiem
                if (search != null && search != "")
                {
                    search = search.Trim();
                    ViewBag.search = search;
                }
                else
                {
                    search = "";
                }
                List<donxuat> ds_donxuat = db.donxuats.Include(n => n.ctdonxuats)
                    .Where(n => n.MaKhachHang.ToString().Contains(search) || n.nguoidung.HoTen.Contains(search)
                    || (n.MaNhanVien != null
                    && (n.MaNhanVien.ToString().Contains(search) || n.nguoidung1.HoTen.Contains(search)))
                    || n.MaDB.ToString().Contains(search)).OrderBy(n => n.TrangThai)
                    .ThenByDescending(n => n.MaDB).ToList();
                return View(ds_donxuat);
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        public ActionResult NhanDon(int? MaDB)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaDB.ToString() != null))
            {
                donxuat donxuat = db.donxuats.Where(n => n.MaDB == MaDB).FirstOrDefault();
                if (donxuat.MaNhanVien == null)
                {
                    donxuat.MaNhanVien = Session["MaNguoiDung"] as int?;
                    db.Entry(donxuat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("ChinhSuaDonXuat", "Admin", new { MaDB = donxuat.MaDB });
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        [HttpGet]
        public ActionResult ChinhSuaDonXuat(int? MaDB)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaDB.ToString() != null))
            {
                donxuat donxuat = db.donxuats.Where(n => n.MaDB == MaDB).FirstOrDefault();
                if (donxuat == null)
                {
                    return HttpNotFound();
                }
                List<ctdonxuat> ctpxk = db.ctdonxuats.Include(n => n.thungsaches).Where(n => n.MaDB == donxuat.MaDB).ToList();
                ViewBag.ctpxk = ctpxk; //chi tiet phieu xuat
                return View(donxuat);
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        //trạng thái đơn xuất(1: chờ admin duyệt, 5: hủy đơn, 2:admin chấp nhận đơn, 3:đang giao hàng, 4 : đã nhận hàng)
        // tráng thái thùng (0: đơn nhập  hàng chưa xác nhận,1: thùng trong kho; 2: thùng đã bán; -1:đơn xuất chưa xác nhận)
        // trạng thái vị trí : 0 là trống, 1 là đầy, 2 là không khả dụng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaDonXuat(donxuat donxuat)
        {
            if (Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN")
            {
                donxuat dx = db.donxuats.SingleOrDefault(n => n.MaDB == donxuat.MaDB);
                dx.TrangThai = donxuat.TrangThai;
                dx.GhiChu = dx.GhiChu;
                if (dx.TrangThai == 3)
                {
                    foreach (ctdonxuat ct in dx.ctdonxuats)
                    {
                        List<thungsach> lst_t = db.thungsaches.Where(n => ct.MaCTDonXuat == n.MaCTDonXuat).ToList();
                        foreach (thungsach ts in lst_t)
                        {
                            vitri vt = db.vitris.Where(n => n.MaThung == ts.MaThung).FirstOrDefault();
                            ts.TrangThai = 2;
                            vt.TrangThai = 0;
                            vt.MaThung = null;
                            db.Entry(ts).State = EntityState.Modified;
                            db.Entry(vt).State = EntityState.Modified;
                        }
                    }
                }
                if (dx.TrangThai == 5)
                {
                    foreach (ctdonxuat ct in dx.ctdonxuats)
                    {
                        List<thungsach> lst_t = db.thungsaches.Where(n => ct.MaCTDonXuat == n.MaCTDonXuat).ToList();
                        foreach (thungsach ts in lst_t)
                        {
                            ts.TrangThai = 1;
                            ts.MaCTDonXuat = null;
                            db.Entry(ts).State = EntityState.Modified;
                        }
                    }
                    List<ctdonxuat> lst_ct = db.ctdonxuats.Where(n => n.MaDB == dx.MaDB).ToList();
                    db.ctdonxuats.RemoveRange(lst_ct);
                }
                db.Entry(dx).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DSDonXuat", "Admin");
            }
            return RedirectToAction("TrangChu", "Sach");
        }

        public ActionResult InHD(int? MaDB)
        {
            if (Session["Role"] != null && ((Session["Role"].ToString() == "ADMIN" || Session["Role"].ToString() == "NHANVIEN") && MaDB.ToString() != null))
            {
                donxuat donxuat = db.donxuats.Where(n => n.MaDB == MaDB).FirstOrDefault();
                return View(donxuat);
            }
            return RedirectToAction("DangNhap", "Admin");
        }

        #endregion

    }
}