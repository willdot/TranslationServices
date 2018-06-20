using Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IAzureTextTranslationService
    {
        Task<TextTranslationResult> TranslateText(string inputText, List<string> languages);
    }
}
