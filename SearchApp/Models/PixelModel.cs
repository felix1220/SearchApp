using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchApp.Models
{
    public class PixelModel
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int BackgroundRed { get; set; }
        public int BackgroundBlue { get; set; }
        public int BackgroundGreen { get; set; }
        public int ID { get; set; }
        public bool IsVisible { get; set; }
        public bool Outline { get; set; }
        public PositionModel Position { get; set; }
        public string Letter { get; set; }
        public int? Direction { get; set; }
        public int? Index { get; set; }
    }
}