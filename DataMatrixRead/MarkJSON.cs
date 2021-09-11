using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMatrixRead
{
    [JsonObject]
    [Serializable]
    class MarkJSON
    {
        public string reader_device_id { get; set; }
        public List<Marks> mark { get; set; }

        public MarkJSON(string reader_device_id, List<Marks> mark)
        {
            this.reader_device_id = reader_device_id;
            this.mark = mark;
        }
    }
}
