using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau
{
    public class Message
    {
        public int MessageType { get; set; }
        public string SenderId { get; set; }
        public string EnveloppeJson { get; set; }
        public Enveloppe Content { get; set; }
    }
}
