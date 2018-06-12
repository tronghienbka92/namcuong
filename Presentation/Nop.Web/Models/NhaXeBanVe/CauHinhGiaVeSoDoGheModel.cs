using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System;
using Nop.Core.Domain.Chonves;
using Nop.Core.Domain.NhaXes;

namespace Nop.Web.Models.NhaXeBanVe
{
    //[Validator(typeof(LoginValidator))]
    public partial class CauHinhGiaVeSoDoGheModel : BaseNopModel
    {
        public CauHinhGiaVeSoDoGheModel()
        {
            HanhTrinhs = new List<SelectListItem>();
            LoaiXes = new List<SelectListItem>();
            SoDoGheGiaVes = new List<SoDoGheGiaVe>();
        }
        public List<SelectListItem> HanhTrinhs { get; set; }
        public List<SelectListItem> LoaiXes { get; set; }
        public List<SoDoGheGiaVe> SoDoGheGiaVes { get; set; } 
        public int HanhTrinhId { get; set; }
        public int LoaiXeId { get; set; }
        public partial class SoDoGheGiaVe 
        {
            public int SoDoGheId { get; set; }
            public string SoGhe { get; set; }
            public decimal GiaVe { get; set; }
            
        }
    }
  
}