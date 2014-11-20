using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizeta.KCV.Landscape
{
    public enum KCVContentLayout
    {
        Portrait,
        LandscapeLeft,
        LandscapeRight,
        Separate
    }
    
    public class LandscapeExtention
    {
        private static readonly LandscapeExtention instance = new LandscapeExtention();

        private LandscapeExtention()
        {
        }

        public static LandscapeExtention Instance
        {
            get { return instance; }
        }

        public void Initialize()
        {
            KCVUIHelper.OperateMainWindow(async () =>
            {
                await Task.Delay(5000);

                LandscapeViewModel.Instance.Initialize();
            });
        }
    }
}
