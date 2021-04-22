using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ruminoid.Common2.Utils.Net
{
    public static class NetUtils
    {
        // https://github.com/AvaloniaUtils/MessageBox.Avalonia/blob/master/src/MessageBox.Avalonia/Controls/Hyperlink.cs
        public static void OpenExternalLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                new Process { StartInfo = { UseShellExecute = true, FileName = url } }.Start(); // https://stackoverflow.com/a/2796367/241446
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("x-www-browser", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Process.Start("open", url);
        }
    }
}
