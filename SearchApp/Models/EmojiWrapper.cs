using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchApp.Models
{
    [Serializable]
    public class EmojiWrapper
    {
        List<Emoji> emojis { get; set; }
    }
}