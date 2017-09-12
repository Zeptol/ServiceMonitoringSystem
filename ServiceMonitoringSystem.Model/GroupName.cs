using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceMonitoringSystem.Model
{
    /// <summary>
    /// 服务分组
    /// </summary>
    public class GroupName
    {
        public int Rid { get; set; }
        public int _id { set; get; }
        /// <summary>
        /// 服务名称
        /// </summary>
        [Required]
        [Display(Name = "服务名称")]
        public string ServiceName { set; get; }
        /// <summary>
        /// 服务中文名
        /// </summary>
        [Required]
        [Display(Name = "服务中文名")]
        public string ServiceNameCN { set; get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDateTime { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
      [Display(Name = "备注")]
        public string Remarks { set; get; }
    }
}
