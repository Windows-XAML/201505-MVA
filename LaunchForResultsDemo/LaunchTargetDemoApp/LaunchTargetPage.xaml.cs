using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LaunchTargetDemoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchTargetPage : Page
    {
        private ProtocolForResultsOperation _protocolForResultsOperation = null;

        public LaunchTargetPage()
        {
            this.InitializeComponent();
        }

        public ProtocolForResultsOperation ProtocolForResultsOperation
        {
            set
            {
                _protocolForResultsOperation = value;
            }
        }

        private void SendResults_Click(object sender, RoutedEventArgs e)
        {
            var values = new ValueSet();
            values["Answer"] = 42;
            _protocolForResultsOperation.ReportCompleted(values);
        }
    }
}
