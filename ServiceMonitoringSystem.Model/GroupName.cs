using System;

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
        public string ServiceName { set; get; }
        /// <summary>
        /// 服务中文名
        /// </summary>
        public string ServiceNameCN { set; get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDateTime { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { set; get; }
    }
}
