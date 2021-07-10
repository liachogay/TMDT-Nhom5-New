using System.Web.Mvc;
using System.Web.Routing;

namespace MyProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "gioi-thieu",
                url: "GioiThieu/GioiThieu",
                defaults: new { controller = "GioiThieu", action = "GioiThieu" }
            );

            routes.MapRoute(
                name: "gio-hang",
                url: "gio-hang",
                defaults: new { controller = "GioHang", action = "GioHang" }
            );

            routes.MapRoute(
                name: "the-loai-seo-all",
                url: "tat-ca",
                defaults: new { controller = "SanPham", action = "SanPhamPartial" }
            );
            /**
             * {HOST}/category : {HOST}/dtdd
             */
            routes.MapRoute(
                name: "the-loai-seo",
                url: "{category}",
                defaults: new { controller = "SanPham", action = "TheLoai" }
            );

            /**
             * {HOST}/{category}/{slug} : {HOST}/dtdd/dien-thoai-sam-sung-a-b
             */
            routes.MapRoute(
                name: "san-pham-seo",
                url: "{category}/{slug}",
                defaults: new { controller = "SanPham", action = "SanPhamSEO" }
            );

            routes.MapRoute(
                name: "SanPham",
                url: "SanPham/XemChiTiet/{id}",
                defaults: new { controller = "SanPham", action = "XemChiTiet", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "TrangChu", id = UrlParameter.Optional }
            );
        }
    }
}