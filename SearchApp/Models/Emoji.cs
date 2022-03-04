using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchApp.Models
{
    [Serializable]
    public class Emoji
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public List<int> Positions { get; set; }

        public List<string> Pixels { get; set; }
    }
}