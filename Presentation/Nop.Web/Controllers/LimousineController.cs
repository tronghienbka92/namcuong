using System;
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
using Nop.Web.Models.NhaXeBanVe;
using Nop.Web.Infrastructure.Cache;
using System.IO;

namespace Nop.Web.Controllers
{
    public class LimousineController : BaseNhaXeController
    {
        public const int THOI_GIAN_GHE_DAT_CHO = 120;
        #region Khoi Tao
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IChonVeService _chonveService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IXeInfoService _xeinfoService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILimousineBanVeService _limousinebanveService;
        private readonly ICacheManager _cacheManager;

        public LimousineController(
            ICacheManager cacheManager,
            IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerService customerService,
            IChonVeService chonveService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IStoreService storeService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IXeInfoService xeinfoService,
            IHanhTrinhService hanhtrinhService,
            IPriceFormatter priceFormatter,
            ILimousineBanVeService limousinebanveService
            )
        {
            this._cacheManager = cacheManager;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._chonveService = chonveService;
            this._diachiService = diachiService;
            this._nhanvienService = nhanvienService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._customerSettings = customerSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._storeService = storeService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._xeinfoService = xeinfoService;
            this._hanhtrinhService = hanhtrinhService;
            this._priceFormatter = priceFormatter;
            this._limousinebanveService = limousinebanveService;

        }
        #endregion
        #region Common
        public ActionResult Index()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new LimousineIndexModel();
            int sldv, slchuyen;
            decimal doanhthu;
            _limousinebanveService.GetLimousineIndex(_workContext.NhaXeId, DateTime.Now, ENBaoCaoChuKyThoiGian.HangNgay, out sldv, out doanhthu, out slchuyen);
            model.ngayhientai = new LimousineIndexModel.ThongKe(sldv, doanhthu, slchuyen);
            _limousinebanveService.GetLimousineIndex(_workContext.NhaXeId, DateTime.Now.AddDays(-1), ENBaoCaoChuKyThoiGian.HangNgay, out sldv, out doanhthu, out slchuyen);
            model.ngayhomqua = new LimousineIndexModel.ThongKe(sldv, doanhthu, slchuyen);
            _limousinebanveService.GetLimousineIndex(_workContext.NhaXeId, DateTime.Now, ENBaoCaoChuKyThoiGian.HangThang, out sldv, out doanhthu, out slchuyen);
            model.trongthang = new LimousineIndexModel.ThongKe(sldv, doanhthu, slchuyen);
            return View(model);
        }

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
        List<SelectListItem> PrepareLichTrinhList(int NhaXeId, ref int LichTrinhId, bool isChonLichTrinh = true)
        {
            var _lichtrinhid = LichTrinhId;
            var items = _hanhtrinhService.GetAllLichTrinhByKhungGio(NhaXeId);
            var ddls = items.Select(c =>
           {
               var item = new SelectListItem();
               item.Text = c.toText(false);
               item.Value = c.Id.ToString();
               if (_lichtrinhid > 0)
                   item.Selected = c.Id == _lichtrinhid;
               else
               {
                   //kiem tra thoi gian hien tai de lay thong tin lich trinh hien tai
                   if (!isChonLichTrinh)
                   {
                       DateTime dtfrom = DateTime.Now.Date.AddHours(c.ThoiGianDi.Hour).AddMinutes(c.ThoiGianDi.Minute);
                       DateTime dtto = dtfrom.AddMinutes(c.KhungThoiGian);
                       if (DateTime.Now > dtfrom && DateTime.Now < dtto)
                       {
                           _lichtrinhid = c.Id;
                           item.Selected = true;
                       }

                   }
               }
               return item;
           }).ToList();

            if (isChonLichTrinh)
                ddls.Insert(0, new SelectListItem { Text = "--------Chọn--------", Value = "0", Selected = 0 == LichTrinhId });
            LichTrinhId = _lichtrinhid;
            return ddls;
        }
        List<SelectListItem> PrepareDiemDonList(int HanhTrinhId, int DiemDonId)
        {
            var hanhtrinh = _hanhtrinhService.GetHanhTrinhById(HanhTrinhId);

            var diemdons = hanhtrinh.DiemDons.OrderBy(c=>c.ThuTu).ToList();
            var ddls = diemdons.Select(c =>
        {
            var item = new SelectListItem();
            item.Text = c.diemdon.TenDiemDon;
            item.Value = c.diemdon.Id.ToString();
            item.Selected = c.diemdon.Id == DiemDonId;
            return item;
        }).ToList();
            //bo diem cuoi
            ddls.RemoveAt(ddls.Count - 1);
            return ddls;
        }
        public ActionResult GetLichTrinh(int HanhTrinhId, bool isAddSelect = true)
        {
            var items = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId);
            var itemdatas = items.Select(c => new
            {
                Id = c.Id,
                MoTa = c.toText(false)
            }).ToList();
            if (isAddSelect)
            {
                itemdatas.Insert(0, new
                {
                    Id = 0,
                    MoTa = "--------Chọn--------"
                });
            }
            return Json(itemdatas, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetKhachHangInNhaXe(string ThongTin,bool? isAdv=false,DateTime? NgayDi=null,int HanhTrinhId=0)
        {
            var khachhangs = _limousinebanveService.GetAllHanhKhach(_workContext.NhaXeId, ThongTin,10).Select(m =>
            {
                var khm = m.toModel();
                //lay thong tin cuoi
                if (isAdv.HasValue && isAdv.Value)
                {
                    var _lasdv = _limousinebanveService.GetDatVeCuoiTheoKhachHangId(m.Id, HanhTrinhId);
                    if(_lasdv!=null)
                    {
                        khm.DiemDonId = _lasdv.DiemDonId.GetValueOrDefault(0);
                    }
                    //lay thong di trong ngay
                    if (NgayDi.HasValue)
                    {
                        var chuyendis = _limousinebanveService.GetAllChuyenDiTheoKhachHang(m.Id, NgayDi.Value);
                        khm.ChuyenDiTrongNgay = "";
                        foreach(var cd in chuyendis)
                        {
                            var cdmodel=cd.toModel(_localizationService);
                            if (String.IsNullOrEmpty(khm.ChuyenDiTrongNgay))
                                khm.ChuyenDiTrongNgay = string.Format("{0}({1})",cd.NgayDiThuc.ToString("HH:mm"), cd.hanhtrinh.MaHanhTrinh);
                            else
                                khm.ChuyenDiTrongNgay = khm.ChuyenDiTrongNgay + string.Format("; {0}({1})", cd.NgayDiThuc.ToString("HH:mm"), cd.hanhtrinh.MaHanhTrinh);
                        }
                    }
                }
                return khm;
            }).ToList();
            return Json(khachhangs, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region "Dat ve"

        public ActionResult QuanLyDatVe()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new QuanLyDatVeModel();
            model.NgayDatVe = DateTime.Now;
            model.HanhTrinhs = PrepareHanhTrinhList(false);
            model.HanhTrinhId = Convert.ToInt32(model.HanhTrinhs[0].Value);
            int _lichtrinhid = model.LichTrinhId;
            model.LichTrinhs = PrepareLichTrinhList(_workContext.NhaXeId, ref _lichtrinhid);
            model.TrangThais = this.GetCVEnumSelectList<ENTrangThaiDatVe>(_localizationService, 0);            
            return View(model);
        }
        [HttpPost]
        public ActionResult QuanLyDatVe(DataSourceRequest command, QuanLyDatVeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var items = _limousinebanveService.GetAllDatVe(_workContext.NhaXeId, model.NgayDatVe, model.HanhTrinhId, model.LichTrinhId, model.trangthai, model.ThongTinDatVe);
            if(model.LoaiDonKhachId>0)
            {
                if (model.LoaiDonKhachId == 1)
                    items = items.Where(c => c.isDonTaxi).ToList();
                else
                    items = items.Where(c =>!c.isDonTaxi).ToList();
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    return x.toModel(_localizationService);
                }),
                Total = items.Count
            };

            return Json(gridModel);
        }

        public ActionResult _ChiTietDatVe(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var item = _limousinebanveService.GetDatVeById(Id);
            return PartialView(item.toModel(_localizationService));
        }
        public ActionResult _DatVe(int DatVeId, int ChuyenDiId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            var model = new DatVeModel();
            if (DatVeId > 0)
            {
                var item = _limousinebanveService.GetDatVeById(DatVeId);
                if (item != null)
                {
                    model = item.toModel(_localizationService);
                }
            }
            else
            {
                model.trangthai = ENTrangThaiDatVe.MOI;
                model.HanhTrinhId = chuyendi.HanhTrinhId;
                model.TenHanhTrinh = chuyendi.hanhtrinh.toText();
                model.ChuyenDiId = ChuyenDiId;
                model.TenLichTrinh = chuyendi.lichtrinh.toText(false);
                model.NgayDi = chuyendi.NgayDi;
                model.GioDi = chuyendi.NgayDiThuc;
            }
            model.DiemDons = PrepareDiemDonList(model.HanhTrinhId,model.DiemDonId);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _DatVe(DatVeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            //kiem tra thong tin khach hang
            if(model.KhachHangId>=0)
            {
                if (model.KhachHangId > 0)
                {
                    var _kh = _limousinebanveService.GetKhachHangById(model.KhachHangId);
                    _kh.DienThoai = model.DienThoai;
                    _kh.Ten = model.TenKhachHang;
                    _kh.DiaChi = model.DiaChiNha;
                    _limousinebanveService.UpdateKhachHang(_kh);
                }
                else
                {
                    var _kh = _limousinebanveService.GetKhachHangByDienThoai(_workContext.NhaXeId, model.DienThoai);
                    if (_kh == null)
                        _kh = new KhachHang();
                    _kh.NhaXeId = _workContext.NhaXeId;
                    _kh.DienThoai = model.DienThoai;
                    _kh.Ten = model.TenKhachHang;
                    _kh.DiaChi = model.DiaChiNha;
                    if (_kh.Id > 0)
                        _limousinebanveService.UpdateKhachHang(_kh);
                    else
                        _kh = _limousinebanveService.InsertKhachHang(_kh);
                    model.KhachHangId = _kh.Id;
                }
            }
            
            //insert thong tin dat ve
            if (model.Id > 0)
            {
                var _datveitem = _limousinebanveService.GetDatVeById(model.Id);
                _datveitem.isDonTaxi = model.isDonTaxi;
                _datveitem.DiaChiNha = model.DiaChiNha;
                _datveitem.GhiChu = model.GhiChu;
                _datveitem.DiemDonId = model.DiemDonId;
                if (model.KhachHangId>0)
                    _datveitem.KhachHangId = model.KhachHangId;
                _datveitem.isThanhToan = model.isThanhToan;
                _datveitem.isNoiBai = model.isNoiBai;
                _datveitem.TenKhachHangDiKem = model.TenKhachHang;
                _limousinebanveService.UpdateDatVe(_datveitem);
            }
            else
            {
                //lay tat ca dat ve cung session
                var _datveitems = _limousinebanveService.GetDatVeBySession(_workContext.NhaXeId, model.ChuyenDiId, getDatGheSession);
                foreach (var _datveitem in _datveitems)
                {
                    _datveitem.isDonTaxi = model.isDonTaxi;
                    _datveitem.DiaChiNha = model.DiaChiNha;
                    _datveitem.GhiChu = model.GhiChu;
                    _datveitem.DiemDonId = model.DiemDonId;
                    _datveitem.KhachHangId = model.KhachHangId;
                    _datveitem.isNoiBai = model.isNoiBai;
                    _datveitem.isThanhToan = model.isThanhToan;
                    _datveitem.trangthai = ENTrangThaiDatVe.DA_XEP_CHO;
                    _limousinebanveService.UpdateDatVe(_datveitem);
                }
            }
            clearDatGheSession();
            return ThanhCong();
        }

        private string getDatGheSession
        {
            get
            {
                if (Session["ChonGheGroup"] == null)
                {
                    Session["ChonGheGroup"] = Guid.NewGuid().ToString();
                }
                return Session["ChonGheGroup"].ToString();
            }

        }
        private void clearDatGheSession()
        {
            Session["ChonGheGroup"] = null;
        }
        private DatVe DatCho(int ChuyenDiId, int SoDoGheId)
        {
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            if (chuyendi == null)
            {
                return null;
            }
            //trang thai khong hop le
            if (chuyendi.trangthai == ENTrangThaiXeXuatBen.KET_THUC || chuyendi.trangthai == ENTrangThaiXeXuatBen.HUY)
            {
                return null;
            }
            //kiem tra xem co ai dat chua
            var checkdatve = chuyendi.DatVes.Where(c => c.SoDoGheId == SoDoGheId && c.TrangThaiId != (int)ENTrangThaiDatVe.HUY).FirstOrDefault();
            if (checkdatve != null)
            {
                //neu co roi thi kiem tra thoi gian dat trc do neu chua qua 2 phut thi out
                if (checkdatve.NgayTao.AddSeconds(THOI_GIAN_GHE_DAT_CHO) > DateTime.Now)
                    return null;
                else
                {
                    //kiem tra trang thai ghe 
                    if (checkdatve.trangthai == ENTrangThaiDatVe.MOI)
                    {
                        //huy ghe nay di                        
                        _limousinebanveService.DeleteDatVe(checkdatve);
                       
                    }
                    else
                        return null;

                }
            }
            //tao thong tin dat ve
            var datve = new DatVe();
            datve.NhaXeId = _workContext.NhaXeId;
            datve.ChuyenDiId = ChuyenDiId;
            datve.SoDoGheId = SoDoGheId;
            datve.NguoiTaoId = _workContext.CurrentNhanVien.Id;
            datve.NgayDi = chuyendi.NgayDi;
            datve.LichTrinhId = chuyendi.LichTrinhId;
            datve.HanhTrinhId = chuyendi.HanhTrinhId;
            datve.trangthai = ENTrangThaiDatVe.MOI;
            datve.SessionID = getDatGheSession;
            //add by lent : 07/08/2016
            //su gia tien theo cau hinh gia ve so do ghe id
            datve.GiaTien = _limousinebanveService.GetGiaVeTheoSoDoId(chuyendi.HanhTrinhId, chuyendi.lichtrinhloaixe.LoaiXeId, SoDoGheId);
            //lay gia ve theo cau hinh gia ve theo lich trinh
            if (datve.GiaTien==decimal.Zero)
                datve.GiaTien = chuyendi.lichtrinhloaixe.GiaVe;
            _limousinebanveService.InsertDatVe(datve);
            return datve;
        }
        [HttpPost]
        public ActionResult ChonGheDatCho(int ChuyenDiId, int SoDoGheId)
        {
            //tao nhanh chuyen di
            var datveitem = DatCho(ChuyenDiId, SoDoGheId);
            if (datveitem == null)
            {
                return Loi();
            }           
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult HuyGheDatCho(int DatVeId)
        {
            //tao nhanh chuyen di
            var datve = _limousinebanveService.GetDatVeById(DatVeId);
            if (datve == null)
            {
                return Loi();
            }
            //datve thai khong hop le
            if (datve.trangthai != ENTrangThaiDatVe.MOI)
            {
                return Loi();
            }
            
            //xem co phai la cua nguoi dang dat ve ko, tranh truong hop nham
            if (datve.NguoiTaoId != _workContext.CurrentNhanVien.Id)
            {
                if (datve.NgayTao.AddSeconds(THOI_GIAN_GHE_DAT_CHO) > DateTime.Now)
                {
                    return Loi();
                }                
            }
            //huy ve            
            //datve.trangthai = ENTrangThaiDatVe.HUY;
            _limousinebanveService.DeleteDatVe(datve);
            //ok, co the huy
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult HuyDatVe(int DatVeId)
        {
            //tao nhanh chuyen di
            var datve = _limousinebanveService.GetDatVeById(DatVeId);
            if (datve == null)
            {
                return Loi();
            }
            //datve thai khong hop le
            if (datve.trangthai != ENTrangThaiDatVe.DA_XEP_CHO)
            {
                return Loi();
            }
            //huy ve            
            datve.trangthai = ENTrangThaiDatVe.HUY;
            
            _limousinebanveService.UpdateDatVe(datve);
            //ok, co the huy, luu lai nhat ky
            var note = "HD " + datve.Ma + " được hủy bởi " + _workContext.CurrentNhanVien.HoVaTen;
            _limousinebanveService.InsertDatVeNote(datve.Id,note);
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult XacNhanVe(int DatVeId)
        {
            //tao nhanh chuyen di
            var datve = _limousinebanveService.GetDatVeById(DatVeId);
            if (datve == null)
            {
                return Loi();
            }
            //datve thai khong hop le
            if (datve.trangthai != ENTrangThaiDatVe.DA_XEP_CHO)
            {
                return Loi();
            }
            //xac nhan va huy xac nhan            
            datve.isDaXacNhan = !datve.isDaXacNhan;
            _limousinebanveService.UpdateDatVe(datve);
            //ok, luu nhat ky
            var note= "HD " + datve.Ma + " được xác nhận bởi " + datve.nguoitao.HoVaTen;
          _limousinebanveService.InsertDatVeNote(datve.Id,note);
           
            return ThanhCong();
        }
        public ActionResult _LenhTaxi(int DatVeId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var item = _limousinebanveService.GetDatVeById(DatVeId);
            return PartialView(item.toModel(_localizationService));

        }
        [HttpPost]
        public ActionResult _LenhTaxi(int DatVeId, string MaTaXi)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            //kiem tra thong tin khach hang

            var _datveitem = _limousinebanveService.GetDatVeById(DatVeId);
            _datveitem.isLenhDonTaXi = true;
            _datveitem.MaTaXi = MaTaXi;
            _limousinebanveService.UpdateDatVe(_datveitem);
            return ThanhCong();
        }

        public ActionResult KhachHangChuyenVe(int DatVeId)
        {
            var datve = _limousinebanveService.GetDatVeById(DatVeId);
            var model = new QuanLyChuyenModel();
            if(datve==null)
            {
                return PartialView(model);
            }
            
            model.DatVeIdChuyenVe = DatVeId;
            model.ChuyenDiIdChuyenVe = datve.ChuyenDiId.GetValueOrDefault(0);
            //lay ta ca chuyen di trong ngay vs tuong lai
            model.ChuyenDiChuyenVes = _limousinebanveService.GetAllChuyenDi(_workContext.NhaXeId, datve.HanhTrinhId, datve.NgayDi).Select(c=> new SelectListItem {
                Value=c.Id.ToString(),
                Text=string.Format("{0} - {1} - {2}",c.Ma,c.lichtrinh.toText(false),c.lichtrinhloaixe.loaixe.TenLoaiXe),
                Selected = c.Id == model.DatVeIdChuyenVe
            }).ToList();

            return PartialView(model);
        }
        public ActionResult _TabDatVeCopy()
        {
            var model = new QuanLyChuyenModel();
            model.arrDatVeCopy = ChuyenVeCopyGet();
            return PartialView(model);
        }
        
        /// <summary>
        /// Kiem tra co dang copy chuyen ve khong
        /// </summary>
        /// <returns></returns>
        private bool CheckChuyenVeCopy()
        {            
            var arrDatVeCopy = ChuyenVeCopyGet();
            if (arrDatVeCopy.Count == 0) return false;
            return true;
        }
        private List<DatVeCopyModel> ChuyenVeCopyGet()
        {
            var arrDatVeCopy = new List<DatVeCopyModel>();
            if (Session["ChuyenVeDatVeIds"] != null)
            {
                arrDatVeCopy = (List<DatVeCopyModel>)Session["ChuyenVeDatVeIds"];
            }
            return arrDatVeCopy;
        }
        private void ChuyenVeCopyAdd(DatVe item)
        {
            var arrDatVeCopy = ChuyenVeCopyGet();            
            var itemcopy = new DatVeCopyModel();
            itemcopy.Id = item.Id;
            itemcopy.Ma = item.Ma;
            if(!arrDatVeCopy.Where(c=>c.Id==itemcopy.Id).Any())
            {
                arrDatVeCopy.Add(itemcopy);
            }
            Session["ChuyenVeDatVeIds"] = arrDatVeCopy;
        }
        private void ChuyenVeCopyDelete(int DatVeId)
        {
            var arrDatVeCopy = ChuyenVeCopyGet();            
            var itemcopy = arrDatVeCopy.Where(c => c.Id == DatVeId).FirstOrDefault();
            if (itemcopy != null)
                arrDatVeCopy.Remove(itemcopy);
            Session["ChuyenVeDatVeIds"] = arrDatVeCopy;
        }
        [HttpPost]
        public ActionResult ChuyenVeCopy(int DatVeId,int ChuyenDiId)
        {            
            //copy tung ve 
            if(DatVeId>0)
            {
                var datve = _limousinebanveService.GetDatVeById(DatVeId);
                if (datve == null)
                {
                    return Loi();
                }
                //datve thai khong hop le
                if (datve.trangthai != ENTrangThaiDatVe.DA_XEP_CHO)
                {
                    return Loi();
                }
                ChuyenVeCopyAdd(datve);

            }
            else if (DatVeId==-1)
            {
                //copy ta ca ve trong chuyen di
                var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
                var datves=chuyendi.DatVeHopLes();
                foreach (var dv in datves)
                {
                    ChuyenVeCopyAdd(dv);
                }
            }
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult ChuyenVeDelete(int DatVeId)
        {           
            //xoa tung ve 
            if (DatVeId > 0)
            {
                ChuyenVeCopyDelete(DatVeId);
            }
            else if (DatVeId == -1)
            {
                //xóa ta ca ve da copy
                Session["ChuyenVeDatVeIds"] = null;
            }
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult ChuyenVePaste(int ChuyenDiId, int SoDoGheId)
        {
            var arrDatVeId = ChuyenVeCopyGet();
            if (arrDatVeId.Count == 0)
                return Loi();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            var arrdatve = chuyendi.DatVes.Where(c => c.trangthai != ENTrangThaiDatVe.HUY);
            //kiem tra xem co du cho trong tren chuyen di nay khong
            if (arrDatVeId.Count > chuyendi.lichtrinhloaixe.loaixe.sodoghe.SoLuongGhe - arrdatve.Count())
            {
                return Loi(string.Format("Chuyến đi không đủ chỗ trống, bạn hãy kiểm tra lại (số lượng vé chuyển là: {0})",arrDatVeId.Count));
            }
            //OK, se uu tien ghe copy dau tien cho SoDoGheId
            var arrSoDoGheId = new List<int>();
            arrSoDoGheId.Add(SoDoGheId);

            var sodoghequytacs = _xeinfoService.GetAllSoDoGheXeQuyTac(chuyendi.lichtrinhloaixe.LoaiXeId);
            //tim tat ca vi tri con trong
            if (sodoghequytacs != null && sodoghequytacs.Count > 0)
            {
                foreach (var s in sodoghequytacs)
                {                    
                    if (s.y >= 1 && s.x >= 1)
                    {
                        //xem tai vi tri nay co nguoi dat chua
                        bool isEmpty = true;
                        foreach (var dv in arrdatve)
                        {
                            //kiem tra thong tin ve da het han 2 phut
                            if (dv.trangthai == ENTrangThaiDatVe.MOI && dv.NgayTao.AddSeconds(THOI_GIAN_GHE_DAT_CHO) < DateTime.Now)
                            {
                                continue;
                            }
                            if (dv.SoDoGheId == s.Id)
                            {
                                isEmpty = false;
                                break;
                            }
                        }
                        if (isEmpty && !arrSoDoGheId.Contains(s.Id))
                        {
                            arrSoDoGheId.Add(s.Id);
                            
                        }
                    }
                }
            }
            bool isHasErr = false;
            for (int i = 0; i < arrDatVeId.Count;i++)
            {
                bool isOK = false;
                while (!isOK)
                {
                    isOK = ChuyenVaDatVe(arrDatVeId[i].Id, ChuyenDiId, arrSoDoGheId[0]);
                    if (arrSoDoGheId.Count>0)
                    {
                        //loai bo vi tri ghe dau tien
                        arrSoDoGheId.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
                //neu het vong chuyen ma ko chuyen dc thi bao loi
                //cho nay se co mot so ve chuyen dc, mot so ve khong chuyen dc
                if (!isOK)
                {
                    isHasErr = true;
                }
                if (arrSoDoGheId.Count == 0)
                    break;
            }
            clearDatGheSession();
            Session["ChuyenVeDatVeIds"] = null;
            if (isHasErr)
                return Loi("Có lỗi trong quá trình chuyển vé");
            return ThanhCong();
        }
        bool ChuyenVaDatVe(int DatVeId, int ChuyenDiId, int SoDoGheId)
        {
            var _datveitemnew = DatCho(ChuyenDiId, SoDoGheId);
            if (_datveitemnew == null)
            {
                return false;
            }
            //update thong tin old- > new
            var _datveitemold = _limousinebanveService.GetDatVeById(DatVeId);
            _datveitemnew.isDonTaxi = _datveitemold.isDonTaxi;
            _datveitemnew.DiaChiNha = _datveitemold.DiaChiNha;
            _datveitemnew.GhiChu = _datveitemold.GhiChu;
            _datveitemnew.DiemDonId = _datveitemold.DiemDonId;
            _datveitemnew.KhachHangId = _datveitemold.KhachHangId;
            _datveitemnew.isThanhToan = _datveitemold.isThanhToan;
            _datveitemnew.isNoiBai = _datveitemold.isNoiBai;
            _datveitemnew.trangthai = _datveitemold.trangthai;
            _datveitemnew.isLenhDonTaXi = _datveitemold.isLenhDonTaXi;
            _datveitemnew.MaTaXi = _datveitemold.MaTaXi;
            _datveitemnew.isDaXacNhan = _datveitemold.isDaXacNhan;
            _datveitemnew.VeChuyenDenId = _datveitemold.Id;
            _datveitemnew.TenKhachHangDiKem = _datveitemold.TenKhachHangDiKem;
            _limousinebanveService.UpdateDatVe(_datveitemnew);
            //huy ve cu
            _datveitemold.trangthai = ENTrangThaiDatVe.HUY;
            _datveitemold.GhiChu = _datveitemold.GhiChu + string.Format("Lý do hủy: Chuyển sang chuyến đi mới (Id={0})", ChuyenDiId);
            _limousinebanveService.UpdateDatVe(_datveitemold);
            // luu lai nhat ky
            var note = "HD " + _datveitemold.Ma + " được chuyển sang " + _datveitemnew.chuyendi.Ma +" lúc "+ _datveitemnew.chuyendi.NgayDiThuc + " bởi " + _datveitemnew.nguoitao.HoVaTen;
            _limousinebanveService.InsertDatVeNote(_datveitemold.Id,note);

            return true;
        }
        [HttpPost]
        public ActionResult ChuyenVe(int DatVeId, int ChuyenDiId, int SoDoGheId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            //kiem tra vi tri hien tai con trong ko ?
            if (!ChuyenVaDatVe(DatVeId, ChuyenDiId, SoDoGheId))
                return Loi();
            clearDatGheSession();
            return ThanhCong();
        }
        #endregion
        #region "Quan ly chuyen di"
        public ActionResult QuanLyChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new QuanLyChuyenModel();
            model.NgayDi = DateTime.Now;
            model.HanhTrinhs = PrepareHanhTrinhList(false);
            model.HanhTrinhId = Convert.ToInt32(model.HanhTrinhs[0].Value);
            model.KhungGioId = (int)CommonHelper.KhungGioHienTai();
            model.khunggios = this.GetCVEnumSelectList<ENKhungGio>(_localizationService, model.KhungGioId);
            return View(model);
        }
        public ActionResult _TabLichTrinh(int HanhTrinhId, int KhungGioId)
        {
            var model = new QuanLyChuyenModel();
            model.LichTrinhs = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId, (ENKhungGio)KhungGioId);
            if (model.LichTrinhs.Count > 0)
            {
                //lui lai 3 lich trinh trc
                model.LichTrinhStepId = -3;
                //lay thong tin lich trinh gan nhat
                foreach (var lt in model.LichTrinhs)
                {
                    var _thoigiandi = DateTime.Now.Date.AddHours(lt.ThoiGianDi.Hour).AddMinutes(lt.ThoiGianDi.Minute);
                    if (_thoigiandi > DateTime.Now)
                    {
                        model.LichTrinhId = lt.Id;
                        break;
                    }
                    else
                        model.LichTrinhId = lt.Id;
                    model.LichTrinhStepId++;
                }
            }

            return PartialView(model);
        }
        public ActionResult _TabChuyenDi(int HanhTrinhId, int LichTrinhId,int KhungGioId, DateTime NgayDi,string ThongTinKhachHang)
        {
            var model = new QuanLyChuyenModel();
            model.isTaoChuyen = this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen);
            //lay thong tin chuyen di
            model.chuyendis = _limousinebanveService.GetAllChuyenDi(_workContext.NhaXeId, HanhTrinhId, LichTrinhId, (ENKhungGio)KhungGioId, NgayDi, "", ThongTinKhachHang).Select(c => { return c.toModel(_localizationService); }).ToList();
            if (model.chuyendis.Count > 0)
            {                
                model.LichTrinhStepId = - 6;
                //lay thong tin chuyen di gan nhat
                foreach (var cd in model.chuyendis)
                {
                    var _thoigiandi = DateTime.Now.Date.AddHours(cd.NgayDiThuc.Hour).AddMinutes(cd.NgayDiThuc.Minute);
                    model.ChuyenDiId = cd.Id;
                    if (_thoigiandi > DateTime.Now)
                    {                        
                        break;
                    }                    
                    model.LichTrinhStepId++;
                }
            }

            return PartialView(model);
        }
        void TaoNhatKyChuyenDi(int ChuyenDiId, ENTrangThaiXeXuatBen trangthai, string GhiChu)
        {
            //tao log
            var item = new HistoryXeXuatBenLog();
            item.TrangThai = trangthai;
            item.GhiChu = GhiChu;
            item.XeXuatBenId = ChuyenDiId;
            item.NguoiTaoId = _workContext.CurrentNhanVien.Id;
            _nhaxeService.InsertHistoryXeXuatBenLog(item);
        }
        public ActionResult _ChinhSuaChuyen(int? LichTrinhId, int? HanhTrinhId)
        {
            var model = new ChuyenDiModel();
            //lay thong tin chuyen di
            int _lichtrinhid = LichTrinhId.GetValueOrDefault(0);
            model.HanhTrinhId = HanhTrinhId.GetValueOrDefault(0);
            model.LichTrinhs = PrepareLichTrinhList(_workContext.NhaXeId, ref _lichtrinhid, false);
            model.LichTrinhId = _lichtrinhid;
            model.LoaiXes = _xeinfoService.GetAllByNhaXeId(_workContext.NhaXeId).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.TenLoaiXe
            }).ToList();
            var lichtrinh = _hanhtrinhService.GetLichTrinhById(_lichtrinhid);
            if (lichtrinh != null)
                model.NgayDiThuc = DateTime.Now.Date.AddHours(lichtrinh.ThoiGianDi.Hour).AddMinutes(lichtrinh.ThoiGianDi.Minute);
            else
                model.NgayDiThuc = DateTime.Now.AddMinutes(15);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult TaoMoiChuyenDi(int HanhTrinhId, string ThoiGianDi, int LoaiXeId, DateTime NgayDi)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            DateTime _thoigiandi = Convert.ToDateTime(ThoiGianDi);
            //lay thong tin lich trinh tu thoi gian di thuc
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId);
            LichTrinh _lichtrinh = null;
            _thoigiandi = NgayDi.Date.AddHours(_thoigiandi.Hour).AddMinutes(_thoigiandi.Minute);
            foreach(var lt in lichtrinhs)
            {
                _lichtrinh = lt;
                DateTime fromDate = NgayDi.Date.AddHours(lt.ThoiGianDi.Hour).AddMinutes(lt.ThoiGianDi.Minute);
                DateTime toDate=fromDate.AddMinutes(lt.KhungThoiGian);                
                //neu nam trong khoang thi break
                if (_thoigiandi >= fromDate && _thoigiandi < toDate)
                {
                    break;
                }
            }
            //tao nhanh chuyen di
            var chuyendi = new ChuyenDi();
            chuyendi.HanhTrinhId = HanhTrinhId;
            chuyendi.LichTrinhId = _lichtrinh.Id;
            chuyendi.NgayDi = NgayDi.AddHours(_lichtrinh.ThoiGianDi.Hour).AddMinutes(_lichtrinh.ThoiGianDi.Minute);
            chuyendi.NgayDiThuc = _thoigiandi;
            chuyendi.NguoiTaoId = _workContext.CurrentNhanVien.Id;
            chuyendi.NhaXeId = _workContext.NhaXeId;
            chuyendi.trangthai = ENTrangThaiXeXuatBen.DU_KIEN;
            var lichtrinhloaixe = _limousinebanveService.GetLichTrinhLoaiXe(HanhTrinhId, _lichtrinh.Id, LoaiXeId);
            chuyendi.LichTrinhLoaiXeId = lichtrinhloaixe.Id;
            _limousinebanveService.InsertChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Tạo mới thông tin chuyến đi");
            return ThanhCong();
        }

        [HttpPost]
        public ActionResult HuyChuyenDi(int ChuyenDiId)
        {
            //tao nhanh chuyen di
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            if (chuyendi == null)
            {
                return Loi();
            }
            //trang thai khong hop le
            if (chuyendi.trangthai != ENTrangThaiXeXuatBen.DU_KIEN)
            {
                return Loi();
            }
            //kiem tra tren chuyen di co khach hang khong ?
            if (chuyendi.DatVeHopLes().Count > 0)
            {
                return Loi();
            }
            //ok, co the huy
            chuyendi.trangthai = ENTrangThaiXeXuatBen.HUY;
            _limousinebanveService.UpdateChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Hủy thông tin chuyến đi");
            return ThanhCong();
        }
        
        [NonAction]
        protected virtual void SoDoGheXeToSoDoGheXeModel(SoDoGheXe nvfrom, QuanLyChuyenModel.SoDoGheXeModel nvto)
        {
            nvto.Id = nvfrom.Id;
            nvto.TenSoDo = nvfrom.TenSoDo;
            nvto.UrlImage = nvfrom.TenSoDo;
            nvto.SoLuongGhe = nvfrom.SoLuongGhe;
            nvto.KieuXeId = nvfrom.KieuXeId;
            nvto.SoCot = nvfrom.SoCot;
            nvto.SoHang = nvfrom.SoHang;
        }
        QuanLyChuyenModel createChuyenDiModel(int ChuyenDiId, ENPhanLoaiPhoiVe PhanLoai)
        {
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            var model = new QuanLyChuyenModel();
            model.isCheckChuyenVe = CheckChuyenVeCopy();
            model.NhanVienHientTaiId = _workContext.CurrentNhanVien.Id;
            model.chuyendihientai = chuyendi.toModel(_localizationService);
            //lay thong tin so do ghe
            var loaixe = chuyendi.lichtrinhloaixe.loaixe;
            var sodoghe = _xeinfoService.GetSoDoGheXeById(loaixe.SoDoGheXeID);

            var modelsodoghe = new QuanLyChuyenModel.SoDoGheXeModel();
            modelsodoghe.PhanLoai = PhanLoai;
            SoDoGheXeToSoDoGheXeModel(sodoghe, modelsodoghe);

            //Lấy thông tin ma tran
            var sodoghevitris = _xeinfoService.GetAllSoDoGheViTri(sodoghe.Id);
            var sodoghequytacs = _xeinfoService.GetAllSoDoGheXeQuyTac(loaixe.Id);

            modelsodoghe.MaTran = new int[modelsodoghe.SoHang, modelsodoghe.SoCot];
            modelsodoghe.DatVes = new DatVeModel[modelsodoghe.SoHang + 1, modelsodoghe.SoCot + 1];

            foreach (var s in sodoghevitris)
            {
                modelsodoghe.MaTran[s.y, s.x] = 1;
            }

            DateTime _ngaydi = chuyendi.NgayDi;
            var arrdatve = chuyendi.DatVes.Where(c => c.trangthai != ENTrangThaiDatVe.HUY).ToList();
            if (PhanLoai == ENPhanLoaiPhoiVe.IN_PHOI_VE)
            {
                arrdatve = chuyendi.DatVes.Where(c => c.trangthai != ENTrangThaiDatVe.HUY || (c.trangthai == ENTrangThaiDatVe.HUY && c.isKhachHuy)).ToList();
            }
            if (sodoghequytacs != null && sodoghequytacs.Count > 0)
            {
                foreach (var s in sodoghequytacs)
                {

                    modelsodoghe.DatVes[s.y, s.x] = new DatVeModel();
                    modelsodoghe.DatVes[s.y, s.x].sodoghekyhieu = s.Val;
                    modelsodoghe.DatVes[s.y, s.x].SoDoGheId = s.Id;
                    modelsodoghe.DatVes[s.y, s.x].trangthai = ENTrangThaiDatVe.CON_TRONG;
                    if (s.y >= 1 && s.x >= 1)
                    {
                        //xem tai vi tri nay co nguoi dat chua
                        foreach (var dv in arrdatve)
                        {
                            //kiem tra thong tin ve da het han 2 phut
                            if (dv.trangthai==ENTrangThaiDatVe.MOI && dv.NgayTao.AddSeconds(THOI_GIAN_GHE_DAT_CHO) < DateTime.Now)
                            {
                                ///_limousinebanveService.DeleteDatVe(dv);
                                continue;
                            }                                
                            if (dv.SoDoGheId == s.Id)
                            {
                                
                                modelsodoghe.DatVes[s.y, s.x] = dv.toModel(_localizationService);
                                break;
                            }
                        }
                    }

                }
            }
            model.sodoghe = modelsodoghe;
            return model;
        }
        public ActionResult _TabSoDoXe(int ChuyenDiId, int? PhanLoai)
        {
            return PartialView(createChuyenDiModel(ChuyenDiId, (ENPhanLoaiPhoiVe)PhanLoai.GetValueOrDefault(0)));
        }


        public ActionResult DanhSachChuyenDi()
        {
            //if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
            //    return AccessDeniedView();
            var model = new DanhSachChuyenDiModel();
            model.IsQuyenTaoChuyen = this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen);
           
          
            if( model.IsQuyenTaoChuyen)
            {
                model.HanhTrinhs = PrepareHanhTrinhList(true);
            }
            model.NgayDi = DateTime.Now;
            model.HanhTrinhs = PrepareHanhTrinhList(false);
            model.HanhTrinhId = Convert.ToInt32(model.HanhTrinhs[0].Value);
            model.KhungGioId = (int)CommonHelper.KhungGioHienTai();
            model.khunggios = this.GetCVEnumSelectList<ENKhungGio>(_localizationService, model.KhungGioId);
            //lay tat ca nhan vien
            model.AllLaiXePhuXes = _nhaxeService.GetAllNhanVienByNhaXe(_workContext.NhaXeId, new ENKieuNhanVien[] { ENKieuNhanVien.LaiXe, ENKieuNhanVien.PhuXe }).Select(c =>
            {
                return new XeXuatBenItemModel.NhanVienLaiPhuXe(c.Id, c.ThongTin());
            }).ToList();
            //lay tat ca thong tin xe
            model.AllXeInfo = _xeinfoService.GetAllXeInfoByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                return new XeXuatBenItemModel.XeVanChuyenInfo(c.Id, c.BienSo);
            }).ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult _DanhSachChuyenDi(DanhSachChuyenDiModel model)
        {
            model.IsQuyenTaoChuyen = this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen);

           
            var items = _limousinebanveService.GetAllChuyenDi(_workContext.NhaXeId, model.HanhTrinhId, 0, model.khunggio, model.NgayDi, model.ThongTinChuyenDi).OrderByDescending(c => c.NgayDiThuc).ThenByDescending(c => c.Id).ToList();
            model.ChuyenDis = items.Select(c =>
            {
                return c.toModel(_localizationService);
            }).ToList();
            return PartialView(model);
        }
        #region cap nhat chi phi
        public ActionResult _CapNhatChiPhiChuyenDi(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
           var model=new ChuyenDiThuChiModel();
           model.ChuyenDiId=Id;
            model.ThuChis =  this.GetCVEnumSelectList<ENLoaiChiPhi>(_localizationService, model.LoaiChiPhiId).Select(c =>
            {
                var item = new ThuChiModel();
                item.TenLoaiThuChi = c.Text;
                item.ChuyenDiThuChiId = Convert.ToInt32(c.Value);
                var thamso=_limousinebanveService.GetThamSoByLoaiThamSo((ENLoaiChiPhi)item.ChuyenDiThuChiId);
                item.SoTien = thamso.GiaTri;
                if((ENLoaiChiPhi)item.ChuyenDiThuChiId==ENLoaiChiPhi.CHI_NOI_BAI)
                {
                    var datves = _limousinebanveService.GetDatVeByChuyenDi(Id);
                    item.SoTien = datves.Where(m => m.isNoiBai).Count() * thamso.GiaTri;
                }
                var _chuyendithuchi = _limousinebanveService.GetChuyenDiThuChiByChuyenDi(Id, item.ChuyenDiThuChiId);
                if (_chuyendithuchi != null)
                {
                    item.LoaiThuChiCheck = true;
                    item.SoTien = _chuyendithuchi.SoTien;
                }
                return item;
            }).ToList();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _CapNhatChiPhiChuyenDi(DataSourceRequest command, ChuyenDiThuChiModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            //xoa het tat ca lai xe ton tai truc do
            _limousinebanveService.DeleteChuyenDiThuChi(model.ChuyenDiId);
            //inset nhung lai xe moi vao trung gian
            foreach (var item in model.ThuChis)
            {
                if (item.LoaiThuChiCheck == true)
                {
                    var _item = new ChuyenDiThuChi();
                    _item.ChuyenId = model.ChuyenDiId;
                    _item.LoaiTaiChinhThuChiId = item.ChuyenDiThuChiId;
                    _item.SoTien = item.SoTien;
                    _limousinebanveService.InsertChuyenDiThuChi(_item);
                }

            }
            return ThanhCong();
        }
        #endregion
        public ActionResult PrinfPhoiVe(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            return View(createChuyenDiModel(Id, ENPhanLoaiPhoiVe.IN_PHOI_VE));
        }
        [HttpPost]
        public ActionResult ThietLapChuyenDi(int LaiXeId, int XeVanChuyenId, int ChuyenDiId, string ThoiGianDi)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            chuyendi.LaiXeId = LaiXeId;
            chuyendi.XeVanChuyenId = XeVanChuyenId;
            if (chuyendi.trangthai == ENTrangThaiXeXuatBen.DU_KIEN)
            {
                chuyendi.trangthai = ENTrangThaiXeXuatBen.CHO_XUAT_BEN;
                chuyendi.STTChuyen = _limousinebanveService.GetSTTChuyenDi(_workContext.NhaXeId, chuyendi.NgayDi);
            }            
            if (!string.IsNullOrEmpty(ThoiGianDi))
            {
                DateTime _thoigiandi = Convert.ToDateTime(ThoiGianDi);
                chuyendi.NgayDiThuc = chuyendi.NgayDiThuc.Date.AddHours(_thoigiandi.Hour).AddMinutes(_thoigiandi.Minute);
            }
            _limousinebanveService.UpdateChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Thiết lập biển số xe và lái xe cho chuyến đi");
            return Json(chuyendi.toModel(_localizationService));

        }
        [HttpPost]
        public ActionResult XuatBenChuyenDi(int LaiXeId, int XeVanChuyenId, int ChuyenDiId, string ThoiGianDi)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            chuyendi.LaiXeId = LaiXeId;
            chuyendi.XeVanChuyenId = XeVanChuyenId;
            
                chuyendi.trangthai = ENTrangThaiXeXuatBen.DANG_DI;
                    
            if (!string.IsNullOrEmpty(ThoiGianDi))
            {
                DateTime _thoigiandi = Convert.ToDateTime(ThoiGianDi);
                chuyendi.NgayDiThuc = chuyendi.NgayDiThuc.Date.AddHours(_thoigiandi.Hour).AddMinutes(_thoigiandi.Minute);
            }
            _limousinebanveService.UpdateChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Xuất bến cho chuyến đi");
            return Json(chuyendi.toModel(_localizationService));

        }
        [HttpPost]
        public ActionResult HuyThietLapChuyenDi(int ChuyenDiId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            chuyendi.LaiXeId = null;
            chuyendi.XeVanChuyenId = null;
            chuyendi.trangthai = ENTrangThaiXeXuatBen.DU_KIEN;
            chuyendi.STTChuyen = 0;
            _limousinebanveService.UpdateChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Hủy thiết lập biển số xe và lái xe của chuyến đi");
            return Json(chuyendi.toModel(_localizationService));

        }
        [HttpPost]
        public ActionResult KetThutChuyenDi(int ChuyenDiId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var chuyendi = _limousinebanveService.GetChuyenDiById(ChuyenDiId);
            chuyendi.trangthai = ENTrangThaiXeXuatBen.KET_THUC;
            _limousinebanveService.UpdateChuyenDi(chuyendi);
            TaoNhatKyChuyenDi(chuyendi.Id, chuyendi.trangthai, "Kết thúc chuyến đi");
            return Json(chuyendi.toModel(_localizationService));

        }

        public ActionResult _ChitietKhachTheoChuyen(int ChuyenDiId)
        {
            //if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
            //    return AccessDeniedView();
            return PartialView(createChuyenDiModel(ChuyenDiId, ENPhanLoaiPhoiVe.IN_PHOI_VE));
        }
        public ActionResult _NhatKyXeXuatBen(int Id)
        {
            //if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
            //    return AccessDeniedView();
            var model = new XeXuatBenItemModel();
            model.Id = Id;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult GetNhatKyXeXuatBen(int Id)
        {
            //if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
            //    return AccessDeniedView();
            var xexuatben = _limousinebanveService.GetChuyenDiById(Id);
            var nhatkymodels = xexuatben.NhatKys.OrderByDescending(c => c.Id).Select(c =>
            {
                var model = new XeXuatBenItemModel.NhatKyXeXuatBen();
                model.NgayTao = c.NgayTao;
                model.GhiChu = c.GhiChu;
                model.TenNguoiTao = c.NguoiTao.HoVaTen;
                model.NguoiTaoId = c.NguoiTaoId;
                model.Id = c.Id;
                return model;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = nhatkymodels,
                Total = xexuatben.NhatKys.Count
            };

            return Json(gridModel);
        }
        public ActionResult _NhatKyDatVe(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new DatVeModel();
            model.Id = Id;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult GetNhatKyDatVe(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var nhatkydatves = _limousinebanveService.GetLichSuDatVeByDatVeId(Id).Select(c =>
                {
                    var _item = new DatVeNoteItem();
                    _item.NgayTao = c.NgayTao;
                    _item.NguoiTao = c.DatVe.nguoitao.TenVaHo;
                    _item.Note = c.Note;
                    return _item;
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = nhatkydatves,
                Total = nhatkydatves.Count
            };

            return Json(gridModel);
        }
        #endregion
        #region Thong tin khach hang            
        public ActionResult QuanLyKhachHang()
        {
            
            var model =new  QuanLyKhachHangModel();
            model.TuNgay = DateTime.Now.AddDays(-7);
            model.DenNgay = DateTime.Now;
            return View(model);
        }
        [HttpPost]
        public ActionResult QuanLyKhachHang(DataSourceRequest command, QuanLyKhachHangModel model)
        {
            
            if (model.LoaiTimKiemId==0)
            {
                //var items = _limousinebanveService.GetAllHanhKhach(_workContext.NhaXeId, model.ThongTinKhachHang);
                var items = _limousinebanveService.GetAllHanhKhach(_workContext.NhaXeId, model.TuNgay,model.DenNgay,model.ThongTinKhachHang);
                var gridModel = new DataSourceResult
                {
                    Data = items.Select(x =>
                    {
                        var modelkh = x.toModel();
                        int slhuy;
                        modelkh.SoLuongDat = _limousinebanveService.GetSoLuongDatVeTheoKhachHang(x.Id,model.TuNgay,model.DenNgay, out slhuy);
                        modelkh.SoLuongHuy = slhuy;
                        return modelkh;
                    }),
                    Total = items.Count
                };
                return Json(gridModel);
            }
            else
            {
                var itemids = _limousinebanveService.GetHanhKhachTheoSoLuongDatVe(_workContext.NhaXeId,model.ThongTinKhachHang,model.TuNgay,model.DenNgay, model.LoaiTimKiemId);              
                var gridModel = new DataSourceResult
                {
                    Data = itemids.Select(x =>
                    {
                        var modelkh = _limousinebanveService.GetKhachHangById(x.ItemId).toModel();
                        int slhuy;
                        modelkh.SoLuongDat = _limousinebanveService.GetSoLuongDatVeTheoKhachHang(x.ItemId, model.TuNgay, model.DenNgay, out slhuy);
                        modelkh.SoLuongHuy = slhuy;
                        return modelkh;
                    }),
                    Total = itemids.Count
                };
                return Json(gridModel);
            }
            

        }
        public ActionResult UpdateIsTaxi(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var khachhang = _limousinebanveService.GetKhachHangById(Id);
            khachhang.IsTaxi = !khachhang.IsTaxi;
            _limousinebanveService.UpdateKhachHang(khachhang);
            return ThanhCong();
        }
        public ActionResult _KhachHangChinhSua(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var khachhang = _limousinebanveService.GetKhachHangById(Id);
            return PartialView(khachhang.toModel());
        }
        public ActionResult KhachHangChinhSua(KhachHangModel model)
        {

            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var khachhang = _limousinebanveService.GetKhachHangById(model.Id);
            khachhang.Ten = model.Ten;
            khachhang.DienThoai = model.DienThoai;
            khachhang.DiaChi = model.DiaChi;
            _limousinebanveService.UpdateKhachHang(khachhang);            
            return ThanhCong();
        }

        #endregion
         
        #region lich su dat ve
        public ActionResult LichSuDatVe()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new LichSuDatVeModel();
            model.NgayDi = DateTime.Now;
            model.ListHanhTrinh = PrepareHanhTrinhList(false);           

            return View(model);
        }
        [HttpPost]
        public ActionResult LichSuDatVe(DataSourceRequest command, string NgayDi, int HanhTrinhId,string StringSearch)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var _ngaydi = Convert.ToDateTime(NgayDi);
            var items = _limousinebanveService.GetLichSuDatVe(_ngaydi, HanhTrinhId, StringSearch,
                command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
               
                Data = items.Select(c=>
                {
                var _item =new DatVeNoteItem();
                _item.MaHD = c.DatVe.Ma;
                _item.KhachHang = c.DatVe.khachhang.toText();
                _item.NgayTao = c.NgayTao;
                _item.NguoiTao = c.DatVe.nguoitao.TenVaHo;
                _item.Note = c.Note;
                return _item;
                }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }
        public ActionResult _TabLichSuBanVe(int ChuyenDiId)
        {
            var model = _limousinebanveService.GetLichSuBanVeByChuyenDiId(ChuyenDiId);
            return PartialView(model);
        }
        #endregion

        #region Cau hinh gia ve
        public ActionResult CauHinhGiaVeChung()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHanhTrinh))
                return AccessDeniedView();
            //lay danh sach HanhTrinh, LoaiXe, Gia Ve theo cau hinh chung
            var giaves = _limousinebanveService.GetAllLichTrinhLoaiXe(_workContext.NhaXeId);
            int STT = 0;
            var models = giaves.Select(c =>
            {
                STT++;
                var model = new LichTrinhLoaiXeModel();
                model.Id = STT;
                model.HanhTrinhId = c.HanhTrinhId;
                model.tenhanhtrinh = c.hanhtrinh.toText();
                model.LoaiXeId = c.LoaiXeId;
                model.tenloaixe = c.loaixe.TenLoaiXe;
                model.GiaVe = c.GiaVe;
                return model;
            }).ToList();

            return View(models);
        }
        [HttpPost]
        public ActionResult CauHinhGiaVeChung(int HanhTrinhId, int LoaiXeId, string GiaVe)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHanhTrinh))
                return AccessDeniedView();
            //lay danh sach lich trinh roi su dung ham update lich trinh update gia ve
            var _GiaVe = Convert.ToInt32(GiaVe);
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId);
            foreach(var lt in lichtrinhs)
            {
                _limousinebanveService.UpdateGiaVe(HanhTrinhId, lt.Id, LoaiXeId, _GiaVe);
            }
            return Json(GiaVe);
        }
        public ActionResult CauHinhGiaVeSoDo()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHanhTrinh))
                return AccessDeniedView();
            //lay danh sach HanhTrinh, LoaiXe, Gia Ve
            var model = new CauHinhGiaVeSoDoGheModel();
            model.HanhTrinhs = PrepareHanhTrinhList();
            var loaixes = _xeinfoService.GetAllByNhaXeId(_workContext.NhaXeId);
            model.LoaiXes = loaixes.Select(c => new SelectListItem
            {
                Value=c.Id.ToString(),
                Text=c.TenLoaiXe
            }).ToList();
            //lay thong tin 
            return View(model);
        }
        public ActionResult _CauHinhGiaVeSoDo(int HanhTrinhId, int LoaiXeId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHanhTrinh))
                return AccessDeniedView(); 
            var model = new CauHinhGiaVeSoDoGheModel();
            model.HanhTrinhId = HanhTrinhId;
            model.LoaiXeId = LoaiXeId;
            var sodogiaves = _limousinebanveService.GetAllHanhTrinhLoaiXeGiaVe(HanhTrinhId, LoaiXeId);
            if(sodogiaves.Count>0)
            {
                model.SoDoGheGiaVes = sodogiaves.Select(c =>
                {
                    var item =new  CauHinhGiaVeSoDoGheModel.SoDoGheGiaVe();
                    item.SoDoGheId = c.SoDoGheId;
                    var sodogheinfi=_xeinfoService.GetSoDoGheXeQuyTacById(c.SoDoGheId);
                    item.SoGhe = sodogheinfi.Val;
                    item.GiaVe = c.GiaVe;
                    return item;
                }).ToList();
            }
            else
            {
                //lay thong tin sodoghe xe
                var sodoghequytacs = _xeinfoService.GetAllSoDoGheXeQuyTac(LoaiXeId).Where(c=>c.x>0 && c.y>0);
                model.SoDoGheGiaVes = sodoghequytacs.Select(c =>
                {
                    var item = new CauHinhGiaVeSoDoGheModel.SoDoGheGiaVe();
                    item.SoDoGheId = c.Id;
                    item.SoGhe = c.Val;
                    item.GiaVe = 0;
                    return item;
                }).ToList();
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _CauHinhGiaVeSoDo(int HanhTrinhId, int LoaiXeId, string GiaVeSoDo)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHanhTrinh))
                return AccessDeniedView();
            //GiaVe=SoDoGheId1;GiaVe1;SoDoGheId2;GiaVe2
            string[] arrdata = GiaVeSoDo.Split('|');
            for (int i = 0; i < arrdata.Length; i = i + 2 )
            {
                if (string.IsNullOrEmpty(arrdata[i + 1]))
                    continue;
                var SoDoGheId = Convert.ToInt32(arrdata[i]);
                var GiaVe = Convert.ToInt32(arrdata[i+1]);
                _limousinebanveService.UpdateGiaVeSoDo(HanhTrinhId, LoaiXeId, SoDoGheId, GiaVe);
            }
                //cat ra roi dung ham update gia ve theo so do
                
                return ThanhCong();
        }
        #endregion
       
        #region Quan ly tham so

        public ActionResult ListThamSo()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLNhaXe))
                return null;
            return View();
        }

        public JsonpResult _ListThamSo()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLNhaXe))
                return null;
            var items = _limousinebanveService.GetAllThamSo();

            return this.Jsonp(items);
        }

        public JsonpResult CreateThamSo()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<ThamSo>>("models");

            if (models != null)
            {
                foreach (var model in models)
                {


                    _limousinebanveService.InsertThamSo(model);

                }
            }
            return this.Jsonp(models);
        }

        public JsonpResult EditThamSo()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<ThamSo>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _limousinebanveService.GetThamSoById(model.Id);
                    item.GiaTri = model.GiaTri;
                    var kq = _limousinebanveService.UpdateThamSo(item);

                }
            }
            return this.Jsonp(models);
        }
        #endregion
        #region Ban dieu chuyen
        public ActionResult BangDieuChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var model = new DanhSachChuyenDiModel();
            model.IsQuyenTaoChuyen = true;
            model.NgayDi = DateTime.Now;
            model.KhungGioId = (int)CommonHelper.KhungGioHienTai();
            model.khunggios = this.GetCVEnumSelectList<ENKhungGio>(_localizationService, model.KhungGioId);
            //lay tat ca nhan vien
            model.AllLaiXePhuXes = _nhaxeService.GetAllNhanVienByNhaXe(_workContext.NhaXeId, new ENKieuNhanVien[] { ENKieuNhanVien.LaiXe, ENKieuNhanVien.PhuXe }).Select(c =>
            {
                return new XeXuatBenItemModel.NhanVienLaiPhuXe(c.Id, c.ThongTin());
            }).ToList();
            //lay tat ca thong tin xe
            model.AllXeInfo = _xeinfoService.GetAllXeInfoByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                return new XeXuatBenItemModel.XeVanChuyenInfo(c.Id, c.BienSo);
            }).ToList();

            return View(model);
        }
        BangDieuChuyenModel KhoiTaoBangDieuChuyen(DanhSachChuyenDiModel model,bool isThongTinXe=true)
        {
            var modelnew = new BangDieuChuyenModel();
            //build thong tin bang dieu chuyen
            //lay thong tin lich trinh, de lam cot
            if (_workContext.NhaXeId == 0)
            {
                Session["NhaXeId"] = 53;                
            }
                
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByKhungGio(_workContext.NhaXeId, model.khunggio).OrderBy(c => c.ThoiGianDi).ToList();
            //lay thong tin hanh trinh, de lang hang
            var hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId, null);

            var allchuyendi = _limousinebanveService.GetAllChuyenDi(_workContext.NhaXeId, 0, 0, ENKhungGio.All, model.NgayDi).ToList();
            KhungThoiGian khungtg = null;
            if (model.khunggio != ENKhungGio.All)
            {
                khungtg = new KhungThoiGian(model.khunggio);
            }
            //tao bang dieu chuyen
            modelnew.hanhtrinhdieuchuyens = new List<BangDieuChuyenModel.HanhTrinhDieuChuyen>();
            int Id = 0;
            foreach (var ht in hanhtrinhs)
            {
                var htdieuchuyen = new BangDieuChuyenModel.HanhTrinhDieuChuyen(ht.Id, ht.toText());
                foreach (var lt in lichtrinhs)
                {
                    Id++;
                    var item = new BangDieuChuyenModel.BangDieuChuyenItem(Id, ht.Id, ht.MaHanhTrinh, lt.Id, lt.ThoiGianDi.ToString("HH:mm"));
                    //var chuyendis = _limousinebanveService.GetAllChuyenDi(_workContext.NhaXeId, ht.Id, lt.Id, model.khunggio, model.NgayDi).OrderByDescending(c => c.NgayDiThuc).ThenByDescending(c => c.Id).ToList();
                    var chuyendis = allchuyendi.Where(c => c.HanhTrinhId == ht.Id && c.LichTrinhId == lt.Id);
                    if (khungtg != null)
                    {
                        chuyendis = chuyendis.Where(c => c.NgayDiThuc.Hour >= khungtg.GioTu
                                && c.NgayDiThuc.Hour < khungtg.GioDen);
                    }
                    item.chuyendis = chuyendis.Select(c =>
                    {
                        return c.toModel(_localizationService);
                    }).ToList();
                    htdieuchuyen.bangdieuchuyens.Add(item);
                }
                modelnew.hanhtrinhdieuchuyens.Add(htdieuchuyen);
            }
            if (!isThongTinXe)
                return modelnew;
            //thong tin xe dieu va chua dieu
            //lay tat ca thong tin xe
            var AllXeInfo = _xeinfoService.GetAllXeInfoByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                return new XeXuatBenItemModel.XeVanChuyenInfo(c.Id, c.BienSo);
            }).ToList();
            modelnew.ThongTinXeDaDieu = "";
            var allchuyendicoxe = allchuyendi.Where(c => c.XeVanChuyenId > 0).ToList();
            foreach (var cd in allchuyendicoxe)
            {
                if (!modelnew.ThongTinXeDaDieu.Contains(cd.xevanchuyen.BienSo + "("))
                {
                    var arrht = cd.laixe.TenVaHo.Split(' ');
                    modelnew.ThongTinXeDaDieu = modelnew.ThongTinXeDaDieu + string.Format("{0}({1}); ", cd.xevanchuyen.BienSo, arrht[0]);
                }

            }
            modelnew.ThongTinXeChuaDieu = "";
            foreach (var xe in AllXeInfo)
            {
                if (!allchuyendicoxe.Any(c => c.xevanchuyen.BienSo.Contains(xe.BienSo)))
                    modelnew.ThongTinXeChuaDieu = modelnew.ThongTinXeChuaDieu + xe.BienSo + "; ";
            }
            return modelnew;
        }
        [HttpGet]
        public ActionResult _BangDieuChuyen(DanhSachChuyenDiModel model)
        {
            var modelnew = KhoiTaoBangDieuChuyen(model);           
            return PartialView(modelnew);
        }
        public ActionResult BangDieuChuyenTrongNgay()
        {
            var model = new DanhSachChuyenDiModel();
            model.NgayDi = DateTime.Now;
            model.KhungGioId = (int)ENKhungGio.All;
            model.khunggios = this.GetCVEnumSelectList<ENKhungGio>(_localizationService, model.KhungGioId);
            model.IsQuyenTaoChuyen = false;
            return View(model);
        }
        [HttpGet]
        public ActionResult _BangDieuChuyenTrongNgay(DanhSachChuyenDiModel model)
        {
            var modelnew = KhoiTaoBangDieuChuyen(model);
            return PartialView(modelnew);
        }
        #endregion
    }
}