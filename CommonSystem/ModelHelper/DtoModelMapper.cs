using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.EntityDto.MessageDto;
using ApplicationLayer.EntityDto.TaskDto;
using AutoMapper;
using Domain.Entity;
using Domain.Meun;
using Hangfire.States;

namespace CommonSystem.ModelHelper
{
    public static class DtoModelMapper
    {
        public static TResult ExtendMap<TResult, TObject>(this IMapper mapper,TObject obj)
        {
            //映射赋值
            TResult result = mapper.Map<TResult>(obj);
            string Column = string.Empty;
            //获取对应特性名称
            foreach (PropertyInfo property in typeof(TResult).GetProperties())
            {
                //若在属性上定义了特性，则根据特性进行赋值
                if (property.IsDefined(typeof(FieldAttribute),false))
                {
                    //获取标记属性值
                    Column = property.GetCustomAttribute<FieldAttribute>().FieldName;
                    //获取标记属性值是否有在映射源中
                    if (obj.GetType().GetProperties().First(n=>n.Name==Column)!=null)
                    {
                        //获取映射源中的对应属性名及属性值
                        dynamic temp = obj.GetType().GetProperty(Column).GetValue(obj,null);
                        property.SetValue(result,temp);
                    }
                }
                //当有忽略属性值的时候则进行赋初始值
                if (property.IsDefined(typeof(IgnoreAttribute),false))
                {
                    property.SetValue(result,property.PropertyType.GetDefaultTypeValue());
                }
            }

            return result;
        }

        public static object GetDefaultTypeValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

    /// <summary>
    /// 模块初始化
    /// </summary>
    public class JuCheapModuleInitializer : ModuleInitializer
    {
        /// <summary>
        /// 加载AutoMapper配置(AutoMapper映射关系配置)
        /// </summary>
        /// <param name="config"></param>
        public override void LoadAutoMapper(IMapperConfigurationExpression config)
        {
            config.CreateMap<UserEntity, UserDto>().ReverseMap();
            config.CreateMap<UserEntity, UserAddDto>().ReverseMap();
            config.CreateMap<UserEntity, UserUpdateDto>().ReverseMap();
            config.CreateMap<UserDto, UserUpdateDto>().ReverseMap();
            config.CreateMap<UserRoleEntity, UserRoleDto>().ReverseMap();
            config.CreateMap<RoleEntity, RoleDto>().ReverseMap();
            config.CreateMap<RoleMenuDto, RoleMenuEntity>().ReverseMap();
            config.CreateMap<PageViewEntity, VisitDto>()
                .ForMember(v => v.VisitDate, e => e.MapFrom(pv => pv.CreateTime))
                .ReverseMap();
            config.CreateMap<LoginLogEntity, LoginLogDto>().ReverseMap();
            config.CreateMap<MenuEntity, MenuDto>()
                .ForMember(m => m.Type, e => e.MapFrom(item => (MenuType)item.Type))
                .ReverseMap();
            config.CreateMap<MenuEntity, TreeDto>()
                .ForMember(m => m.id, e => e.MapFrom(item => item.Id))
                .ForMember(m => m.pId, e => e.MapFrom(item => item.ParentId))
                .ForMember(m => m.name, e => e.MapFrom(item => item.Name));
            config.CreateMap<RoleEntity, TreeDto>()
                .ForMember(m => m.id, e => e.MapFrom(item => item.Id))
                .ForMember(m => m.name, e => e.MapFrom(item => item.Name));

            config.CreateMap<DepartmentEntity, DepartmentDto>().ReverseMap();
            config.CreateMap<AreaEntity, AreaDto>().ReverseMap();
            config.CreateMap<MessageEntity, MessageDto>().ReverseMap();
            config.CreateMap<MessageEntity, MessageQueryDto>();
            config.CreateMap<TaskTemplateFormEntity, TaskTemplateFormDto>().ReverseMap();
            config.CreateMap<TaskTemplateStepEntity, TaskTemplateStepDto>().ReverseMap();
            config.CreateMap<TaskTemplateStepOperateEntity, TaskTemplateStepOperateDto>().ReverseMap();
            config.CreateMap<TaskTemplateEntity, TaskTemplateDto>().ReverseMap();
        }
    }

    public class AutoMapperConfig
    {
        private static MapperConfiguration _mapperConfiguration;

        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            var moduleInitializers = new ModuleInitializer[]
            {
                new JuCheapModuleInitializer()
            };

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var m in moduleInitializers)
                {
                    m.LoadAutoMapper(cfg);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration GetMapperConfiguration()
        {
            if (_mapperConfiguration == null)
                Register();

            return _mapperConfiguration;
        }
    }

    public abstract class ModuleInitializer
    {
        /// <summary>
        /// 加载AutoMapper配置
        /// </summary>
        /// <param name="cofig"></param>
        public abstract void LoadAutoMapper(IMapperConfigurationExpression cofig);
    }
}
