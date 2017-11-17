namespace ServiceMonitoringSystem.Model
{
    public class tblGPS
    {
        public string _id { get; set; }
        public string VehicleId { get; set; }
        public string VehicleNo { get; set; }
        public string LineId { get; set; }
        public string LineName { get; set; }

        public string GPSDateTime { get; set; }

        public string SendDateTime { get; set; }
        public float Lon { get; set; }
        public float Lat { get; set; }
        public float Lon02 { get; set; }
        public float Lat02 { get; set; }
        public int ToDir { get; set; }
        public int Speed { get; set; }
        public int Height { get; set; }
        public int Angle { get; set; }
        public int PrevLevel { get; set; }
        public int NextLevel { get; set; }
        public int NextStationMeter { get; set; }
        public int RunStatus { get; set; }
        public int Status { get; set; }
        public int Overspeed { get; set; }
        public int FlameoutFlag { get; set; }
        public int CacheFlag { get; set; }
    }
}
