using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchApp.Models
{
    public class PixelSection
    {
        public string SectionName { get; set; }
        public List<PixelModel> Pixels { get; set; }
        public List<PixelModel> Words { get; set; }
        public PlanModel Plan { get; set; }
        public int Modulus { get; set; }
    }
}