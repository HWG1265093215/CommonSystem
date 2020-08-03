
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntityConfiguration
{
    /// <summary>
    /// 任务流模板表信息配置
    /// </summary>
    public class TaskTemplateConfiguration : BaseConfiguration<TaskTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<TaskTemplateEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("TaskTemplate");
            builder.Property(x => x.Name).IsRequired().IsUnicode(true).HasMaxLength(100);
        }
    }
}
