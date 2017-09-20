using System.Collections.Generic;

namespace ServiceMonitoringSystem.Model
{
    public class ServiceConf
    {
        public List<string> OutAddr { get; set; }

        public List<string> InAddr { get; set; }

        public List<string> Depends { get; set; }

        public string Remarks { get; set; }
    }
}
