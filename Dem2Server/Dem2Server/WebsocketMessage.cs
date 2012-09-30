using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dem2Server
{
    class WebsocketMessage
    {
        public string msgType { get; set; }
        public string body { get; set; }

        public void Send(IWebSocketConnection connectionToRecipient)
        {
            connectionToRecipient.Send(JsonConvert.SerializeObject(this, new IsoDateTimeConverter()));
        }
    }
}
