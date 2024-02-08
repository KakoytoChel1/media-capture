using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using WinUI3MediaCaptureApp.Models;

namespace WinUI3MediaCaptureApp.ViewModels
{
    internal interface IMediaCapture
    {
        #region Properties
        public MediaCapture MCapture { get; }
        #endregion

        #region Methods
        internal Task<List<VideoDevice>> InitializeVideoDevices();

        internal Task<List<AudioDevice>> InitializeAudioDevices();

        internal Task PauseRecord();

        internal Task ResumeRecord();

        internal Task<BitmapImage> CapturePhoto(string filename);

        internal Task InitializeCapture(string videoDevId, string audioDevId, int selectedVideoDeviceIndex);

        internal Task StartRecord(string fileName);

        internal Task StopRecord();

        internal void DisposeAndStop();
        #endregion
    }
}
