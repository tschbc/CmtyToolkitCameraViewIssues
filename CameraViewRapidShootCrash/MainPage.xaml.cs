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
            }
            catch (Exception ex)
            {
                ShowErrorText($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        void ShowErrorText(string message, [CallerMemberName] string memberName = "")
        {
            ErrorsLabel.Text += $"{memberName}.{message}\n";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Camera.CaptureImage(CancellationToken.None);
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
            ShowErrorText($"Camera_MediaCaptureFailed: {e.FailureReason}");
        }
    }
}
