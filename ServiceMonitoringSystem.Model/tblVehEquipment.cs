using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceMonitoringSystem.Model
{
    public class tblVehEquipment
    {
        public string _id { get; set; }
        public string EquipmentNo { get; set; }
        public string ICCID { get; set; }
        public string LineId { get; set; }
        public string LineName { get; set; }
        public string Manufacturer { get; set; }
        public string SoftVersion { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
        public string VehicleId { get; set; }
        public string VehicleNo { get; set; }
    }
}
