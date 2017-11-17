using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceMonitoringSystem.Model
{
    public class tblVehRoute
    {
        public string _id { get; set; }
        public string IPAddress_F { get; set; }
        public string IPAddress_P { get; set; }
        public string IPAddress_S { get; set; }
        public string IPAddress_T { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LatestTime { get; set; }
    }
}
