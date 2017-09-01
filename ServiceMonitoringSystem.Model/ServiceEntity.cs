using System;
using MongoDB.Bson.Serialization.Attributes;
using ServiceMonitoringSystem.Common.Enums;

namespace ServiceMonitoringSystem.Model
{
    public class ServiceEntity
    {
        public object _id { get; set; }

        public string Dependency { get; set; }

        public string Host { get; set; }

        public string ServiceName { get; set; }

        public int PrimaryId { get; set; }

        public int SecondaryId { get; set; }

        public string SecondaryName { get; set; }

        public string Metrics { get; set; }

        public string RegContent { get; set; }

        public ServiceTypeEnum ServiceType { get; set; }

        public string Remark { get; set; }

        public string Version { get; set; }

        public bool IsAlert { get; set; }

        public bool IsStorage { get; set; }

        public int ThresholdSec { get; set; }

        public bool IsApproved { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartupDate { get; set; }
    }
}
