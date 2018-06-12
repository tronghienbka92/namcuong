using System;
using System.Collections.Generic;


namespace Nop.Core.Domain.NhaXes
{
    public class ThamSo : BaseEntity
    {
        public string TenThamSo { get; set; }
       
        public int LoaiThamSoId { get; set; }
        public ENLoaiChiPhi LoaiThamSo
        {
            get
            {
                return (ENLoaiChiPhi)LoaiThamSoId;
            }
            set
            {
                LoaiThamSoId = (int)value;
            }
        }
        public decimal GiaTri { get; set; }  
     
        
    }
}
