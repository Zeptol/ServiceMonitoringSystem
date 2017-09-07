using MongoDB.Bson;

namespace ServiceMonitoringSystem.Model
{
    public class BasicType
    {
        public int _id { get; set; }
        public int Rid { get; set; }
        public int? TypeId { get; set; }
        public int? Num { get; set; }
        public string Name { get; set; }
    }
}
