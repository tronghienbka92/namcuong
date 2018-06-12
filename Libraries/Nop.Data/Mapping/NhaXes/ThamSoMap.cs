using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;

namespace Nop.Data.Mapping.NhaXes
{
    public class ThamSoMap : NopEntityTypeConfiguration<ThamSo>
    {
        public ThamSoMap()
        {
            this.ToTable("CV_ThamSo");
            this.HasKey(c => c.Id);
            this.Property(p => p.GiaTri).HasPrecision(18, 3);

            this.Ignore(c => c.LoaiThamSo);





        }
    }
}
