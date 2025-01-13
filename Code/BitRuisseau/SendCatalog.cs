using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitRuisseau
{
    public class SendCatalog : IMessage
    {
        /*
            type 1
        */
        private List<MediaData> _content;

        public List<MediaData>? Content
        {
            get => _content;
            set => _content = value;
        }


        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
