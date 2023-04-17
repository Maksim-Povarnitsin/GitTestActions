using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSK.AzimuthPT.JsonFlowReader.Service.Object
{
    public class Connection
    {
        public string connection_id { get; set; }
    }
    public class ConnectionFunc
    {
        public string GetStr(string msg)
        {
            string json_azimuthAPI = msg.Replace("data:", "");
            return json_azimuthAPI;
        }
    }
}
