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
    public class SampleTableConfig : IEntityTypeConfiguration<SampleTable>
    {
        public void Configure(EntityTypeBuilder<SampleTable> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Path).HasMaxLength(50);
        }
    }
}
