using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShutAndAdmin
    {
        /// <summary>
        /// Spreadsheet name
        /// </summary>
        [JsonProperty]
        private string type;
        

        public ShutAndAdmin()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStringType()
        {
            return type;
        }

        public void SetType(string t)
        {
            type = t;
        }
    }
}

