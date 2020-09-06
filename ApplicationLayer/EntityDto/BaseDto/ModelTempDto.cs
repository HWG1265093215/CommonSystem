using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationLayer.EntityDto.BaseDto
{
    public class ModelTempDto
    {
        public string Id { get; set; }

        [Display(Name ="模块名称"),Required(ErrorMessage =Message.Required)]
        public string TempName { get; set; }

        public string ContentPath { get; set; }

        public string TempTable { get; set; }

        public string TempType { get; set; }

        public string ModelOrgId { get; set; }
    }
}
