using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Views.Controls;
using Livet;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace Gizeta.KCV.Landscape
{
    public enum KCVContentLayout
    {
        Portrait,
        LandscapeLeft,
        LandscapeRight,
        Separate
    }

    public class LandscapeViewModel : NotificationObject
    {
        private readonly static LandscapeViewModel instance = new LandscapeViewModel();

        private Grid layoutRoot = null;
        private Grid contentContainer = null;
        private FrameworkElement hostControl = null;
        private FrameworkElement pluginControl = null;

        private LandscapeViewModel()
        {
            Settings.Current.PropertyChanged += Settings_PropertyChanged;
        }

        public static LandscapeViewModel Instance
        {
            get { return instance; }
        }

        public void Initialize()
        {
            KCVApp.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(5000);

                switchLayout();
                (hostControl as KanColleHost).ZoomFactor = this.BrowserZoomFactor / 100.0;
            });
        }

        public KCVContentLayout CurrentLayout
        {
            get { return PluginSettings.Current.Layout; }
            set
            {
                if (PluginSettings.Current.Layout != value)
                {
                    PluginSettings.Current.Layout = value;
                    PluginSettings.Current.Save();

                    switchLayout();

                    this.RaisePropertyChanged();
                }
            }
        }

        public int BrowserZoomFactor
        {
            get { return PluginSettings.Current.BrowserZoomFactor; }
            set
            {
                if (PluginSettings.Current.BrowserZoomFactor != value)
                {
                    PluginSettings.Current.BrowserZoomFactor = value;
                    PluginSettings.Current.Save();

                    (hostControl as KanColleHost).ZoomFactor = value / 100.0;

                    this.RaisePropertyChanged();
                }
            }
        }

        public ICommand LandscapeLeft
        {
            get
            {
                return new RelayCommand(() =>
                {
                    findControls();
                    closeContentWindow();

                    pluginControl.Visibility = Visibility.Visible;

                    contentContainer.RowDefinitions.Clear();

                    contentContainer.ColumnDefinitions.Clear();
                    contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    Grid.SetColumn(hostControl, 1);
                    Grid.SetColumn(pluginControl, 0);
                });
            }
        }

        public ICommand LandscapeRight
        {
            get
            {
                return new RelayCommand(() =>
                {
                    findControls();
                    closeContentWindow();

                    pluginControl.Visibility = Visibility.Visible;

                    contentContainer.RowDefinitions.Clear();

                    contentContainer.ColumnDefinitions.Clear();
                    contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    Grid.SetColumn(hostControl, 0);
                    Grid.SetColumn(pluginControl, 1);
                    Grid.SetColumnSpan(hostControl, 1);
                });
            }
        }

        public ICommand Portrait
        {
            get
            {
                return new RelayCommand(() =>
                {
                    findControls();
                    closeContentWindow();

                    pluginControl.Visibility = Visibility.Visible;

                    contentContainer.RowDefinitions.Clear();
                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    contentContainer.ColumnDefinitions.Clear();

                    Grid.SetRow(pluginControl, 1);
                    Grid.SetRowSpan(hostControl, 1);
                });
            }
        }

        public ICommand Separate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    findControls();
                    
                    var window = new MainContentWindow
                    {
                        DataContext = KCVApp.ViewModelRoot,
                        Width = PluginSettings.Current.WindowWidth,
                        Height = PluginSettings.Current.WindowHeight
                    };
                    window.Show();

                    KCVApp.Current.MainWindow.Width = 800 * BrowserZoomFactor / 100.0;
                    KCVApp.Current.MainWindow.Height = 480 * BrowserZoomFactor / 100.0 + 59;
                    pluginControl.Visibility = Visibility.Collapsed;

                    contentContainer.RowDefinitions.Clear();
                    contentContainer.ColumnDefinitions.Clear();
                });
            }
        }

        public void AdjustWindow()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                KCVApp.Current.MainWindow.Width = 800 * BrowserZoomFactor / 100.0;
                KCVApp.Current.MainWindow.Height = 480 * BrowserZoomFactor / 100.0 + 59;
            }
        }

        public void AdjustHost()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                this.BrowserZoomFactor = (int)Math.Floor(100.0 * Math.Min(KCVApp.Current.MainWindow.ActualWidth / 800, (KCVApp.Current.MainWindow.ActualHeight - 59) / 480));
            }
        }

        private void findControls()
        {
            if (KCVApp.Current.MainWindow != null && layoutRoot == null)
            {
                layoutRoot = KCVApp.Current.MainWindow.Content as Grid;
                contentContainer = layoutRoot.Children[1] as Grid;
                hostControl = contentContainer.Children[0] as FrameworkElement;
                pluginControl = contentContainer.Children[1] as FrameworkElement;
            }
        }

        private void closeContentWindow()
        {
            if (MainContentWindow.Current != null)
            {
                KCVApp.Current.MainWindow.Width = Math.Max(hostControl.ActualWidth, MainContentWindow.Current.ActualWidth);
                KCVApp.Current.MainWindow.Height = 720;

                MainContentWindow.Current.Close();
            }
        }

        private void switchLayout()
        {
            switch (PluginSettings.Current.Layout)
            {
                case KCVContentLayout.Portrait:
                    this.Portrait.Execute(null);
                    break;
                case KCVContentLayout.LandscapeLeft:
                    this.LandscapeLeft.Execute(null);
                    break;
                case KCVContentLayout.LandscapeRight:
                    this.LandscapeRight.Execute(null);
                    break;
                case KCVContentLayout.Separate:
                    this.Separate.Execute(null);
                    break;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "BrowserZoomFactor")
            {
                this.BrowserZoomFactor = Settings.Current.BrowserZoomFactorPercentage;
            }
        }
    }
}
