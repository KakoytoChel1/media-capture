using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Media.Core;
using WinUI3MediaCaptureApp.Models;

namespace WinUI3MediaCaptureApp.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {

        public IMediaCapture mCapture;
        private DispatcherTimer timer;
        private TimeSpan recordingTime;

#nullable enable
        public MainViewModel()
        {
            mCapture = new MCapture(this);
            IsMaximized = false;
            SelectedVideoDeviceIndex = 0;
            SelectedAudioDeviceIndex = 0;
            IsCapturing = false;
            IsRecording = false;

            VideoDevices = new ObservableCollection<VideoDevice>();
            AudioDevices = new ObservableCollection<AudioDevice>();

            SetMediaDevices();
            InitializeTimer();
        }

        #region Properties

        [ObservableProperty]
        private ObservableCollection<VideoDevice>? _videoDevices;

        [ObservableProperty]
        private ObservableCollection<AudioDevice>? _audioDevices;

        [ObservableProperty]
        private AudioDevice? _selectedAudioDevice;

        [ObservableProperty]
        private VideoDevice? _selectedVideoDevice;

        [ObservableProperty]
        private BitmapImage? _capturedImageSource;

        [ObservableProperty]
        private MediaSource? _mediaSource;

        [ObservableProperty]
        private string? _recordVideoFileName;

        [ObservableProperty]
        private bool? _isMaximized;

        [ObservableProperty]
        private bool _isCapturing;

        [ObservableProperty]
        private bool _recordingIsPaused;

        [ObservableProperty]
        private XamlRoot? _root;

        [ObservableProperty]
        private bool _isRecording;

        [ObservableProperty]
        private int _selectedVideoDeviceIndex;

        [ObservableProperty]
        private int _selectedAudioDeviceIndex;

        [ObservableProperty]
        private string? _currentRecordingTimeStr;
        #endregion

        #region Commands

        [RelayCommand]
        private void AppClose()
        {
            mCapture.DisposeAndStop();
        }

        [RelayCommand]
        private async Task StartCapture()
        {
            if (SelectedVideoDevice != null && SelectedAudioDevice != null)
            {
                var videoId = SelectedVideoDevice.Id;
                var audioId = SelectedAudioDevice.Id;

                if (videoId != null && audioId != null)
                {
                    await mCapture.InitializeCapture(videoId, audioId, SelectedVideoDeviceIndex);
                    IsCapturing = true;
                }
            }
        }

        [RelayCommand]
        private async Task StopCapture()
        {

            if (IsRecording)
            {
                await mCapture.StopRecord();
                IsRecording = false;
                RecordingIsPaused = false;
            }

            mCapture.DisposeAndStop();
            IsCapturing = false;

            MediaSource = null;
        }

        [RelayCommand]
        private async Task StartRecord(XamlRoot root)
        {
            if (IsCapturing)
            {
                if (!IsRecording)
                {
                    ContentDialog contentDialog = new ContentDialog()
                    {
                        XamlRoot = root,
                        Title = "Creating new video file",
                        Content = new AddNewRecordPage(),
                        IsPrimaryButtonEnabled = true,
                        PrimaryButtonText = "OK",
                        CloseButtonText = "Cancel"
                    };

                    var content = contentDialog.Content as AddNewRecordPage;
                    if (content != null)
                    {
                        content.SetInputText(MainModel.CreateVideoFileName());

                        if (await contentDialog.ShowAsync() is ContentDialogResult.Primary)
                        {
                            string fileName = content.GetInputText();

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                await mCapture.StartRecord(fileName);
                                IsRecording = true;
                                StartTimer();
                            }
                        }
                    }
                }
                else if(IsRecording)
                {
                    await mCapture.StopRecord();
                    StopTimer();
                    IsRecording = false;
                    RecordingIsPaused = false;
                }
            }
        }

        [RelayCommand]
        private async Task PauseRecording()
        {
            if (IsRecording == true && IsCapturing == true)
            {
                if (!RecordingIsPaused)
                {
                    await mCapture.PauseRecord();
                    RecordingIsPaused = true;
                }
                else
                {
                    await mCapture.ResumeRecord();
                    RecordingIsPaused = false;
                }
            }
        }

        [RelayCommand]
        private async Task StopRecord()
        {
            if (IsCapturing)
            {
                if (IsRecording)
                {
                    await mCapture.StopRecord();
                    IsRecording = false;
                }
            }
        }

        [RelayCommand]
        private async Task CapturePhoto()
        {
            if (IsCapturing)
            {
                CapturedImageSource = await mCapture.CapturePhoto(MainModel.CreatePhotoFileName());
            }
        }
        #endregion

        #region Other methods

        private async void SetMediaDevices()
        {
            List<VideoDevice> vDev = await mCapture.InitializeVideoDevices();
            List<AudioDevice> aDev = await mCapture.InitializeAudioDevices();

            await Task.Run(() =>
            {
                foreach (var device in vDev)
                {
                    if (VideoDevices != null)
                    {
                        VideoDevices.Add(device);
                    }
                }

                foreach (var device in aDev)
                {
                    if (AudioDevices != null)
                    {
                        AudioDevices.Add(device);
                    }
                }
            });

        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, object e)
        {
            if (IsRecording && !RecordingIsPaused)
            {
                recordingTime = recordingTime.Add(TimeSpan.FromSeconds(1));
                CurrentRecordingTimeStr = recordingTime.ToString(@"hh\:mm\:ss");
            }
        }

        private void StartTimer()
        {
            recordingTime = TimeSpan.Zero;
            CurrentRecordingTimeStr = recordingTime.ToString(@"hh\:mm\:ss");
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
            recordingTime = TimeSpan.Zero;
            CurrentRecordingTimeStr = recordingTime.ToString(@"hh\:mm\:ss");
        }
        #endregion

#nullable disable
    }
}
