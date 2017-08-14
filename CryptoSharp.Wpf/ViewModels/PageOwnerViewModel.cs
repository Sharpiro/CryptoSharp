using System.Diagnostics;
using System.Reflection;

namespace CryptoSharp.Wpf.ViewModels
{
    public class PageOwnerViewModel
    {
        public string Title => $"CryptoSharp-{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}";
    }
}