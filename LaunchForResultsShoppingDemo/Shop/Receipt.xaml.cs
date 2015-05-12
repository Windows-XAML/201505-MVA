using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Shop
{
    public sealed partial class Receipt : Page
    {
        public Receipt()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = e.Parameter as ValueSet;

            var receipt = new List<string>();

            if (result.Keys.Count >= 2
                && result.ContainsKey("Success")
                && result.ContainsKey("Reason"))
            {
                if (((bool)result["Success"]))
                {
                    // Check that we have all the info we expect and that it corresponds to the 
                    if (result.ContainsKey("Transaction")
                        && result.ContainsKey("Total")
                        && result.ContainsKey("CardEnding")
                        && result.ContainsKey("AuthCode"))
                    {
                        receipt.Add("Payment Successful");
                        receipt.Add(string.Format("Amount: {0}", result["Total"]));
                        receipt.Add(string.Format("Paid with card ending: {0}", result["CardEnding"]));
                        receipt.Add(string.Format("Transaction Id: {0}", result["Transaction"]));
                        receipt.Add(string.Format("Auth Code: {0}", result["AuthCode"]));
                    }
                    else
                    {
                        receipt.Add("Payment exception");
                        receipt.Add("Unable to verify transaction details");
                    }
                }
                else
                {
                    receipt.Add("Payment failed");
                    receipt.Add(result["Reason"].ToString());
                }
            }
            else
            {
                receipt.Add("Payment exception");
                receipt.Add("Check out error");
            }

            this.ReceiptDetails.ItemsSource = receipt;
        }

        private void HomeClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
