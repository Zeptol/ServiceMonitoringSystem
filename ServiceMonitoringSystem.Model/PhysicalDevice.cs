using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceMonitoringSystem.Model
{
    public class PhysicalDevice
    {
        public ObjectId _id { get; set; }
        public int Rid { get; set; }
         [Required, Display(Name = "型号")]
        public string ModelNum { get; set; }
         [Required, Display(Name = "业主方")]
        public int? Owner { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        [Display(Name = "上架日期")]
        public DateTime? Date { get; set; }
         [Required, Display(Name = "公网IP")]
         [RegularExpression("^((2[0-4]\\d|25[0-5]|[1-9]?\\d|1\\d{2})\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$", ErrorMessage = "无效的IP地址")]
        public string PublicIP { get; set; }
         [Required, Display(Name = "内网IP")]
         [RegularExpression("^((2[0-4]\\d|25[0-5]|[1-9]?\\d|1\\d{2})\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$", ErrorMessage = "无效的IP地址")]
        public string IntranetIP { get; set; }
         [Display(Name = "备注")]
        public string Remark { get; set; }
         [Required, Display(Name = "设备类型")]
        public int? DeviceType { get; set; }
        [Display(Name = "CPU")]
        public string Cpu { get; set; }
        [Display(Name = "内存")]
        public string Memory { get; set; }
        [Display(Name = "存储")]
        public string Storage { get; set; }
          [Display(Name = "所在位置")]
        public string Locale { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Display(Name = "过保日期")]
        public DateTime? WarrantyExpiry { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Display(Name = "购买日期")]
        public DateTime? PurchaseDate { get; set; }
        [RegularExpression("^((2[0-4]\\d|25[0-5]|[1-9]?\\d|1\\d{2})\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$", ErrorMessage = "无效的IP地址")]
        [Display(Name = "管理IP")]
        public string ManagementIP { get; set; }
         [Display(Name = "网络拓扑参数")]
        public string NetTpParam { get; set; }
        [Required,Display(Name = "机器名称")]
        public string MachineName { get; set; }
        [Display(Name = "域IP")]
        [RegularExpression("^((2[0-4]\\d|25[0-5]|[1-9]?\\d|1\\d{2})\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$", ErrorMessage = "无效的IP地址")]
        public string DomainIP { get; set; }
    }
}
