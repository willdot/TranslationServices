using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class WatsonTranslationBody
    {
        public string text { get; set; }

        public string source { get; set; }

        public string target { get; set; }

    }
}
