using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.Desktop.Metro.Converters;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views.Controls;
using Livet;
using MetroRadiance.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace Gizeta.KCV.Landscape
{
    public class LandscapeViewModel : NotificationObject
    {
        private readonly static LandscapeViewModel instance = new LandscapeViewModel();

        private CallMethodButton windowOpenButton;
        private Grid contentContainer;
        private KanColleHost hostControl;
        private FrameworkElement pluginControl;

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
            createButton();

            hostControl = KCVUIHelper.KCVWindow.FindChildren<KanColleHost>().First();            
            contentContainer = hostControl.Parent as Grid;
            pluginControl = (from view in contentContainer.FindChildren<ContentControl>()
                             where view.Content is StartContentViewModel || view.Content is MainContentViewModel
                             select view as FrameworkElement).First();

            hostControl.ZoomFactor = this.BrowserZoomFactor / 100.0;
            switchLayout(CurrentLayout);
        }

        public KCVContentLayout CurrentLayout
        {
            get { return PluginSettings.Current.Layout; }
            set
            {
                if (PluginSettings.Current.Layout != value)
                {
                    switchLayout(PluginSettings.Current.Layout, value);

                    PluginSettings.Current.Layout = value;
                    PluginSettings.Current.Save();

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

                    hostControl.ZoomFactor = value / 100.0;

                    this.RaisePropertyChanged();
                }
            }
        }

        public void AdjustWindow()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                KCVUIHelper.KCVWindow.Width = this.HostWidth;
                KCVUIHelper.KCVWindow.Height = this.HostHeight + 59;
            }
        }

        public void AdjustHost()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                this.BrowserZoomFactor = (int)Math.Floor(100.0 * Math.Min(KCVUIHelper.KCVWindow.ActualWidth / 800, (KCVUIHelper.KCVWindow.ActualHeight - 59) / 480));
            }
        }

        public void OpenWindow()
        {
            if (CurrentLayout == KCVContentLayout.Separate && MainContentWindow.Current == null)
            {
                var window = new MainContentWindow
                {
                    DataContext = KCVApp.ViewModelRoot,
                    Width = PluginSettings.Current.WindowWidth,
                    Height = PluginSettings.Current.WindowHeight
                };
                window.Show();
                this.IsWindowOpenButtonShow = false;
            }
        }

        private bool isWindowOpenButtonShow = false;
        public bool IsWindowOpenButtonShow
        {
            get { return isWindowOpenButtonShow; }
            internal set
            {
                if (isWindowOpenButtonShow != value)
                {
                    isWindowOpenButtonShow = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double HostWidth
        {
            get { return 800.0 * this.BrowserZoomFactor / 100; }
        }

        public double HostHeight
        {
            get { return 480.0 * this.BrowserZoomFactor / 100; }
        }

        private void switchLayout(KCVContentLayout newValue)
        {
            switchLayout(KCVContentLayout.Portrait, newValue);
        }

        private void switchLayout(KCVContentLayout oldValue, KCVContentLayout newValue)
        {
            if (oldValue == KCVContentLayout.Separate)
            {
                if (newValue == KCVContentLayout.Portrait)
                {
                    KCVUIHelper.KCVWindow.Width = Math.Max(this.HostWidth, MainContentWindow.Current.ActualWidth);
                    KCVUIHelper.KCVWindow.Height = this.HostHeight + MainContentWindow.Current.ActualHeight;
                }
                else
                {
                    KCVUIHelper.KCVWindow.Width = this.HostWidth + MainContentWindow.Current.ActualWidth;
                    KCVUIHelper.KCVWindow.Height = Math.Max(this.HostHeight, MainContentWindow.Current.ActualHeight);
                }
                MainContentWindow.Current.Close();
                pluginControl.Visibility = Visibility.Visible;
                this.IsWindowOpenButtonShow = false;
            }

            contentContainer.RowDefinitions.Clear();
            contentContainer.ColumnDefinitions.Clear();

            if (newValue == KCVContentLayout.Separate)
            {
                var window = new MainContentWindow
                {
                    DataContext = KCVApp.ViewModelRoot,
                    Width = PluginSettings.Current.WindowWidth,
                    Height = PluginSettings.Current.WindowHeight
                };
                window.Show();

                KCVUIHelper.KCVWindow.Width = this.HostWidth;
                KCVUIHelper.KCVWindow.Height = this.HostHeight + 59;
                pluginControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (newValue == KCVContentLayout.Portrait)
                {
                    if (oldValue != KCVContentLayout.Separate)
                    {
                        KCVUIHelper.KCVWindow.Width = Math.Max(this.HostWidth, pluginControl.ActualWidth);
                        KCVUIHelper.KCVWindow.Height = this.HostHeight + pluginControl.ActualHeight;
                    }
                    
                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    Grid.SetRow(pluginControl, 1);
                    Grid.SetRowSpan(hostControl, 1);
                }
                else
                {
                    if (newValue == KCVContentLayout.LandscapeLeft)
                    {
                        if (oldValue == KCVContentLayout.Portrait)
                        {
                            KCVUIHelper.KCVWindow.Width = this.HostWidth + pluginControl.ActualWidth;
                            KCVUIHelper.KCVWindow.Height = Math.Max(this.HostHeight, pluginControl.ActualHeight);
                        }

                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                        Grid.SetColumn(hostControl, 0);
                        Grid.SetColumn(pluginControl, 1);
                        Grid.SetColumnSpan(hostControl, 1);
                    }
                    else
                    {
                        if (oldValue == KCVContentLayout.Portrait)
                        {
                            KCVUIHelper.KCVWindow.Width = this.HostWidth + pluginControl.ActualWidth;
                            KCVUIHelper.KCVWindow.Height = Math.Max(this.HostHeight, pluginControl.ActualHeight);
                        }

                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        Grid.SetColumn(hostControl, 1);
                        Grid.SetColumn(pluginControl, 0);
                    }
                }
            }
        }

        private void createButton()
        {
            var resDict = new ResourceDictionary();
            resDict.Source = new Uri("pack://application:,,,/KCV.Landscape;component/Style.Controls.WindowOpenButton.xaml");
            KCVUIHelper.KCVWindow.Resources.MergedDictionaries.Add(resDict);

            var topButtonContainer = KCVUIHelper.KCVWindow.FindChildren<ZoomFactorSelector>().First().Parent as StackPanel;
            windowOpenButton = new CallMethodButton();
            windowOpenButton.ToolTip = "恢复关闭的窗口";
            windowOpenButton.SetResourceReference(Control.StyleProperty, "WindowOpenButtonStyleKey");
            windowOpenButton.MethodName = "OpenWindow";
            windowOpenButton.MethodTarget = this;

            var buttonBinding = new Binding();
            buttonBinding.Source = this;
            buttonBinding.Path = new PropertyPath("IsWindowOpenButtonShow");
            buttonBinding.Mode = BindingMode.TwoWay;
            buttonBinding.Converter = new UniversalBooleanToVisibilityConverter();
            windowOpenButton.SetBinding(CaptionButton.VisibilityProperty, buttonBinding);
            topButtonContainer.Children.Insert(0, windowOpenButton);
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
