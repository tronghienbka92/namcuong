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
    public partial class LichTrinhLoaiXeModel : BaseNopModel
    {
        public LichTrinhLoaiXeModel()
        {
           
           
        }
        public int Id { get; set; }
        [NopResourceDisplayName("ChonVe.NhaXe.HanhTrinh.HanhTrinhId")]
        public int HanhTrinhId { get; set; }
        public string tenhanhtrinh { get; set; }
        public int LoaiXeId { get; set; }
        public string tenloaixe { get; set; }
        public decimal GiaVe { get; set; }   
    }
  
}