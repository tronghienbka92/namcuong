using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;

namespace Nop.Data.Mapping.NhaXes
{
    public class ChuyenDiThuChiMap : NopEntityTypeConfiguration<ChuyenDiThuChi>
    {
        public ChuyenDiThuChiMap()
        {
            this.ToTable("CV_ChuyenDiThuChi");
            this.HasKey(c => c.Id);
          this.HasRequired(c=>c.ChuyenDi)
               .WithMany()
             .HasForeignKey(c => c.ChuyenId);





        }
    }
}
