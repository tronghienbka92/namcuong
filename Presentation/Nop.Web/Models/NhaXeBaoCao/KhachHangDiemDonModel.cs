using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System;
using Nop.Core.Domain.Chonves;
using Nop.Core.Domain.NhaXes;

namespace Nop.Web.Models.NhaXeBaoCao
{
   
    public class TongHopDiemDonModel : BaseNopModel
    {
        public TongHopDiemDonModel()
        {

        }
        public int DiemDonId { get; set; }
        public string TenDiemDon { get; set; }
        public int SLKhach { get; set; }
        public decimal DoanhThu { get; set; }
       
    }
    public  class KhachHangDiemDonModel : BaseNopModel
    {
        public KhachHangDiemDonModel()
        {
          
            HanhTrinhs = new List<SelectListItem>();
        }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.TuNgay")]
        public DateTime TuNgay { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.DenNgay")]
        public DateTime DenNgay { get; set; }
         public string SearchName { get; set; }
       
         public List<SelectListItem> HanhTrinhs { get; set; }
         public int HanhTrinhId { get; set; }
         
    }
    public class KhachHangDiemDonItemModel : BaseNopModel
    {
       

        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.TuNgay")]
        public DateTime TuNgay { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.DenNgay")]
        public DateTime DenNgay { get; set; }
        public string SearchName { get; set; }
        public int DiemDonId { get; set; }
        public int HanhTrinhId { get; set; }

    }
   
}