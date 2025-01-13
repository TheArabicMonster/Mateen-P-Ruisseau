using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau
{
    public class GenericEnvelope
    {
        public string SenderId { get; set; }
        public MessageType MessageType { get; set; }
        public string EnveloppeJson { get; set; }
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }
        public GenericEnvelope(MessageType messageType, string senderID)
        {
            SenderId = senderID;
            MessageType = messageType;
        }
    }
}
