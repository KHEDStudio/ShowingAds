using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShowingAds.AndroidApp.Core
{
    public static class Settings
    {
        public static Guid DeviceId = Guid.Empty;
        public static readonly int MaxReadyVideos = 100;
        public static readonly Uri ServerPath = new Uri("http://84.38.188.128:3667/");
        public static readonly Uri LoginPath = new Uri(ServerPath, "session/");
        public static readonly Uri DeviceControllerPath = new Uri(ServerPath, "device/");
        public static readonly Uri ChannelPath = new Uri(DeviceControllerPath, "channel/");
        public static readonly Uri FilesServicePath = new Uri("http://31.184.219.123:3666/");
        public static readonly Uri NotifyPath = new Uri("http://31.184.219.123:3669/notify/");
#if DEBUG
        public static readonly Uri LogServer = new Uri("https://9a7e87ad-8d72-45f8-9a47-3b02aa9b2773.mock.pstmn.io/");
#else
        public static readonly Uri LogServer = new Uri("http://31.184.219.123:3670/");
#endif

        public static Uri GetVideoDownloadUri(Guid videoId) => GetVideoDownloadUri(videoId.ToString());
        public static Uri GetVideoDownloadUri(string videoId) => new Uri(FilesServicePath, $"video?video={videoId}");

        public static Uri GetLogoDownloadUri(Guid logoId) => GetLogoDownloadUri(logoId.ToString());
        public static Uri GetLogoDownloadUri(string logoId) => new Uri(FilesServicePath, $"logo?logo={logoId}");

        public static string GetConfigFilePath(string fileName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileName);

        public static string GetLogotypeFilesPath(Guid fileName) => GetLogotypeFilesPath(fileName.ToString());
        public static string GetLogotypeFilesPath(string fileName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileName);

        public static string GetVideoFilesPath(Guid fileName) => GetVideoFilesPath(fileName.ToString());
        public static string GetVideoFilesPath(string fileName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileName);
    }
}
