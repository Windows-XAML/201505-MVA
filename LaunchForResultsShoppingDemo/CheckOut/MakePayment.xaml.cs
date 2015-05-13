
namespace CheckOut
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Windows.ApplicationModel.Activation;   
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Windows.Foundation.Collections;
    using Windows.System;
    using Newtonsoft.Json;
    using CheckOut.Models;

    public sealed partial class MakePayment : Page
    {
        // reference used for completing continuation
        private ProtocolForResultsOperation operation;
        private string totalAmount;
        private string transaction;


        // List of known apps that are authorised to use the CheckOut
        List<Tuple<string, string>> validCallers = new List<Tuple<string, string>>();

        public MakePayment()
        {
            this.InitializeComponent();

            // Tracking PFN and "friendly" name of app for easy reference - no-one wants to remember PFNs
            this.validCallers.Add(new Tuple<string, string>("38e7759a-c952-4109-87f7-b8c2235e5765_z5jhcbv2wstvg", "Demo Shop"));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var protocolForResultsArgs = e.Parameter as ProtocolForResultsActivatedEventArgs;
            this.operation = protocolForResultsArgs.ProtocolForResultsOperation;
            var callerPfn = protocolForResultsArgs.CallerPackageFamilyName;

            // We only want certain, approved apps to use our checkout functionality so check the caller is one
            // This is, of course, optional. You don't have to do this.
            if (this.validCallers.Any(c => c.Item1.Equals(callerPfn)))
            {
                if (protocolForResultsArgs.Data.Keys.Count == 2
                    && protocolForResultsArgs.Data.ContainsKey("Transaction")
                    && protocolForResultsArgs.Data.ContainsKey("Items"))
                {
                    // This is a bit hacky - you could put this into a view model if you wanted
                    this.transaction = protocolForResultsArgs.Data["Transaction"] as string;
                    var items = protocolForResultsArgs.Data["Items"] as string;

                    var allItems = JsonConvert.DeserializeObject<List<Item>>(items);

                    this.ItemsInOrder.ItemsSource = allItems;

                    this.totalAmount = allItems.Select(i => i.UnitPrice * i.Quantity).Sum().ToString();
                    this.TotalAmount.Text = this.totalAmount;
                }
                else
                {
                    var result = new ValueSet();
                    result["Success"] = false;
                    result["Reason"] = "Invalid payload";

                    operation.ReportCompleted(result);
                }
            }
            else
            {
                var result = new ValueSet();
                result["Success"] = false;
                result["Reason"] = "Application not approved";

                operation.ReportCompleted(result);
            }
        }

        private void CancelClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var result = new ValueSet();
            result["Success"] = false;
            result["Reason"] = "Cancelled";

            operation.ReportCompleted(result);
        }

        private void MakePaymentClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var result = new ValueSet();
            result["Success"] = true;
            result["Reason"] = "Payment made";
            result["Transaction"] = this.transaction;
            result["Total"] = this.totalAmount;

            // We're just using a couple of harcdoded payment options in this demo.
            // Obviously, if using actual ones from another source then getting the last 4 digits would need to be done differently.
            var cardEnding = "";

            if (this.CardEnding1234.IsChecked ?? false)
            {
                cardEnding = "1234";
            }
            else if (this.CardEnding9876.IsChecked ?? false)
            {
                cardEnding = "9876";
            }

            result["CardEnding"] = cardEnding;
            result["AuthCode"] = string.Format("{0}{1}", this.transaction, this.totalAmount).GetHashCode();

            operation.ReportCompleted(result);
        }
    }
}
