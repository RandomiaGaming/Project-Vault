using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android.OS.Storage;

namespace ProjectVault
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriveSelectorPage : ContentPage
    {
        private readonly App app;
        public DriveSelectorPage(App app)
        {
            if (app is null)
            {
                throw new NullReferenceException("App cannot be null.");
            }
            this.app = app;
            InitializeComponent();
            PathEntry.Text = "/";
            foreach (string drive in PathHelper.GetDrives(app.GetContext()))
            {
                Button button = new Button();
                button.BindingContext = drive;
                button.Text = drive;
                button.FontSize = 20;
                button.TextColor = Color.Black;
                button.FontAttributes = FontAttributes.Bold;
                button.BackgroundColor = new Color(1.0, 0.58823529411, 0.58823529411);
                button.CornerRadius = 10;
                button.HeightRequest = 50;
                button.VerticalOptions = LayoutOptions.Fill;
                button.Clicked += OnBrowserButtonPressed;
                BrowserStackLayout.Children.Add(button);
            }
        }
        public void OnPathEntryUnfocused(object sender, EventArgs e)
        {
            app.SwitchPath(PathEntry.Text);
        }
        public void OnBrowserButtonPressed(object sender, EventArgs e)
        {
            Button senderButton = (Button)sender;
            string senderButtonPath = (string)senderButton.BindingContext;
            app.SwitchPath(senderButtonPath);
        }
        public void OnRefreshButtonPressed(object sender, EventArgs e)
        {
            app.SwitchPath(app.currentPath);
        }
    }
}