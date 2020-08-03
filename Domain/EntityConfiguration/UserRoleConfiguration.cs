
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

//using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntityConfiguration
{
    /// <summary>
    /// UserRole表信息配置
    /// </summary>
    public class UserRoleConfiguration : BaseConfiguration<UserRoleEntity>
    {
        public override void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("UserRoles");
            builder.Property(e => e.RoleId).HasMaxLength(36).IsRequired();
            builder.Property(e => e.UserId).HasMaxLength(36).IsRequired();
        }
    }
}
