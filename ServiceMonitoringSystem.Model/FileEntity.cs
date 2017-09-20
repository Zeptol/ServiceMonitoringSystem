using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceMonitoringSystem.Model
{
    public class FileEntity
    {
        public ObjectId _id { get; set; }
        public int SecondaryId { get; set; }
        public int Rid { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public string Author { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }
        public string Md5 { get; set; }
        public string Url { get; set; }
    }
}
