using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;

namespace CameraViewRapidShootCrash
{
    public partial class MainPage : ContentPage
    {
        readonly Task initTask;

        IReadOnlyList<CameraInfo>? cameras;

        int selectedCamera;

        public MainPage()
        {
            InitializeComponent();

            Camera.MediaCaptured += Camera_MediaCaptured;
            Camera.MediaCaptureFailed += Camera_MediaCaptureFailed;

            initTask = Init();
        }

        async Task Init()
        {
            try
            {
                cameras = await Camera.GetAvailableCameras(CancellationToken.None);
                await Camera.StartCameraPreview(CancellationToken.None);
                ShowDebugText("Camera preview started");
            }
            catch (Exception ex)
            {
                ShowDebugText($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        void ShowDebugText(string message, [CallerMemberName] string memberName = "")
        {
            DebugLabel.Text += $"{memberName}.{message}\n";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Camera.CaptureImage(CancellationToken.None);
            }
            catch (Exception ex)
            {
                ShowDebugText($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        private void NextCamera_Clicked(object sender, EventArgs e)
        {
            Camera.SelectedCamera = cameras?[++selectedCamera % cameras.Count];
        }

        private async void Camera_MediaCaptured(object? sender, MediaCapturedEventArgs e)
        {
            await initTask;

            CapturedImage.Source = ImageSource.FromStream(() => e.Media);
        }

        private void Camera_MediaCaptureFailed(object? sender, MediaCaptureFailedEventArgs e)
        {
            ShowDebugText($"Camera_MediaCaptureFailed: {e.FailureReason}");
        }
    }
}
