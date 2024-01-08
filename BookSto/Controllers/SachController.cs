using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookSto.Models;
using System.Data.Entity;

namespace BookSto.Controllers
{
    public class SachController : Controller
    {
        BookStoEntities2 db = new BookStoEntities2();
        
        #region Tài Khoản
        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["TaiKhoan"] != null)
            {
                return View();
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        public ActionResult ChiTietTaiKhoan()
        {
            if (Session["TaiKhoan"] != null)
            {
                if (Session["TaiKhoan"] != null)
                {
                    var s = Session["TaiKhoan"].ToString();
                    nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == s && n.TrangThai == 1); // V
                    return View(nguoidung);
                }
            }
            return RedirectToAction("DangNhap", "Admin");
        }

        [HttpGet]
        public ActionResult ChinhSuaTaiKhoan()
        {
            if (Session["TaiKhoan"] != null)
            {
                var s = Session["TaiKhoan"].ToString();
                nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == s); // V
                return View(nguoidung);
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        [HttpPost]
        public ActionResult ChinhSuaTaiKhoan(nguoidung nguoidung_web)
        {
            if (Session["TaiKhoan"] != null)
            {
                var s = Session["TaiKhoan"].ToString();
                nguoidung nguoidung_db = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == s); // V
                nguoidung_db.HoTen = nguoidung_web.HoTen;
                nguoidung_db.GioiTinh = nguoidung_web.GioiTinh;
                nguoidung_db.Tuoi = nguoidung_web.Tuoi;
                nguoidung_db.Email = nguoidung_web.Email;
                nguoidung_db.SDT = nguoidung_web.SDT;
                nguoidung_db.DiaChi = nguoidung_web.DiaChi;
                Session["HoTen"] = nguoidung_web.HoTen;
                Session["DiaChi"] = nguoidung_web.DiaChi;  // để giao hàng
                db.Entry(nguoidung_db).State = EntityState.Modified;
                db.SaveChanges();
                TempData["OK"] = "Cập nhật người dùng thành công!!";
                return RedirectToAction("ChiTietTaiKhoan", "NguoiDung");
            }
            return RedirectToAction("DangNhap", "Admin");
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
        public ActionResult DoiMatKhau(MatKhau matkhau)
        {

            if (Session["TaiKhoan"] != null)
            {
                if (ModelState.IsValid)
                {
                    var a = Session["TaiKhoan"].ToString();
                    nguoidung nguoidung = db.nguoidungs.SingleOrDefault(n => n.TaiKhoan == a); // V

                    if (matkhau.oldPassword == nguoidung.MatKhau)
                    {
                        nguoidung.MatKhau = matkhau.newPassword;
                        db.SaveChanges();
                        return RedirectToAction("ThongTinTaiKhoan", "NguoiDung");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật Khẩu cũ sai");
                        return View("DoiMatKhau");
                    }
                }
                return View("DoiMatKhau");
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        #endregion

        #region Thùng Sách
        private List<Thung> dsHangTon()
        {
            var danhSachGop = db.thungsaches
                .Where(ts => ts.TrangThai == 1)
                .GroupBy(ts => new { ts.MaSach, ts.SoLuongSach })
                .Select(g => new
                {
                    Sach = g.FirstOrDefault().sach,
                    MaThung = g.FirstOrDefault().MaThung,
                    MaSach = g.Key.MaSach,
                    SoSachTrongThung = g.Key.SoLuongSach,
                    TongSoThung = g.Count(),
                    TenSach = g.FirstOrDefault().sach.TenSach,
                    TacGia = g.FirstOrDefault().sach.tacgia.TenTacGia,
                    Gia = g.FirstOrDefault().sach.GiaTriSach,
                    Anh = g.FirstOrDefault().sach.Anh,
                    GiamGia = g.FirstOrDefault().sach.GiamGia
                })
                .ToList();

            List<Thung> lst_thung = new List<Thung>();
            foreach (var t in danhSachGop)
            {
                lst_thung.Add(new Thung(t.Sach, t.MaThung, t.MaSach, t.SoSachTrongThung, t.TongSoThung, t.TenSach, t.TacGia, t.Gia, t.Anh, t.GiamGia));
            }
            return lst_thung;
        } 
        public ActionResult TrangChu()
        {
            thungsach s_top = db.thungsaches.OrderByDescending(n => n.MaSach).Take(1).SingleOrDefault();
            TempData["s_top"] = s_top;
            List<Thung> lst_thung = dsHangTon();
            return View(lst_thung);
        }

        public ActionResult ChiTietSach(int? MaThung,int? MaSach)
        {
            if(MaSach == null)
            {
                return RedirectToAction("TrangChu", "Sach");
            }
            if(MaThung == null)
            {
                var ct1 = db.saches
                    .GroupJoin(
                        db.thungsaches
                            .Where(ts => ts.TrangThai == 1),
                        s => s.MaSach,
                        ts => ts.MaSach,
                        (s, tsGroup) => new
                        {
                            Sach = s,
                            Thungsaches = tsGroup
                        }
                    )
                    .SelectMany(
                        result => result.Thungsaches.DefaultIfEmpty(),
                        (result, ts) => new
                        {
                            Sach = result.Sach,
                            MaThung = ts != null ? ts.MaThung : (int?)null,
                            MaSach = ts != null ? ts.MaSach : result.Sach.MaSach,
                            SoSachTrongThung = ts != null ? ts.SoLuongSach : 0,
                            TongSoThung = result.Thungsaches.Count(),
                            TenSach = result.Sach.TenSach,
                            TacGia = result.Sach.tacgia.TenTacGia,
                            Gia = result.Sach.GiaTriSach,
                            Anh = result.Sach.Anh,
                            GiamGia = result.Sach.GiamGia
                        }
                    )
                    .Where(result => result.TongSoThung == 0)
                    .ToList();
                var ctiet = ct1.Where(n => n.MaSach == MaSach).SingleOrDefault();
                CTSach cts1 = new CTSach(ctiet.MaThung, ctiet.Sach, ctiet.SoSachTrongThung, ctiet.TongSoThung, ctiet.Sach.tacgia.TenTacGia, ctiet.Sach.nhaxb.TenNXB, ctiet.Sach.theloai.TenTheLoai);
                return View(cts1);
            }
            var item = (
                    from ts in db.thungsaches
                    where ts.TrangThai == 1
                    join g in (
                        from t in db.thungsaches
                        where t.TrangThai == 1
                        group t by new { t.MaSach, t.SoLuongSach } into grouped
                        select new
                        {
                            MaSach = grouped.Key.MaSach,
                            SoLuongSach = grouped.Key.SoLuongSach,
                            TongSoThung = grouped.Count()
                        }) on new { ts.MaSach, ts.SoLuongSach } equals new { g.MaSach, g.SoLuongSach }
                    join s in db.saches on ts.MaSach equals s.MaSach
                    join tg in db.tacgias on s.MaTacGia equals tg.MaTacGia
                    join nxb in db.nhaxbs on s.MaNXB equals nxb.MaNXB
                    join tl in db.theloais on s.MaTheLoai equals tl.MaTheLoai
                    group new { ts, g, s, tg, nxb, tl } by new
                    {
                        s.MaSach,
                        s.MaTheLoai,
                        s.MaTacGia,
                        s.MaNXB,
                        s.Anh,
                        s.GiamGia,
                        s.GiaTriSach,
                        s.GioiThieu,
                        s.SoLuong,
                        s.Tap,
                        s.TenSach,
                        s.TomTat,
                        s.TongSoTap,
                        s.TongSoTrang,
                        s.TrangThai,
                        ts.SoLuongSach,
                        tg.TenTacGia,
                        nxb.TenNXB,
                        tl.TenTheLoai
                    } into grouped
                    select new
                    {
                        MaThung = grouped.Min(x => x.ts.MaThung),
                        Sach = grouped.FirstOrDefault().s,
                        SoSachTrongThung = grouped.Key.SoLuongSach,
                        TongSoThung = grouped.Sum(x => x.g.TongSoThung),
                        TenTacGia = grouped.Key.TenTacGia,
                        TenNXB = grouped.Key.TenNXB,
                        TenTheLoai = grouped.Key.TenTheLoai
                    }).ToList();
            CTSach cts;
            var ct = item.Where(n => n.MaThung == MaThung).SingleOrDefault();
            cts = new CTSach(ct.MaThung, ct.Sach, ct.SoSachTrongThung, ct.TongSoThung, ct.TenTacGia, ct.TenNXB, ct.TenTheLoai);
            return View(cts);
        }

        public ActionResult PhanLoaiSach(int? matheloai, int? matacgia, string tensach)
        {
            List<Thung> lst_thung = dsHangTon();
            List<theloai> lst_tl = db.theloais.ToList();
            List<tacgia> lst_tg = db.tacgias.ToList();
            TempData["lst_tl"] = lst_tl;
            TempData["lst_tg"] = lst_tg;
            if ((matheloai != null) || (matacgia != null) || (tensach != null && tensach != ""))
            {
                tensach = tensach.Trim();
                TempData["matheloai"] = matheloai;
                TempData["matacgia"] = matacgia;
                TempData["tensach"] = tensach;
                lst_thung = lst_thung
                    .Where(n => (matacgia == null || n.Sach.MaTacGia == matacgia)
                    && (matheloai == null || n.Sach.MaTheLoai == matheloai) 
                    && n.TenSach.ToLower().Contains(tensach.ToLower()))
                    .ToList();
            }
            lst_thung = lst_thung.OrderByDescending(n => n.MaThung).ToList();
            var th = db.saches
                    .GroupJoin(
                        db.thungsaches
                            .Where(ts => ts.TrangThai == 1),
                        s => s.MaSach,
                        ts => ts.MaSach,
                        (s, tsGroup) => new
                        {
                            Sach = s,
                            Thungsaches = tsGroup
                        }
                    )
                    .SelectMany(
                        result => result.Thungsaches.DefaultIfEmpty(),
                        (result, ts) => new
                        {
                            Sach = result.Sach,
                            MaThung = ts != null ? ts.MaThung : (int?)null,
                            MaSach = ts != null ? ts.MaSach : result.Sach.MaSach,
                            SoSachTrongThung = ts != null ? ts.SoLuongSach : 0,
                            TongSoThung = result.Thungsaches.Count(),
                            TenSach = result.Sach.TenSach,
                            TacGia = result.Sach.tacgia.TenTacGia,
                            Gia = result.Sach.GiaTriSach,
                            Anh = result.Sach.Anh,
                            GiamGia = result.Sach.GiamGia
                        }
                    )
                    .Where(result => result.TongSoThung == 0 && result.Sach.TrangThai == 1)
                    .ToList(); 
            List<Thung> lst_thung0 = new List<Thung>();
            foreach (var t in th)
            {
                lst_thung0.Add(new Thung(t.Sach, t.MaThung, t.MaSach, t.SoSachTrongThung, t.TongSoThung, t.TenSach, t.TacGia, t.Gia, t.Anh, t.GiamGia));
            }
            TempData["lst_thung0"] = lst_thung0;
            return View(lst_thung);
        }
        #endregion

        #region Giỏ Hàng

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>; // khi đã có session giỏ hàng thì != null
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;     // tạo session giỏ hàng khi chưa có lstGH
            }
            return lstGioHang;
        }

        private int TongSoLuong()
        {
            int TongSL = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                TongSL = lstGioHang.Sum(n => n.SoThung);
            }
            return TongSL;
        }

        private double? TongTien()
        {
            double? dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                RedirectToAction("TrangChu", "Sach");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult ThemSach_GioHang(int MaThung, string Url)
        {
            thungsach ts = db.thungsaches.SingleOrDefault(n => n.MaThung == MaThung);
            if (ts == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<GioHang> lstGioHang = LayGioHang();
            GioHang Thung_Trong_GH = lstGioHang.Find(n => n.ts.MaThung == MaThung);
            if (Thung_Trong_GH == null)
            {
                Thung_Trong_GH = new GioHang(MaThung);
                lstGioHang.Add(Thung_Trong_GH);
                ViewBag.TongSoLuong = TongSoLuong();
                return Redirect(Url);
            }
            else
            {
                Thung_Trong_GH.SoThung++;
                ViewBag.TongSoLuong = TongSoLuong();
                return Redirect(Url);
            }
        }

        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() != 0)
            {
                ViewBag.TongSoLuong = TongSoLuong();
            }
            return PartialView();
        }

        public ActionResult CapNhatGioHang(List<int> listMaThung, List<int> listSL)
        {
            List<GioHang> lstGioHang = LayGioHang();
            for (int i = 0; i < listMaThung.Count; i++)
            {
                GioHang thung = lstGioHang.SingleOrDefault(n => n.ts.MaThung == listMaThung[i]);
                thung.SoThung = listSL[i];
            }
            //kiểm tra sản phẩm có trong giỏ hàng không
            return Json("200");
        }
        public ActionResult XoaSach_GioHang(int? MaThung)
        {
            thungsach ts = db.thungsaches.SingleOrDefault(n => n.MaThung == MaThung);
            if (ts == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<GioHang> lstGioHang = LayGioHang();
            //kiểm tra sản phẩm có trong giỏ hàng không
            GioHang sach_trong_gh = lstGioHang.SingleOrDefault(n => n.ts.MaThung == MaThung);
            if (sach_trong_gh != null)
            {
                lstGioHang.RemoveAll(n => n.ts.MaThung == MaThung);
            }
            if (lstGioHang.Count == 0)
            {
                RedirectToAction("TrangChu", "Sach");
            }
            return RedirectToAction("GioHang", "Sach");
        }

        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            if (Session["TaiKhoan"] != null && Session["DiaChi"] == null)
            {
                ViewBag.Diachi = "Yêu cầu cập nhật địa chỉ nhận hàng !";
                return RedirectToAction("ChinhSuaTaiKhoan", "Admin");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("TrangChu", "Sach");
            }
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count() == 0)
            {
                return RedirectToAction("TrangChu", "Sach");
            }
            //lưu thông tin vào bảng hóa đơn
            donxuat donban = new donxuat();
            donban.NgayTao = DateTime.Now;
            donban.MaKhachHang = (int)Session["MaNguoiDung"];
            donban.TongTien = (int)TongTien();
            donban.TrangThai = 1; // trạng thái đơn xuất (1: chờ admin duyệt, 5: hủy đơn, 2:admin chấp nhận đơn, 3:đang giao hàng, 4 : đã nhận hàng)
            db.donxuats.Add(donban);
            db.SaveChanges();
            // lưu thông tin vào chi tiết hóa đơn
            foreach (var item in lstGioHang)
            {
                ctdonxuat ctdb = new ctdonxuat();
                ctdb.MaDB = donban.MaDB;
                ctdb.MaSach = item.ts.sach.MaSach;
                ctdb.SoLuong = item.SoThung;
                ctdb.GiaXuat = (int)item.ThanhTien;
                db.ctdonxuats.Add(ctdb);
                List<thungsach> lst_thung = db.thungsaches.Where(n=>n.TrangThai == 1 && n.MaSach == item.ts.MaSach).Take(item.SoThung).ToList();
                foreach (var item2 in lst_thung)
                {
                    item2.TrangThai = -1;// trạng thái thùng (0: đơn nhập hàng chưa xác nhận,1: thùng trong kho; 2: thùng đã bán; -1:đơn bán chưa xác nhận)
                    item2.MaCTDonXuat = ctdb.MaCTDonXuat;
                    db.Entry(item2).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (Session["GioHang"] != null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            db.SaveChanges();
            return RedirectToAction("TrangChu", "Sach");
        }
        #endregion
        #region Theo dõi đơn hàng

        public ActionResult DSDonDat()
        {
            if(Session["TaiKhoan"] != null)
            {
                int makh = (int)Session["MaNguoiDung"];
                List<donxuat> lst_dx = db.donxuats.Where(n => n.MaKhachHang == makh).Include(n => n.ctdonxuats).OrderByDescending(n => n.MaDB).ToList();
                return View(lst_dx);
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        public ActionResult CTDonDat(int? MaDB)
        {
            if(Session["TaiKhoan"] != null && MaDB.ToString() != null)
            {
                int makh = (int)Session["MaNguoiDung"];
                donxuat lst_dx = db.donxuats.Where(n => n.MaDB == MaDB).Include(n => n.ctdonxuats).SingleOrDefault();
                return View(lst_dx);
            }
            return RedirectToAction("DangNhap", "Admin");
        }
        #endregion
    }
}