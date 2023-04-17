using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSK.AzimuthPT.JsonFlowReader.Service.ObjectsToSettings
{
    public class CoubModuleSettings
    {
        public string ModuleId { get; set; } = String.Empty;
        public string Authority { get; set; } = String.Empty;
        public string CoubUri { get; set; } = String.Empty;
    }

    public class DataServiceSettings
    {
        public string Name { get; set; } = String.Empty;
        public string CustomizationString { get; set; } = String.Empty;
        public string DataServiceTypeString { get; set; } = String.Empty;
    }

    public class Logging
    {
        public bool IncludeScopes { get; set; } = false;
        public string PathFormat { get; set; } = String.Empty;
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }

    public class LogLevel
    {
        public string Default { get; set; } = String.Empty;
        public string System { get; set; } = String.Empty;
        public string Microsoft { get; set; } = String.Empty;
    }

    public class Root
    {
        public ServiceSettings ServiceSettings { get; set; } = new ServiceSettings();
    }

    public class ServiceSettings
    {
        public int TimeToSleep_msec { get; set; } = 600000;
    }

    public class WebAuditor
    {
        public string LoggerTypeString { get; set; }= String.Empty;
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }
}
