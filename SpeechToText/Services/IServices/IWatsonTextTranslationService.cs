using Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IWatsonTextTranslationService
    {
        Task<TextTranslationResult> TranslateText(string inputText, string inputLanguage, List<string> languages);
    }
}
