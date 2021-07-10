using MyProject.Models;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class SanPhamController : Controller
    {
        //
        // GET: /SanPham/
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SanPhamPartial(int page = 1, int pageSize = 12)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            var dsSanPham = new Product();
            var model = dsSanPham.ListAll(page, pageSize);
            return View(model);
        }

        public ActionResult SanPhamTheoLoai(int maLoaiSP)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            var dsSPTheoLoai = db.SanPhams.Where(sp => sp.MaLoaiSP == maLoaiSP).OrderBy(sp => sp.GiaBan).ToList();
            if (dsSPTheoLoai.Count == 0)
            {
                ViewBag.thongBao = "Sản phẩm đã hết. Xin quý khách thông cảm";
            }
            return View(dsSPTheoLoai);
        }

        public ActionResult SanPham()
        {
            var listSanPham = db.SanPhams.OrderBy(sp => sp.TenSP).ToList();
            return View(listSanPham);
        }

        public ActionResult TheLoai(string category, int page = 1, int pageSize = 12)
        {
            var _category = db.LoaiSanPhams.SingleOrDefault(c => c.Slug == category);
            if (_category == null) return HttpNotFound();

            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            var listSanPham = db.SanPhams.Where(c => c.MaLoaiSP == _category.MaLoaiSP).OrderByDescending(sp => sp.MaSP).ToPagedList(page, pageSize);

            return View("SanPhamPartial", listSanPham);
        }

        // Lay san pham dua tren slug
        public ActionResult SanPhamSEO(string category, string slug)
        {
            var _category = db.LoaiSanPhams.SingleOrDefault(c => c.Slug == category);
            if (_category == null) return HttpNotFound();

            var _sanPham = db.SanPhams.SingleOrDefault(sp => sp.Slug == slug);
            if (_sanPham == null) return HttpNotFound();

            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);

            return View("XemChiTiet", _sanPham);
        }

        public ActionResult XemChiTiet(int masp)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            SanPham sanPham = db.SanPhams.Single(s => s.MaSP == masp);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        public ActionResult SanPhamTuongTu()
        {
            var listSanPham = db.SanPhams.OrderBy(sp => sp.TenSP).ToList();
            return View(listSanPham);
        }

        public ActionResult timKiemSanPham(string tenSP)
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            if (!string.IsNullOrEmpty(tenSP))
            {
                var query = from sp in db.SanPhams where sp.TenSP.Contains(tenSP) select sp;
                if (query.Count() == 0)
                {
                    return RedirectToAction("thongBaoRong", "SanPham");
                }
                return View(query);
            }
            return View();
        }

        public ActionResult thongBaoRong()
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            ViewBag.stringEmpty = "Không tìm thấy sản phẩm";
            return View();
        }

    }
}
