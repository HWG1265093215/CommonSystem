using Domain.Entity;
using Domain.EntityConfiguration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Domain.EntityConfiguration
{
    /// <summary>
    /// 访问量表配置
    /// </summary>
    public class SiteViewConfiguration : BaseConfiguration<SiteViewEntity>
    {
        public override void Configure(EntityTypeBuilder<SiteViewEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("SiteViews");
            builder.Property(x => x.Number).IsRequired();
            builder.Property(x => x.Day).IsRequired();
        }
    }
}
