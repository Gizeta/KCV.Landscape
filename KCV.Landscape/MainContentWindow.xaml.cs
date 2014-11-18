using Grabacr07.KanColleViewer.Views;
using System;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Current = null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            PluginSettings.Current.WindowWidth = this.ActualWidth;
            PluginSettings.Current.WindowHeight = this.ActualHeight;

            if(PluginSettings.Current.Layout == KCVContentLayout.Separate)
                e.Cancel = true;
        }
    }
}
