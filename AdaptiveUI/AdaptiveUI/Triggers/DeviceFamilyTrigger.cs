using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Template10.Triggers
{
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _deviceFamily;
        public string DeviceFamily
        {
            get { return _deviceFamily; }
            set
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core
                    .ResourceContext.GetForCurrentView().QualifierValues;
                if (qualifiers.ContainsKey("DeviceFamily"))
                    SetActive(qualifiers["DeviceFamily"] == (_deviceFamily = value));
                else
                    SetActive(false);
            }
        }
    }

}
