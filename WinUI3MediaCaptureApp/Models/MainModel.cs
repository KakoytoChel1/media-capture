using System;

namespace WinUI3MediaCaptureApp.Models
{
    internal static class MainModel
    {
        internal static string CreateVideoFileName()
        {
            DateTime dateTime = DateTime.Now;
            string result = $"Video-{dateTime.ToString("dd.MM.yyyy-HH.mm.ss")}";
            return result;
        }

        internal static string CreatePhotoFileName()
        {
            DateTime dateTime = DateTime.Now;
            string result = $"Photo-{dateTime.ToString("dd.MM.yyyy-HH.mm.ss")}";
            return result;
        }
    }
}
