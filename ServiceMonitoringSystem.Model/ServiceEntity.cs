using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceMonitoringSystem.Common.Attributes;
using ServiceMonitoringSystem.Common.Enums;

namespace ServiceMonitoringSystem.Model
{
    public class ServiceEntity
    {
        public int _id { get; set; }

        public string Dependency { get; set; }
        [Required,Display(Name = "部署机器"),Excel]
        public string Host { get; set; }
        [Required,Display(Name = "服务名称"),Excel]
        public string ServiceName { get; set; }

        public int PrimaryId { get; set; }

        public int SecondaryId { get; set; }
        [Required,Display(Name = "二级服务名称"),Excel]
        public string SecondaryName { get; set; }

        public string Metrics { get; set; }

        public string RegContent { get; set; }

        public ServiceTypeEnum ServiceType { get; set; }
         [Display(Name = "备注"),Excel]
        public string Remark { get; set; }
        [Required,Display(Name = "版本号"),Excel]
        public string Version { get; set; }

        public bool IsAlert { get; set; }

        public bool IsStorage { get; set; }

        public int ThresholdSec { get; set; }

        public bool IsApproved { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartupDate { get; set; }
    }
}
