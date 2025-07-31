using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectVault
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EncryptedFileInspectorPage : ContentPage
    {
        private readonly App app;
        private readonly string filePath;
        public EncryptedFileInspectorPage(App app, string filePath)
        {
            if (app is null)
            {
                throw new NullReferenceException("App cannot be null.");
            }
            this.app = app;
            if (!PathHelper.IsPathFile(filePath))
            {
                throw new Exception("filePath is invalid.");
            }
            this.filePath = filePath;
            InitializeComponent();
            PathEntry.Text = filePath;
        }
        public void OnDecryptFileButtonPressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
            File.Move(filePath, filePath.Substring(0, filePath.Length - 4));
        }
        public void OnPathEntryUnfocused(object sender, EventArgs e)
        {
            app.SwitchPath(PathEntry.Text);
        }
        public void OnVirtualBackButtonPressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
        protected override bool OnBackButtonPressed()
        {
            app.SwitchPath(PathHelper.GetParentDirectory(filePath));
            return true;
        }
    }
}