using Grabacr07.KanColleViewer.Views;
using System.ComponentModel;

namespace Gizeta.KCV.Landscape
{
    public partial class MainContentWindow
    {
        public static MainContentWindow Current { get; private set; }
        
        public MainContentWindow()
        {
            InitializeComponent();

            Current = this;
            MainWindow.Current.Closed += (sender, args) => this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Current = null;

            PluginSettings.Current.WindowWidth = this.ActualWidth;
            PluginSettings.Current.WindowHeight = this.ActualHeight;

            base.OnClosing(e);
        }
    }
}
