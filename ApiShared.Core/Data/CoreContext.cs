using ApiShared.Core.Data.BaseInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data
{
    public class CoreContext : DbContext
    {
        public CoreContext()
        {

        }

        public CoreContext(DbContextOptions<CoreContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var assembly in StaticParameters.CoreContext.ConfigurationsAssembly)
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly,
                    t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                typeof(EntityModel).IsAssignableFrom(i.GenericTypeArguments[0])));
            }
        }
    }
}
