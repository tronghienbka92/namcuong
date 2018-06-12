﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using WebGrease.Css.Extensions;
using Nop.Web.Models.NhaXes;
using Nop.Core.Data;
using Nop.Services.NhaXes;
using Nop.Core.Caching;
using Nop.Core.Domain.News;
using Nop.Core.Domain.NhaXes;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services.Chonves;
using Nop.Services.Security;
using Nop.Core.Domain.Security;
using System.Globalization;
using Nop.Services.Catalog;
using Nop.Web.Models.VeXeKhach;
using Nop.Core.Domain.Chonves;
using Nop.Web.Models.NhaXeBaoCao;
using System.IO;
using Nop.Services.ExportImport;


namespace Nop.Web.Controllers
{
    public class NhaXeBaoCaoController : BaseNhaXeController
    {
        #region Khoi Tao
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IVeXeService _vexeService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPhoiVeService _phoiveService;
        private readonly IPhieuGuiHangService _phieuguihangService;
        private readonly IHangHoaService _hanghoaService;
        private readonly IXeInfoService _xeinfoService;
        private readonly ILimousineBanVeService _limousinebanveService;
        private readonly IExportManager _exportManager;

        public NhaXeBaoCaoController(IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerService customerService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            IHanhTrinhService hanhtrinhService,
             IVeXeService vexeService,
            IPriceFormatter priceFormatter,
            IPhoiVeService phoiveService,
            IPhieuGuiHangService phieuguihangService,
            IHangHoaService hanghoaService,
            IXeInfoService xeinfoService,
            ILimousineBanVeService limousinebanveService,
             IExportManager exportManager
            )
        {
            this._limousinebanveService = limousinebanveService;
            this._hanghoaService = hanghoaService;
            this._phieuguihangService = phieuguihangService;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._diachiService = diachiService;
            this._nhanvienService = nhanvienService;
            this._permissionService = permissionService;
            this._hanhtrinhService = hanhtrinhService;
            this._vexeService = vexeService;
            this._priceFormatter = priceFormatter;
            this._phoiveService = phoiveService;
            this._xeinfoService = xeinfoService;
            this._exportManager = exportManager;

        }
        #endregion
        #region Common
        [NonAction]
        protected virtual void PrepareThongKeItemToModel(ThongKeItem model, ThongKeItem item)
        {
            model.Nhan = item.Nhan;
            model.NhanSapXep = item.NhanSapXep;
            model.ItemId = item.ItemId;
            model.ItemDataDate = item.ItemDataDate;
            model.GiaTri = item.GiaTri;
            model.SoLuong = item.SoLuong;
            model.GiaTri1 = item.GiaTri1;
            model.GiaTri2 = item.GiaTri2;
        }
        [NonAction]
        protected virtual void PrepareListQuy(BaoCaoNhaXeModel model)
        {
            if (DateTime.Now.Month < 4)
                model.QuyId = 1;
            else if (DateTime.Now.Month < 7)
                model.QuyId = 2;
            else if (DateTime.Now.Month < 10)
                model.QuyId = 3;
            else
                model.QuyId = 4;
            model.ListQuy = this.GetCVEnumSelectList<ENBaoCaoQuy>(_localizationService, model.QuyId);
        }

        [NonAction]
        protected virtual void PrepareListNgayThangNam(BaoCaoNhaXeModel model)
        {
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                model.ListYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == model.NamId) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListMonth.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == model.ThangId) });
            }
        }

        [NonAction]
        protected virtual IList<SelectListItem> GetListLoaiThoiGian()
        {
            return this.GetCVEnumSelectList<ENBaoCaoLoaiThoiGian>(_localizationService, 0);
        }
        [NonAction]
        protected virtual IList<SelectListItem> GetListChuKyThoiGian()
        {
            return this.GetCVEnumSelectList<ENBaoCaoChuKyThoiGian>(_localizationService, 0);
        }

        void PrepareListVanPhongModel(BaoCaoNhaXeModel model)
        {
            if (this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao))
            {
                var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId);
                model.VanPhongs = vanphongs.Select(c => new SelectListItem
                {
                    Text = c.TenVanPhong,
                    Value = c.Id.ToString(),
                }).ToList();
            }
            else
            {
                SelectListItem item = new SelectListItem();
                item.Text = _workContext.CurrentVanPhong.TenVanPhong;
                item.Value = _workContext.CurrentVanPhong.Id.ToString();
                item.Selected = true;
                model.VanPhongs.Add(item);
            }
        }

        void PrepareListXeModel(BaoCaoNhaXeModel model)
        {
            var xevanchuyen = _xeinfoService.GetAllXeVanChuyenByNhaXeId(_workContext.NhaXeId);
            model.Xe = xevanchuyen.Select(c => new SelectListItem
            {
                Text = c.BienSo,
                Value = c.Id.ToString(),
            }).ToList();
        }
        #endregion
        #region bao cao doanh thu

        public ActionResult BaoCaoDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }

        [HttpPost]
        public ActionResult BaoCaoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();

            var items = _limousinebanveService.GetBaoCaoDoanhThu(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id);
            ENBaoCaoChuKyThoiGian loaiid = (ENBaoCaoChuKyThoiGian)model.Loai1Id;
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                switch (loaiid)
                {
                    case ENBaoCaoChuKyThoiGian.HangNgay:
                        {
                            _doanhthu.ThoiGian = string.Format("{0}/{1}/{2}", _doanhthu.Nhan, model.ThangId, model.NamId);
                            break;
                        }
                    case ENBaoCaoChuKyThoiGian.HangThang:
                        {
                            _doanhthu.ThoiGian = string.Format("{0}/{1}", _doanhthu.Nhan, model.NamId);
                            break;
                        }
                    case ENBaoCaoChuKyThoiGian.HangNam:
                        {
                            _doanhthu.ThoiGian = _doanhthu.Nhan;
                            break;
                        }
                }
                _doanhthu.TongDoanhThu = c.GiaTri;
                _doanhthu.DoanhThuChonVe = c.GiaTri1;
                _doanhthu.DoanhThuNhaXe = c.GiaTri2;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThu(string thoigian)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.NgayBan = thoigian;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ChiTietDoanhThu(DataSourceRequest command, string thoigian)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            DateTime _ngaydi = Convert.ToDateTime(thoigian);
            var items = _limousinebanveService.GetDetailDoanhThu(_workContext.NhaXeId, _ngaydi).Select(c => new BaoCaoNhaXeModel.KhachHangMuaVeModel
            {
                CustomerId = c.CustomerId,
                NguonVeXeId = c.NguonVeXeId,
                KyHieuGhe = c.KyHieuGhe,
                isChonVe = c.isChonVe,
                GiaVe = c.GiaVe,
                NgayDi = c.NgayDi,
                SoDienThoai = c.SoDienThoai,
                TenKhachHang = c.TenKhachHang,
                ThongTinChuyenDi = c.ThongTinChuyenDi,

            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }


        public ActionResult ThongKeDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult BieuDoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var items = _limousinebanveService.GetBaoCaoDoanhThu(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id).OrderBy(c => c.NhanSapXep).ToList();
            return Json(items);
        }

        #endregion
        
        #region Doanh thu theo xe
        public ActionResult DoanhThuLaiXeTheoXe()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new ListDoanhthuNhanVienXeModel();
            model.TuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.DenNgay = DateTime.Now;

            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetDoanhThuLaiXeTheoXe(string TuNgay, string DenNgay, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new ListDoanhthuNhanVienXeModel();
            var _tungay = Convert.ToDateTime(TuNgay);
            var _denngay = Convert.ToDateTime(DenNgay);
            model.doanhthus = _limousinebanveService.GetAllChuyenTheoNgay(_tungay, _denngay, SearchName,_workContext.NhaXeId);

            return PartialView(model);
        }
        
        [HttpPost]
        public ActionResult _ChiTietDoanhThuTheoXe(DataSourceRequest command, int XeId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDetailDoanhThuBanVeTungXeTheoNgay(_NgayBan, _workContext.NhaXeId, XeId).Select(c =>
            {

                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }
        public ActionResult ExportExcelDoanhThuXe(string TuNgay, string DenNgay, string SearchName)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {
                var _tungay = Convert.ToDateTime(TuNgay);
                var _denngay = Convert.ToDateTime(DenNgay);


                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var dt = _limousinebanveService.GetAllChuyenTheoNgay(_tungay, _denngay, SearchName,_workContext.NhaXeId);
                    _exportManager.ExportDoanhThuXeToXlsx(stream, dt, _tungay, _denngay);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Tổng hợp doanh thu" + _tungay.ToString("ddMMyyyy") + "_" + _denngay.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
        #region Doanh thu chi tiet tung luot theo xe
        public ActionResult HLDoanhThuChiTietTheoNgay()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new ListDoanhThuChiTietModel();
            model.NgayDi = DateTime.Now;

            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _ChiTietDoanhThuTrongNgayTheoXe(string NgayDi, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new ListDoanhThuChiTietModel();
            var _NgayDi = Convert.ToDateTime(NgayDi);

            model.ListXe = _limousinebanveService.GetAllChuyenTheoXeTrongNgay(_NgayDi, SearchName,_workContext.NhaXeId);

            return PartialView(model);
        }

        public ActionResult ExportExcelChiTietChuyenTrongNgay(string NgayDi, string SearchName)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {
                var _NgayDi = Convert.ToDateTime(NgayDi);

                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var dt = _limousinebanveService.GetAllChuyenTheoXeTrongNgay(_NgayDi, SearchName,_workContext.NhaXeId);
                    _exportManager.ExportLuotTrongNgayToXlsx(stream, dt, _NgayDi);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Chi tiêt theo ngày" + _NgayDi.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
        #region doanh thu theo nhan vien
        public ActionResult DoanhThuNhanvien()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoNgay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var items = _limousinebanveService.GetDoanhThuBanVeTheoNgay(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NgayBan = c.Nhan;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };
            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoNhanVien(DataSourceRequest command, int VanPhongId, string NgayBan)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime ngayban = Convert.ToDateTime(NgayBan);
            var items = _limousinebanveService.GetDoanhThuBanVeTheoNhanVien(_workContext.NhaXeId, VanPhongId, ngayban);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.NgayBan = NgayBan;
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoTrangThai(DataSourceRequest command, int VanPhongId, string NgayBan, int NhanvienId)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime ngayban = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDoanhThuBanVeTheoTrangThai(_workContext.NhaXeId, VanPhongId, ngayban, NhanvienId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = NhanvienId;
                _doanhthu.NgayBan = NgayBan;
                _doanhthu.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (_doanhthu.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    _doanhthu.TrangThaiPhoiVeText = "Đã thanh toán";
                if (_doanhthu.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    _doanhthu.TrangThaiPhoiVeText = "Chưa thanh toán";
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuNhanVien(int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
            model.NhanVienId = NhanVienId;
            model.NgayBan = NgayBan;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _ChiTietDoanhThuNhanVien(DataSourceRequest command, int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _limousinebanveService.GetDetailDoanhThu(_workContext.NhaXeId, _NgayBan, NhanVienId).Select(c =>
            {
                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }
        #endregion
        #region thong ke theo hanh trinh
        public ActionResult DoanhThuTuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();

            modeldoanhthu.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            modeldoanhthu.ListLoai2.Insert(0, _item);
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);
            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuTuyen(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var ListItem = new List<ThongKeItem>();
            ListItem = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId)
               .Select(c =>
               {
                   var _doanhthu = new ThongKeItem();
                   _doanhthu.Nhan = c.MoTa;
                   int _sl;
                   _doanhthu.GiaTri = _limousinebanveService.DoanhThuTuyen(c.Id, model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                   _doanhthu.SoLuong = _sl;
                   return _doanhthu;
               }).ToList();
            return Json(ListItem.ToList());
        }
        public ActionResult DoanhThuLichTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            modeldoanhthu.ListLoai2 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuLichTrinh(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(model.Loai1Id).Select(c =>
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = string.Format("{0}-{1}", c.ThoiGianDi.ToString("HH:mm"), c.ThoiGianDi.AddMinutes(c.KhungThoiGian).ToString("HH:mm"));
                    int _sl;
                    _doanhthu.GiaTri = _limousinebanveService.DoanhThuLichTrinh(c.Id, model.Loai1Id, model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai2Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    return _doanhthu;
                }).ToList();


            return Json(lichtrinhs);


        }
        public ActionResult DoanhThuVanPhong()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuVanPhong(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Select(c =>
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = c.TenVanPhong;
                    int _sl;
                    var listnhavien = _nhanvienService.GetAllByVanPhongId(c.Id);
                    _doanhthu.GiaTri = _phoiveService.DoanhThuVanPhong(listnhavien.Select(nv => nv.Id).ToList(), model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    return _doanhthu;
                }).ToList();

            return Json(vanphongs);
        }
        #endregion
        #region Báo cáo doanh thu theo tuyến
        public ActionResult BaoCaoDoanhThuTuyen()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddMonths(-1);
            model.DenNgay = DateTime.Now;
            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            model.ListLoai2.Insert(0, _item);
            return View(model);
        }
        [HttpPost]
        public ActionResult BaoCaoDoanhThuTuyen(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId).ToList();
            var items = _phoiveService.GetDoanhThuTheoTuyen(tuNgay, denNgay, lichtrinhs.Select(lt => lt.Id).ToList());

            var doanhthus = items.Select(c =>
            {
                var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuXeTungNgayModel();
                model.NgayBan = c.Nhan;
                model.TongDoanhThu = c.GiaTri;
                model.NgayBan = c.ItemDataDate.ToString("yyyy-MM-dd");
                model.SoLuong = c.SoLuong;
                return model;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };
            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuTheoChang(int NguonVeId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
            model.NguonVeId = NguonVeId;
            model.NgayBan = NgayBan;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _ChiTietDoanhThuTheoChang(DataSourceRequest command, int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _limousinebanveService.GetDetailDoanhThu(_workContext.NhaXeId, _NgayBan, NhanVienId).Select(c =>
            {
                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }

        #endregion
        #region Bao cao ky gui hang hoa
        public ActionResult DoanhThuKyGuiHangNgay()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult DoanhThuKyGuiHangNgay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();

            var items = _phieuguihangService.GetDoanhThuNhanvien(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.ItemDataDate = Convert.ToDateTime(c.ItemDataDay + "-" + c.ItemDataMonth + "-" + c.ItemDataYear);
                _doanhthu.NgayBan = _doanhthu.ItemDataDate.ToString("yyyy-MM-dd");

                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuKyGui(int NhanVienId, string NgayThu, string NotPay)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDetailDoanhThuKiGuiModel();
            model.NhanVienId = NhanVienId;
            model.NgayBan = NgayThu;
            model.NotPay = NotPay;
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _ChiTietDoanhThuKyGui(DataSourceRequest command, int NhanVienId, string NgayThu, string NotPay)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayThu);
            var items = new List<PhieuGuiHang>();
            if (NotPay == "null")
            {
                items = _phieuguihangService.GetAllByNhanVien(_workContext.NhaXeId, NhanVienId, _NgayBan);
            }

            else
            {
                items = _phieuguihangService.GetDetailDoanhThuKiGuiNotPay(_workContext.NhaXeId, NhanVienId, _NgayBan);
            }

            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var hanghoas = _hanghoaService.GetAllHangHoaByPhieuGuiHangId(x.Id);
                    var m = x.ToModel(_localizationService, _priceFormatter, hanghoas);
                    return m;
                }),
                Total = items.Count
            };
            return Json(gridModel);
        }
        public ActionResult DoanhThuKyGuiHangNgayNotPay()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }

        [HttpPost]
        public ActionResult _DoanhThuKyGuiHangNgayNotPay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();

            var items = _phieuguihangService.GetDoanhThuKiGuiNotPay(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.ItemDataDate = Convert.ToDateTime(c.ItemDataDay + "-" + c.ItemDataMonth + "-" + c.ItemDataYear);
                _doanhthu.NgayBan = _doanhthu.ItemDataDate.ToString("yyyy-MM-dd");
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }

        // thống kê theo doanh thu
        public ActionResult HangHoaTheoDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult HangHoaTheoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            //_phieuguihangService.InsertPhieuGuiHang(phieugui);
            var items = _phieuguihangService.GetAllPhieuGuiHangByCuoc(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id);
            return Json(items);
        }
        public ActionResult HangHoaTheoVanPhong()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult HangHoaTheoVanPhong(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var _doanhthu = new ThongKeItem();
                _doanhthu.Nhan = c.TenVanPhong;
                int _sl;
                var listnhavien = _nhanvienService.GetAllByVanPhongId(c.Id);
                _doanhthu.GiaTri = _phieuguihangService.HangHoaDoanhThuVanPhong(listnhavien.Select(nv => nv.Id).ToList(), model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                _doanhthu.SoLuong = _sl;
                return _doanhthu;
            }).ToList();

            return Json(vanphongs);
        }
        #endregion
        #region doanh thu luot cho nhan vien
        [NonAction]
        protected virtual void PrepareListNgayThangNamToLuot(DoanhThuNhanVienLuotModel model)
        {
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                model.ListYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == model.NamId) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListMonth.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == model.ThangId) });
            }
        }
        public ActionResult HLThongKeLuotLaiXe()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new DoanhThuNhanVienLuotModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNamToLuot(modeldoanhthu);
            //PrepareListQuyToLuot(modeldoanhthu);

            return View(modeldoanhthu);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetDoanhThuNhanVienLuot(int ThangId, int NamId, int Loai1Id, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new DoanhThuNhanVienLuotModel();
            model.ThangId = ThangId;
            model.NamId = NamId;
            model.Loai1Id = Loai1Id;

            DoanhThuNhanvienLuotPrepare(model, SearchName);
            return PartialView(model);
        }

        //public ActionResult ExportExcelLuotLaiXe(int ThangId, int NamId, int Loai1Id)
        //{
        //    if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
        //        return AccessDeniedView();

        //    try
        //    {


        //        var luot = _limousinebanveService.GetAllDatVeByKhachHang(TuNgay, DenNgay, KhachHangId);

        //        byte[] bytes;
        //        using (var stream = new MemoryStream())
        //        {
        //            _exportManager.ExportKhachHangToXlsx(stream, luot, TuNgay, DenNgay);
        //            bytes = stream.ToArray();

        //        }

        //        return File(bytes, "text/xls", "Thống kê lượt khách hàng_" + TuNgay.ToString("ddMMyyyy") + "_" + DenNgay.ToString("ddMMyyyy") + ".xlsx");
        //    }
        //    catch (Exception exc)
        //    {
        //        ErrorNotification(exc);
        //        return RedirectToAction("List");
        //    }
        //}
        [NonAction]
        protected virtual int CountXeXuatBen(List<ThongKeLuotXuatBenItem> dataview, int NhanVienId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.LaiXeId == NhanVienId && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Count();
        }
        [NonAction]
        protected virtual void DoanhThuNhanvienLuotPrepare(DoanhThuNhanVienLuotModel model, string SearchName)
        {

            int SoNhan = 0;
            DoanhThuNhanVienLuotModel.DateModel[] arrNhan = new DoanhThuNhanVienLuotModel.DateModel[] { };
            var arrNhanVien = _nhaxeService.GetAllNhanVienByNhaXe(_workContext.NhaXeId, new ENKieuNhanVien[] { ENKieuNhanVien.LaiXe, ENKieuNhanVien.PhuXe }, SearchName);
            var arrNhanVienId = arrNhanVien.Select(c => c.Id).ToArray();

            model.SoNhanVien = arrNhanVien.Count();


            switch (model.Loai1Id)
            {
                case (int)ENBaoCaoChuKyThoiGian.HangNgay:
                    {
                        SoNhan = DateTime.DaysInMonth(model.NamId, model.ThangId);
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.nhan = c;
                            item.day = c;
                            item.month = model.ThangId;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangThang:
                    {
                        SoNhan = 12;
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.day = 0;
                            item.month = c;
                            item.nhan = c;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangNam:
                    {
                        //lay trong vong 5 nam
                        SoNhan = 5;
                        arrNhan = Enumerable.Range(DateTime.Now.Year - SoNhan + 1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.day = 0;
                            item.month = 0;
                            item.nhan = c;
                            item.year = c;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        model.NamId = 0;
                        break;
                    }
            }

            //lay thong tin du lieu 
            var _dataview = _nhaxeService.GetThongKeLuotXuatBen(model.ThangId, model.NamId, arrNhanVienId);

            model.SoNhan = SoNhan;
            model.DoanhThuLuot = new DoanhThuNhanVienLuotModel.NhanVienDateModel[model.SoNhanVien + 1, SoNhan + 1];
            for (int i = 0; i < model.SoNhanVien + 1; i++)
            {
               
                for (int j = 0; j < SoNhan + 1; j++)
                {
                    if (i == 0 && j > 0)
                    {
                        model.DoanhThuLuot[0, j] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[0, j].Nhan = arrNhan[j - 1].nhan;

                    }
                    if (j == 0 && i > 0)
                    {
                        model.DoanhThuLuot[i, 0] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[i, 0].NhanVienId = arrNhanVien[i - 1].Id;
                        model.DoanhThuLuot[i, 0].TenNhanVien = arrNhanVien[i - 1].HoVaTen;

                    }

                    if (i > 0 && j > 0)
                    {
                        model.DoanhThuLuot[i, j] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[i, j].Nhan = arrNhan[j - 1].nhan;
                        model.DoanhThuLuot[i, j].NhanVienId = arrNhanVien[i - 1].Id;
                        model.DoanhThuLuot[i, j].soLuot = CountXeXuatBen(_dataview, arrNhanVien[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);
                       
                    }

                }
                //so ngay di trong thang

            }

        }



        #endregion
        #region thong ke luot cho khach hang
        public ActionResult HLThongKeLuotKhachHang()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new TKLuotKhachHangModel();
            model.TuNgay = DateTime.Now.AddMonths(-1);
            model.DenNgay = DateTime.Now;

            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetThongKeLuotKhachHang(string TuNgay, string DenNgay, int KhachHangId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new TKLuotKhachHangModel();
            var _tungay = Convert.ToDateTime(TuNgay);
            var _denngay = Convert.ToDateTime(DenNgay).Date.AddDays(1);
            model.doanhthus = _limousinebanveService.GetAllDatVeByKhachHang(_tungay, _denngay, KhachHangId);
            model.TongSL = model.doanhthus.Count();
            model.TongTien = model.doanhthus.Sum(c=>c.GiaTien);
            return PartialView(model);
        }
        public ActionResult ExportExcelKhachHang(DateTime TuNgay, DateTime DenNgay, int KhachHangId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {


                var luot = _limousinebanveService.GetAllDatVeByKhachHang(TuNgay, DenNgay, KhachHangId);
               
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportKhachHangToXlsx(stream, luot, TuNgay, DenNgay);
                    bytes = stream.ToArray();

                }

                return File(bytes, "text/xls", "Thống kê lượt khách hàng_" + TuNgay.ToString("ddMMyyyy") + "_" + DenNgay.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #endregion
        public ActionResult ExportExcelLenhPhu(int Id)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {


                var _historyxexuatben = _limousinebanveService.GetChuyenDiById(Id);
                var bienso="";
                if (_historyxexuatben.xevanchuyen != null)
                    bienso = _historyxexuatben.xevanchuyen.BienSo;
                //lay thong ti khach di xe
                var hanhkhachs = _limousinebanveService.GetDatVeByChuyenDi(Id).Select(c =>
                {
                    var _datve = new DatVe();
                    _datve = c;
                    var htdd = _hanhtrinhService.GetHTDDByHanhTrinhVaDiemDon(_datve.HanhTrinhId, _datve.DiemDonId.Value);
                    if (htdd != null)
                    {
                        _datve.ThuTuDon = htdd.ThuTu;

                    }
                    return _datve;
                }).OrderBy(c => c.ThuTuDon).ToList();
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportLenhPhuToXlsx(stream, hanhkhachs, _historyxexuatben.NgayDiThuc, bienso);
                    bytes = stream.ToArray();

                }

                return File(bytes, "text/xls", "Lệnh phụ_" + bienso + "_" + _historyxexuatben.NgayDiThuc.ToString("ddMMyyyy HH:mm") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        public ActionResult ExportExcelDSKhachHang()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {
                var ngay=DateTime.Now;

                var items = _limousinebanveService.GetAllHanhKhach(_workContext.NhaXeId, "");
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportDSKhachHangToXlsx(stream, items);
                    bytes = stream.ToArray();

                }

                return File(bytes, "text/xls", "DSKhachhang_" +  ngay.ToString("ddMMyyyy HH:mm") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #region So luong khach tai cac diem don
        List<SelectListItem> PrepareHanhTrinhList(bool isAll = true, bool isChonHanhTrinh = false, int HanhTrinhId = 0)
        {
            List<HanhTrinh> hanhtrinhs = new List<HanhTrinh>();
            if (isAll || _workContext.CurrentNhanVien.isQuanTri)
                hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId);
            else
                hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId, _workContext.CurrentNhanVien.VanPhongs.Select(c => c.Id).ToArray());
            var ddls = hanhtrinhs.Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                item.Selected = c.Id == HanhTrinhId;
                return item;
            }).ToList();

            if (isChonHanhTrinh)
                ddls.Insert(0, new SelectListItem { Text = "--------Chọn--------", Value = "0", Selected = 0 == HanhTrinhId });
            return ddls;
        }
        public ActionResult ThongKeTheoDiemDon()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new KhachHangDiemDonModel();
            model.TuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.DenNgay = DateTime.Now;
            model.HanhTrinhs = PrepareHanhTrinhList(true,true);
            return View(model);
        }
        [HttpPost]
        public ActionResult ThongKeTheoDiemDon(DataSourceRequest command, KhachHangDiemDonModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();

            var items = _hanhtrinhService.GetAllDiemDonByNhaXeId(_workContext.NhaXeId);
           
            var Diemdons = items.Select(c =>
            {
                var m = new TongHopDiemDonModel();
                m.DiemDonId = c.Id;
                m.TenDiemDon = c.TenDiemDon;
                var datves = _limousinebanveService.GetAllDatVeDiemDon(model.TuNgay, model.DenNgay, c.Id,model.HanhTrinhId);
                m.SLKhach = datves.Count();
                m.DoanhThu = datves.Sum(n => n.GiaTien);
                return m;
            }).OrderByDescending(c=>c.DoanhThu).ToList();


            var gridModel = new DataSourceResult
            {
                Data = Diemdons,
                Total = Diemdons.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietKhachTheoDiemDon(string tungay, string denngay, int DiemDonId,int HanhTrinhId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new KhachHangDiemDonItemModel();
            model.TuNgay = Convert.ToDateTime(tungay);
            model.DenNgay = Convert.ToDateTime(denngay);
            model.DiemDonId = DiemDonId;
            model.HanhTrinhId = HanhTrinhId;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ChiTietKhachTheoDiemDon(DataSourceRequest command, DateTime TuNgay, DateTime DenNgay, int DiemDonId,int HanhTrinhId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
          
            var datves = _limousinebanveService.GetAllDatVeDiemDon(TuNgay, DenNgay, DiemDonId,HanhTrinhId).Select(c => new DatVeModel
            {
                Ma=c.Ma,
               TenKhachHang=c.khachhang.Ten,
               NgayDi=c.NgayDi,
               GiaTien=c.GiaTien
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = datves,
                Total = datves.Count
            };

            return Json(gridModel);
        }

        #endregion
    }
}