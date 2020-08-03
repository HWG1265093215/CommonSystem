using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntityConfiguration
{
    /// <summary>
    /// 部门表表配置
    /// </summary>
    public class DepartmentConfiguration : BaseConfiguration<DepartmentEntity>
    {
        public override void Configure(EntityTypeBuilder<DepartmentEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Departments");
            builder.Property(x => x.ParentId).HasMaxLength(36);
            builder.Property(x => x.Name).IsRequired().IsUnicode(true).HasMaxLength(50);
            builder.Property(x => x.FullName).IsUnicode(true).HasMaxLength(500);
        }
    }
}
