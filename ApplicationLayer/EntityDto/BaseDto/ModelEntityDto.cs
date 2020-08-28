using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationLayer.EntityDto.BaseDto
{
    public class ModelEntityDto
    {
        [Display(Name = "序号")]
        public int txt_Number { get; set; }

        [Display(Name = "列名")]
        public string columnName { get; set; }

        [Display(Name = "是否下拉框")]
        public string isCombobox { get; set; }

        [Display(Name = "显示名称")]
        public string displayShowName { get; set; }

        [Display(Name = "是否禁用")]
        public string isDisable { get; set; }

        [Display(Name = "方法名称")]
        public string meunType { get; set; }

        [Display(Name = "菜单类型")]
        public string meunTable { get; set; }

        [Display(Name ="默认值")]
        public string defaultValue { get; set; }

        [Display(Name ="是否必填")]
        public string isRequest { get; set; }

        [Display(Name ="跨行")]
        public int columnSpan { get; set; }
    }
}
