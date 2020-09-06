using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.Filters;
using ApplicationLayer.IService;
using AutoMapper;
using CommonSystem.Filter;
using CommonSystem.ModelHelper;
using Dapper;
using Domain;
using Infrastructrue;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace CommonSystem.Controllers
{
    [IgnoreActionFilterAttribute]
    public class DemoController : Controller
    {
        public IModelTempService _tempService { get; set; }

        public DBContext _dBContext { get; set; }

        public IWebHostEnvironment _hostEnvironment{ get; set; }

        public IWebHostEnvironment _hosting { get; set; }

        public IMapper _mapper { get; set; }

        [Meun(Id =Menu.DemoPageId,ParentId =Menu.SystemId,Order ="10",Name ="模板配置页面")]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetListWithPager(BaseFilter baseFilter)
        {
            var ListAll = await _tempService.SearchAsync(baseFilter);
            return Json(ListAll);
        }
        [Meun(Id = Menu.DemoPageGetTableId, ParentId = Menu.DemoPageId, Order = "1", Name = "生成模板xmlDataTable")]
        public IActionResult GetSqlTableDetails(string TableName,string ProcName)
        {
            DynamicParameters dy = new DynamicParameters();
            DataTable TableDetails = new DataTable();
            List<ModelEntityDto> list = new List<ModelEntityDto>();
            if (TableName!="RPG_Proc_Report")
            {
                dy.Add("TableName", TableName);
                TableDetails = DapperHelper.ExcuteProduce("TableDetails", dy);
               
            }
            else
            {
                dy.Add("TableName", ProcName);
                TableDetails = DapperHelper.ExcuteProduce("GetProcDetails", dy);

                ModelEntityDto modelEntity = new ModelEntityDto();
                //序号
                modelEntity.txt_Number = 1;
                //列名
                modelEntity.columnName = ProcName;
                //默认值
                modelEntity.defaultValue =ProcName;
                //显示名称
                modelEntity.displayShowName = "存储过程名称";
                //是否下拉框
                modelEntity.isCombobox = "N";
                //是否禁用
                modelEntity.isDisable = "N";
                //是否必填
                modelEntity.isRequest = "N";
                //跨行
                modelEntity.columnSpan = 1;
                //隐藏显示
                modelEntity.isShow = "Y";
                list.Add(modelEntity);
            }

            foreach (DataRow item in TableDetails.Rows)
            {
                ModelEntityDto modelEntity = new ModelEntityDto();
                //序号
                modelEntity.txt_Number =(list.Count+1);
                //列名
                modelEntity.columnName = item[1].ToString();
                //默认值
                modelEntity.defaultValue = (string)item[2];
                //显示名称
                modelEntity.displayShowName = (string)item[3];
                //是否下拉框
                modelEntity.isCombobox = "N";
                //是否禁用
                modelEntity.isDisable = "N";
                //是否必填
                modelEntity.isRequest = "N";
                //跨行
                modelEntity.columnSpan = 1;
                //隐藏显示
                modelEntity.isShow = "Y";
                list.Add(modelEntity);
            }
            return Json(list);
        }

        public IActionResult Edit(string Query)
        {
            ModelEntityDto entityDto = JsonConvert.DeserializeObject<ModelEntityDto>(Query);
            return View(entityDto);
        }
        [Meun(Id = Menu.DemoPageDeleteId, ParentId = Menu.DemoPageId, Order = "3", Name = "删除模板")]
        public async  Task<IActionResult> Delete([FromBody]List<string> ids)
        {
            var result = new JsonResult<bool>();
            if (ids.AnyOne())
            {
                result.flag =await _tempService.DeleteAsync(ids);
            }
            return Json(result);
        }

        [Meun(Id = Menu.DemoPageSaveXml, ParentId = Menu.DemoPageId, Order = "2", Name = "生成模板xmlDataTable")]
        public IActionResult EditModel(string TempName,string TempTable,string TempType,string ModelOrgId,string JsonData,string ModelIndex)
        {
            string urlContentPath = @"\Content\" + ModelOrgId + @"\" + TempName + ".xml";
            var modeltemp = new ModelTempDto()
            {
                TempName = TempName,
                TempTable = TempTable,
                ModelOrgId = ModelOrgId,
                TempType = TempType,
                Id = ModelIndex,
                ContentPath=urlContentPath 
            };
            string urlPath = _hostEnvironment.WebRootPath;
            urlContentPath = urlPath + urlContentPath;
            List<ModelEntityDto> listDto = JsonConvert.DeserializeObject<List<ModelEntityDto>>(JsonData);
            OtherHelper.CreateListXml<ModelEntityDto>(urlContentPath,listDto);




            _tempService.Add(modeltemp);


            List<ModelEntityDto> list = OtherHelper.SerialierList<ModelEntityDto>(urlContentPath); 

            return Json(new JsonResult<string>
            {
                msg="模板保存成功！",flag=true
            });
        }

        [ResponseCache(Duration=500,Location =ResponseCacheLocation.Client)]
        public  IActionResult GetEfTable()
        {
            List<string> list = new List<string>();
            foreach (PropertyInfo p in _dBContext.GetType().GetProperties())
            {
                if (p.PropertyType.Name.Contains("DbSet"))
                {
                    Console.WriteLine(p.Name);
                    list.Add(p.Name);
                }
            }
            return Json(list);
        }
        [Meun(Id = Menu.DemoPageGetXmlData, ParentId = Menu.DemoPageId, Order = "5", Name = "获取xml文件数据并序列化")]
        public async  Task<IActionResult> GetXmlData(string id)
        {
            var  temp =await _tempService.Find(id);

            string urlContendPath =_hostEnvironment.WebRootPath+temp.ContentPath;

            List<ModelEntityDto> list = OtherHelper.SerialierList<ModelEntityDto>(urlContendPath);

            return Json(list);
        }

        public IActionResult GetProcDetails(string ProcName)
        {
            DynamicParameters dy = new DynamicParameters();
            dy.Add("ProcName", ProcName);
            DataTable TableDetails = DapperHelper.ExcuteProduce("GetProcDetails", dy);
            return Ok();
        }
    }
}
