using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Data.Xml;
using System;
using System.IO;

namespace Gizeta.KCV.Landscape
{
    [Serializable]
    public class PluginSettings
    {
        private static readonly string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "grabacr.net",
            "KanColleViewer",
            "KCV.Landscape.xml");

        public static PluginSettings Current { get; set; }

        public static void Load()
        {
            try
            {
                Current = filePath.ReadXml<PluginSettings>();
            }
            catch(Exception ex)
            {
                Current = GetInitialSettings();
            }
        }

        public void Save()
        {
            try
            {
                this.WriteXml(filePath);
            }
            catch (Exception ex) { }
        }

        public static PluginSettings GetInitialSettings()
        {
            return new PluginSettings
            {
                Layout = KCVContentLayout.Portrait,
                WindowWidth = 800,
                WindowHeight = 400,
                BrowserZoomFactor = Settings.Current.BrowserZoomFactorPercentage
            };
        }

        public KCVContentLayout Layout { get; set; }

        public double WindowWidth { get; set; }

        public double WindowHeight { get; set; }

        public int BrowserZoomFactor { get; set; }
    }
}
