using ApiShared.Sample.DbData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Sample.DbDataConfig
{
    public class CHARACTERISTIC_DETAILConfig : IEntityTypeConfiguration<CHARACTERISTIC_DETAIL>
    {
        public void Configure(EntityTypeBuilder<CHARACTERISTIC_DETAIL> builder)
        {
            builder.ToTable("CHARACTERISTIC_DETAIL", "MasterData");
            builder.HasKey(x => x.CHARACTERISTIC_DETAIL_CODE);
            
            builder.HasOne(x=>x.CHARACTERISTIC_CODENavigation)
                .WithMany(x => x.CHARACTERISTIC_DETAIL)
                .HasForeignKey(x => x.CHARACTERISTIC_CODE);
        }
    }
}
