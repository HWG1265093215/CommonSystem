using System.ComponentModel.DataAnnotations;
using ApplicationLayer.EntityDto.BaseDto;
using Domain.Meun;
using Infrastructrue;

namespace ApplicationLayer.EntityDto.Task
{
    public class TaskTemplateFormDto
    {
        public string Id { get; set; }
        /// <summary>
        /// 所属模板Id
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
        [Display(Name = "控件类型")]
        [Required(ErrorMessage = Message.Required)]
        public ControlType ControlType { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        [Display(Name = "控件名称")]
        [Required(ErrorMessage = Message.Required)]
        [MaxLength(50, ErrorMessage = Message.MaxLength)]
        public string ControlName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        public string ControlTypeName => ControlType.GetDescriptionForEnumName();
    }
}
