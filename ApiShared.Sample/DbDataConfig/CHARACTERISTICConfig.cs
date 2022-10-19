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
    public class CHARACTERISTICConfig : IEntityTypeConfiguration<CHARACTERISTIC>
    {
        public void Configure(EntityTypeBuilder<CHARACTERISTIC> builder)
        {
            builder.ToTable("CHARACTERISTIC", "MasterData");
            builder.HasKey(x => x.CHARACTERISTIC_CODE);

        }
    }
}
