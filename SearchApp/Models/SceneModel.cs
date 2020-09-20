using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchApp.Models
{
    public class SceneModel
    { 
        public string SceneName { get; set; }
        public string PuzzleName { get; set; }
        public List<WordModel> Words { get; set; }

        public string Mod { get; set; }
    }
}