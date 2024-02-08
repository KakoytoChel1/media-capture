using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3MediaCaptureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewRecordPage : Page
    {
        public AddNewRecordPage()
        {
            this.InitializeComponent();
        }

        public string GetInputText() => this.InputTextBox.Text;

        public void SetInputText(string text)
        {
            this.InputTextBox.Text = text;
        }
    }
}
