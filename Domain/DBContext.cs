using Domain.Entity;
using Domain.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class DBContext:DbContext
    {
        public DBContext(DbContextOptions<DBContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var temp = typeof(SystemConfigConfiguration).Assembly
                .GetTypes().Where(q=>q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName)!=null&&!q.FullName.StartsWith("Domain.EntityConfiguration.BaseConfiguration"));

            foreach (var type in temp)
            {
                dynamic configurationType = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationType);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        #region DbSets

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<RoleEntity> Roles { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public DbSet<MenuEntity> Menus { get; set; }

        /// <summary>
        /// 用户角色关系
        /// </summary>
        public DbSet<UserRoleEntity> UserRoles { get; set; }

        /// <summary>
        /// 角色菜单关系
        /// </summary>
        public DbSet<RoleMenuEntity> RoleMenus { get; set; }

        /// <summary>
        /// 路径码
        /// </summary>
        public DbSet<PathCodeEntity> PathCodes { get; set; }

        /// <summary>
        /// 页面访问记录
        /// </summary>
        public DbSet<PageViewEntity> PageViews { get; set; }

        /// <summary>
        /// 登录日志
        /// </summary>
        public DbSet<LoginLogEntity> LoginLogs { get; set; }

        /// <summary>
        /// 系统配置
        /// </summary>
        public DbSet<SystemConfigEntity> SystemConfigs { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public DbSet<AreaEntity> Areas { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public DbSet<DepartmentEntity> Departments { get; set; }

        /// <summary>
        /// 访问数据统计
        /// </summary>
        public DbSet<SiteViewEntity> SiteViews { get; set; }

        /// <summary>
        /// 站内信
        /// </summary>
        public DbSet<MessageEntity> Messages { get; set; }

        /// <summary>
        /// 站内信接收人
        /// </summary>
        public DbSet<MessageReceiverEntity> MessageReceivers { get; set; }

        /// <summary>
        /// 任务流模板
        /// </summary>
        public DbSet<TaskTemplateEntity> TaskTemplates { get; set; }

        /// <summary>
        /// 任务流表单
        /// </summary>
        public DbSet<TaskTemplateFormEntity> TaskTemplateForms { get; set; }

        /// <summary>
        /// 任务流步骤
        /// </summary>
        public DbSet<TaskTemplateStepEntity> TaskTemplateSteps { get; set; }

        /// <summary>
        /// 任务流操作
        /// </summary>
        public DbSet<TaskTemplateStepOperateEntity> TaskTemplateStepOperates { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public DbSet<ModelTempEntity> ModelTemp { get; set; }

        /// <summary>
        /// 动态读取存储过程
        /// </summary>
        public DbSet<RPG_Proc_ReportEntity> RPG_Proc_Report { get; set; }
        #endregion
    }
}
