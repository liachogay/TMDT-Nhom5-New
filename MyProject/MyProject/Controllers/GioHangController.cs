using MyProject.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    public class GioHangController : Controller
    {
        //
        // GET: /GioHang/

        public ActionResult Index()
        {
            return View();
        }
        // Tạo đối tượng db chứa dữ liệu từ Model QLBanQuanAo
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang == null)
            {
                // Nếu listGioHang chưa tồn tại thì khởi tạo
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        // Xây dựng phương thức thêm vào giỏ hàng
        public ActionResult ThemGioHang(int msp, string strURL)
        {
            // Lấy ra giỏ hàng
            List<GioHang> listGioHang = LayGioHang();
            // Kiểm tra sách này có tồn tại trong Session["GioHang"] hay chưa ?
            GioHang SanPham = listGioHang.Find(sp => sp.maSP == msp);
            if (SanPham == null)
            { // Chưa có trong giỏ hàng
                SanPham = new GioHang(msp);
                listGioHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            { // Đã có sản phẩm này trong giỏ 
                SanPham.soLuong++;
                return Redirect(strURL);
            }
        }

        // Xây dựng phương thức tính tổng số lượng
        private int TongSoLuong()
        {
            int tongSoLuong = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tongSoLuong = listGioHang.Sum(sp => sp.soLuong);
                Session.Add("TongSoLuong", tongSoLuong);
            }
            return tongSoLuong;
        }

        // Xây dựng phương thức tính tổng thành tiền
        private double TongThanhTien()
        {
            double tongThanhTien = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tongThanhTien += listGioHang.Sum(sp => sp.thanhTien);
            }
            return tongThanhTien;
        }

        // Xây dựng trang giỏ hàng

        public ActionResult GioHang()
        {
            ViewBag.loaiSP = db.LoaiSanPhams.OrderBy(sp => sp.MaLoaiSP);
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("GioHangTrong", "GioHang");
            }
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listGioHang);
        }

        public ActionResult GioHangPartial()
        {
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }

        public ActionResult CapNhatGioHang(int maSP, FormCollection f)
        {
            List<GioHang> listGH = LayGioHang();
            GioHang sp = listGH.Single(s => s.maSP == maSP);
            if (sp != null)
            {
                sp.soLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult XoaGioHang(int maSP)
        {
            List<GioHang> listGH = LayGioHang();
            GioHang sp = listGH.Single(s => s.maSP == maSP);
            if (sp != null)
            {
                listGH.RemoveAll(s => s.maSP == maSP);
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            if (listGH.Count == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult XoaGioHangAll()
        {
            List<GioHang> listGH = LayGioHang();
            listGH.Clear();
            if (listGH.Count == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            return RedirectToAction("SanPhamPartial", "SanPham");
        }

        public ActionResult GioHangTrong()
        {
            ViewBag.thongBao = "Bạn chưa thêm sản phẩm nào";
            return View();
        }

        public ActionResult ViewGioHangHover()
        {
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.thongBao = "Bạn chưa thêm sản phẩm nào";
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listGioHang);
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiểm tra đăng nhập
            if (Session["taikhoan"] == null || Session["taikhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }

            // Lấy giỏ hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            if (ViewBag.TongSoLuong == 0)
            {
                return RedirectToAction("SanPhamPartial", "SanPham");
            }
            ViewBag.TongThanhTien = TongThanhTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            // Thêm đơn hàng
            HoaDon ddh = new HoaDon();
            KhachHang kh = (KhachHang)Session["taikhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:mm/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrang = "Chưa thanh toán";
            db.HoaDons.InsertOnSubmit(ddh);
            db.SubmitChanges();
            Session.Add("NgayGiao", ddh.NgayGiao);
            Session.Add("MaHD", ddh.MaHD);

            foreach (var item in gh)
            {
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaSP = item.maSP;
                ctdh.SoLuong = item.soLuong;
                ctdh.DonGia = (decimal)item.donGia;
                db.ChiTietHoaDons.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            return RedirectToAction("XacNhanDatHang", "GioHang");
        }

        public ActionResult XacNhanDatHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            Session["GioHang"] = null;
            return View(lstGioHang);
        }

        private Payment payment;
        private Payment CreatePayment(APIContext apiContext, string redirectURL)
        {
            var listItems = new ItemList() { items = new List<Item>() };
            List<GioHang> listGioHangs = LayGioHang();
            foreach (var gioHang in listGioHangs)
            {
                listItems.items.Add(new Item()
                {
                    name = gioHang.tenSP,
                    currency = "USD",
                    price = Math.Round(gioHang.donGia / 23000).ToString(),
                    quantity = gioHang.soLuong.ToString(),
                    sku = "sku"
                });
            }
            var payer = new Payer() { payment_method = "paypal" };

            //do the configuration RedirectUrls here with RedirectUrls object
            var redirUrl = new RedirectUrls
            {
                cancel_url = redirectURL,
                return_url = redirectURL
            };

            //create Details object
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = Math.Round(listGioHangs.Sum(x => x.donGia * x.soLuong) / 23000).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = Math.Round(Convert.ToDouble(Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal))).ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Shop test transaction description",
                invoice_number = Convert.ToString(new Random().Next(10000)),
                amount = amount,
                item_list = listItems
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrl
            };
            // de t test lai
            return payment.Create(apiContext);
        }

        //create execute payment method
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId,
            };
            payment = new Payment()
            {
                id = paymentId
            };

            return payment.Execute(apiContext, paymentExecution);
        }

        //Create payment with paypal method
        public ActionResult PaymentWithPaypal()
        {
            //Getting context from the paypal bases on  clientId and clientSecret for payment
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //Create a payment
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/GioHang/PaymentWithPaypal?";
                    var guid = Convert.ToString(new Random().Next(10000));
                    var createdPayment = CreatePayment(apiContext, baseUrl + "guid=" + guid);

                    //get link returned from paypal response to create call function
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;


                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    //This one will be executed when he have received all the payment params from previous call
                    var guid = Request.Params["guid"];
                    var executePayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executePayment.state.ToLower() != "approved")
                    {
                        return View("Failure");
                    }
                }
            }
            catch (Exception e)
            {
                PaypalLogger.Log("Error: " + e.Message);
                return View("Failure");
            }
            return View("Success");
        }
    }
}
