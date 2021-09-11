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
   public class Marks
    {
        public string code { get; set; }
        public string decode { get; set; }

        public Marks(string code, string decode)
        {
            this.code = code;
            this.decode = decode;
        }
    }
}
