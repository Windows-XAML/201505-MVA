using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Template10.Services.AdService
{
    public class AdHelper
    {
        private InterstitialAd _ad;
        string _applicationId = string.Empty;
        string _adUnitId = string.Empty;

        public AdHelper(string applicationId, string adUnitId)
        {
            _applicationId = applicationId;
            _adUnitId = adUnitId;

            _ad = new InterstitialAd();
            _ad.AdReady += (s, e) => AfterReady?.Invoke();
            _ad.Completed += (s, e) => { if (IsReady) AfterComplete?.Invoke(); };
            _ad.Cancelled += (s, e) => AfterCanceled?.Invoke();
            _ad.ErrorOccurred += (s, e) => AfterError?.Invoke(e);
        }

        public bool IsReady { get { return _ad?.State == InterstitialAdState.Ready; } }
        public Action AfterReady { get; set; }
        public Action AfterComplete { get; set; }
        public Action AfterCanceled { get; set; }
        public Action<AdErrorEventArgs> AfterError { get; set; }

        public void Preload(bool show = false)
        {
            if (show)
                this.AfterReady += () => Show();
            _ad.RequestAd(AdType.Video, _applicationId, _adUnitId);
        }

        public void Show()
        {
            _ad.Show();
        }
    }
}
