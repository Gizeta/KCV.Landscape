﻿using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;

namespace Gizeta.KCV.Landscape
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "KCV.Landscape")]
    [ExportMetadata("Description", "KanColleViewer布局切换插件。")]
    [ExportMetadata("Version", "1.0.0")]
    [ExportMetadata("Author", "@Gizeta")]
    public class PluginLoader : IToolPlugin
    {
        private static bool hasInitialized = false;

        public PluginLoader()
        {
            if(!hasInitialized)
            {
                hasInitialized = true;

                PluginSettings.Load();
                LandscapeExtention.Instance.Initialize();
            }
        }

        public string ToolName
        {
            get { return "Landscape"; }
        }

        public object GetSettingsView()
        {
            return null;
        }

        public object GetToolView()
        {
            return new LandscapeView { DataContext = LandscapeViewModel.Instance };
        }
    }
}
