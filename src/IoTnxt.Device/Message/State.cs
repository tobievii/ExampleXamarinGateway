using System;
using System.Collections.Generic;
using System.Text;
using IoTnxt.Device.Abstraction;

namespace IoTnxt.Device.Message
{
    public class StateRequest : ICommand
    {
        public string commandText { get; set; }
        public Guid messageId { get; } = Guid.NewGuid();
        public string messageSourceId { get; set; }
        public DateTime postUtc { get; set; } = DateTime.UtcNow.AddMinutes(1);
        public DateTime fromUtc { get; set; } = DateTime.UtcNow.AddMinutes(-10);
        public Guid sourceMessageID { get; } = Guid.NewGuid();
        public Dictionary<string, string> headers { get; set; } = new Dictionary<string, string>()
        {
            {"FileName", "" },
            {"Version", "2.15.0" },
            {"Raptor", "000000000000" }
        };
        public List<QueuePacketParameters> parameters { get; set; } = new List<QueuePacketParameters>();
        public Dictionary<string, Dictionary<string,object>> deviceGroups { get; set; } =  new Dictionary<string, Dictionary<string, object>>();
        

    }

    public class QueuePacketParameters
    {
        public string tag { get; set; }
        public object deviceGroups { get; set; }
    }

    //export interface IQueuePacketHeader
    //{
    //    FileName: string
    //    Raptor: string
    //    Version: string
    //}

    //export interface QueuePacketParameters
    //{
    //    DeviceGroups: object,
    //    Tag: string
    //}
}


