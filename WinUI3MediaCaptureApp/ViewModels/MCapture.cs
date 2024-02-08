using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using WinUI3MediaCaptureApp.Models;
using Windows.Media.Core;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace WinUI3MediaCaptureApp.ViewModels
{
    internal class MCapture : IMediaCapture
    {
#nullable enable
        private MediaCapture? _mediaCapture;
        private MainViewModel _mainViewModel;

        public MCapture(MainViewModel viewModel)
        {
            _mainViewModel = viewModel;
        }

        MediaCapture? IMediaCapture.MCapture => _mediaCapture;

        async Task<List<AudioDevice>> IMediaCapture.InitializeAudioDevices()
        {
            var _audioDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);

            List<AudioDevice> audioDevices = new List<AudioDevice>();

            foreach (var device in _audioDevices)
            {
                audioDevices.Add(new AudioDevice { Name = device.Name, Id = device.Id });
            }

            return audioDevices;
        }

        async Task<List<VideoDevice>> IMediaCapture.InitializeVideoDevices()
        {
            var _videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            List<VideoDevice> videoDevices = new List<VideoDevice>();

            foreach (var device in _videoDevices)
            {
                videoDevices.Add(new VideoDevice { Name = device.Name, Id = device.Id });
            }

            return videoDevices;
        }

        async Task IMediaCapture.InitializeCapture(string videoDevId, string audioDevId, int selectedVideoDeviceIndex)
        {
            _mediaCapture = new MediaCapture();

            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

            MediaFrameSourceGroup selectedFrameSourceGroup = frameSourceGroups[selectedVideoDeviceIndex];

            var mediaCaptureInitializationSettings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedFrameSourceGroup,
                AudioDeviceId = audioDevId,
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };

            await _mediaCapture.InitializeAsync(mediaCaptureInitializationSettings);

            _mainViewModel.MediaSource = MediaSource.CreateFromMediaFrameSource(_mediaCapture.FrameSources[selectedFrameSourceGroup.SourceInfos[selectedVideoDeviceIndex].Id]);
        }

        async Task IMediaCapture.StartRecord(string fileName)
        {
            if (_mediaCapture != null)
            {
                StorageFile videoFile = await KnownFolders.VideosLibrary.CreateFileAsync(
                    $"{fileName}.mp4", CreationCollisionOption.OpenIfExists);

                MediaEncodingProfile profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);

                await _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);
            }
        }

        async Task IMediaCapture.StopRecord()
        {
            if (_mediaCapture != null && _mediaCapture.CameraStreamState == CameraStreamState.Streaming)
            {
                await _mediaCapture.StopRecordAsync();
            }
            else if(_mediaCapture != null && _mediaCapture.CameraStreamState == CameraStreamState.NotStreaming)
            {
                await _mediaCapture.ResumeRecordAsync();
                await _mediaCapture.StopRecordAsync();
            }
        }

        void IMediaCapture.DisposeAndStop()
        {
            if (_mediaCapture != null)
            {
                using (var mediaCapture = _mediaCapture)
                {
                    _mediaCapture = null;
                }
            }
        }

        async Task IMediaCapture.PauseRecord()
        {
            if(_mediaCapture != null && _mediaCapture.CameraStreamState == CameraStreamState.Streaming)
            {
                await _mediaCapture.PauseRecordAsync(MediaCapturePauseBehavior.RetainHardwareResources);
            }
        }

        async Task IMediaCapture.ResumeRecord()
        {
            if (_mediaCapture != null && _mediaCapture.CameraStreamState == CameraStreamState.NotStreaming)
            {
                await _mediaCapture.ResumeRecordAsync();
            }
        }

        async Task<BitmapImage?> IMediaCapture.CapturePhoto(string filename)
        {
            BitmapImage? result = null;

            if (_mediaCapture != null)
            {
                var myPictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                StorageFile imgfile = await myPictures.SaveFolder.CreateFileAsync($"{filename}.jpg", CreationCollisionOption.GenerateUniqueName);

                await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), imgfile);

                result = new BitmapImage(new Uri(imgfile.Path));
            }
            return result;
        }
#nullable disable
    }
}
