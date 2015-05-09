using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template10.Services.AdService
{
    class AdService
    {
        AdHelper _helper;

        public AdService()
        {
            // for demo only
            var adUnitId = "11389925";
            var appId = "d25517cb-12d4-4699-8bdc-52040c712cab";
            _helper = new AdHelper(appId, adUnitId);
        }

        // don't try to show while showing
        private bool _busy = false;

        // don't show in some other instance if already shown
        private static bool _shown = false;

        public void Show()
        {
            if (_busy)
                return;
            _busy = true;

            if (_shown)
                return;

            _helper.AfterComplete = () => { _shown = true; AfterShown?.Invoke(); };
            _helper.AfterCanceled = () => { _shown = false; };
            _helper.AfterError = (e) => { _shown = true; };
            _helper.Preload(true);
        }

        public Action AfterReady { get; set; }
        public Action AfterShown { get; set; }
    }
}
