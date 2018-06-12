using System;
using System.Collections.Generic;


namespace Nop.Core.Domain.NhaXes
{
    public class ChuyenDiThuChi : BaseEntity
    {
        public int ChuyenId { get; set; }

        public int LoaiTaiChinhThuChiId { get; set; }
        public virtual ChuyenDi ChuyenDi { get; set; }

        public decimal SoTien { get; set; }        
    }
}
