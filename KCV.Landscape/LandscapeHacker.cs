using Grabacr07.KanColleViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Grabacr07.KanColleViewer.ViewModels.Contents;

namespace Gizeta.KCV.Landscape
{
    public class LandscapeHacker
    {
        private static readonly LandscapeHacker instance = new LandscapeHacker();

        private ContentControl mainToolsView;

        private LandscapeHacker()
        {
        }

        public static LandscapeHacker Instance
        {
            get { return instance; }
        }
        
        public void Hack()
        {
            var resDict = new ResourceDictionary();
            resDict.Source = new Uri("pack://application:,,,/KCV.Landscape;component/LandscapeHacker.Resource.xaml");
            KCVUIHelper.KCVWindow.Resources.MergedDictionaries.Add(resDict);

            if (PluginSettings.Current.InsertScrollBarToPluginTab)
            {
                KCVUIHelper.KCVContent.FindVisualChildren<ListBoxItem>().Where(x => x.DataContext is ToolsViewModel).First().Selected += MainToolsTab_Selected;
            }
        }

        private void insertScrollBarToPluginTab()
        {
            var listBox = KCVUIHelper.KCVContent.FindVisualChildren<ListBox>().Where(x => x.DataContext is ToolsViewModel).First();
            listBox.Template = KCVUIHelper.KCVWindow.Resources["LandscapePluginTabListBoxKey"] as ControlTemplate;
            ScrollViewer.SetCanContentScroll(listBox, false);
        }

        private void MainToolsTab_Selected(object sender, RoutedEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            listBoxItem.Selected -= MainToolsTab_Selected;
            mainToolsView = KCVUIHelper.KCVContent.FindVisualChildren<ContentControl>().Where(x => x.DataContext is ToolsViewModel).Last();
            mainToolsView.LayoutUpdated += MainToolsView_LayoutUpdated;
        }

        private void MainToolsView_LayoutUpdated(object sender, EventArgs e)
        {
            mainToolsView.LayoutUpdated -= MainToolsView_LayoutUpdated;
            mainToolsView = null;
            KCVUIHelper.OperateMainWindow(() => insertScrollBarToPluginTab());
        }
    }
}
