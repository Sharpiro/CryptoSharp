using System.Reflection;

namespace CryptoSharp.Wpf.ViewModels
{
    public class PageOwnerViewModel
    {
        public string Title => $"CryptoSharp-{Assembly.GetExecutingAssembly().GetName().Version}";
    }
}