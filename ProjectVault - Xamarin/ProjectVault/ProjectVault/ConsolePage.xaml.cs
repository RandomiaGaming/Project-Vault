using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectVault
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsolePage : ContentPage
    {
        private readonly App app;
        public ConsolePage(App app)
        {
            if (app is null)
            {
                throw new NullReferenceException("App cannot be null.");
            }
            this.app = app;
            InitializeComponent();
            ScrollConsoleToBottom();
        }
        public void ScrollConsoleToBottom()
        {
            ConsoleContainer.ScrollToAsync(ConsoleText, ScrollToPosition.End, false);
        }
        public void OnEncryptPageButtonPressed(object sender, EventArgs e)
        {
            app.SwitchToEncryptPage();
        }
        public void OnDecryptPageButtonPressed(object sender, EventArgs e)
        {
            app.SwitchToDecryptPage();
        }
        public void OnConsolePageButtonPressed(object sender, EventArgs e)
        {
            app.SwitchToConsolePage();
        }
        public void OnRunCommandButtonPressed(object sender, EventArgs e)
        {
            string command = CommandEntry.Text;
            CommandEntry.Text = "";
            if (command == "Dunky")
            {
                for (int i = 0; i < 100; i++)
                {
                    ConsoleText.Text = ConsoleText.Text + "\n" + "LAL";
                }
            }
            else
            {
                ConsoleText.Text = ConsoleText.Text + "\n" + command;
            }
            ScrollConsoleToBottom();
        }
    }
}