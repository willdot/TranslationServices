using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class ClientTranslationInput
    {

        public string Base64String { get; set; }

        public string InputLanguage { get; set; }

        public string[] OutputLanguages { get; set; }

        public string OutputVoice { get; set; }

        public bool ReturnAudioOutput { get; set; }

        public ClientTranslationInput()
        {
            
        }
    }
}
