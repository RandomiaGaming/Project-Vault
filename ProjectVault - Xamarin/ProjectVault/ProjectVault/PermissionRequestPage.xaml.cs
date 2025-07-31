using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectVault
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionRequestPage : ContentPage
    {
        private readonly App app;
        public PermissionRequestPage(App app)
        {
            if (app is null)
            {
                throw new NullReferenceException("App cannot be null.");
            }
            this.app = app;
            InitializeComponent();
        }
        public void OnGrantPermissionsButtonPressed(object sender, EventArgs e)
        {
            app.PromptForStorageAccess();
        }
        public void OnCloseAppButtonPressed(object sender, EventArgs e)
        {
            app.activity.Finish();
        }
    }
}