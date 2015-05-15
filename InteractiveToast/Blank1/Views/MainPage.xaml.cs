using System;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace Blank1.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            switch (access)
            {
                case BackgroundAccessStatus.Unspecified:
                case BackgroundAccessStatus.Denied:
                    return;
            }

            var task = new BackgroundTaskBuilder
            {
                Name = nameof(MyBackground.MyTask),
                TaskEntryPoint = typeof(MyBackground.MyTask).ToString(),
            };
            task.SetTrigger(new ToastNotificationActionTrigger());
            task.Register();

            MyButton.IsEnabled = true;
        }

        private void ShowInteractiveToast(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var tag = "Jerry";
            var group = "Nixon";
            var title = "New TODO Item";
            var content = "Lorem ipsum dolor sit amet.";

            var element =
                new XElement("toast",
                    new XAttribute("activationType", "background"),
                    new XElement("visual",
                        new XElement("binding",
                            new XAttribute("template", "ToastGeneric"),
                            new XElement("text", title),
                            new XElement("text", content)
                            )
                        ),
                    new XElement("actions",
                        new XElement("input",
                            new XAttribute("id", "Title"),
                            new XAttribute("type", "text"),
                            new XAttribute("title", "Item title:")
                            ),
                        new XElement("action",
                            new XAttribute("activationType", "background"),
                            new XAttribute("arguments", "TodoItem"),
                            new XAttribute("content", "Submit")
                            )
                        )
                    );

            var xml = element.ToString();
            var document = new XmlDocument();
            document.LoadXml(xml);

            ToastNotificationManager.CreateToastNotifier()
                .Show(new ToastNotification(document)
                {
                    Tag = tag,
                    Group = group,
                    ExpirationTime = DateTime.Now.AddMinutes(5),
                });
        }

        private void ShowProtocolToast(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var tag = "Jerry";
            var group = "Nixon";
            var title = "New TODO Item";
            var content = "Lorem ipsum dolor sit amet.";

            var element =
                new XElement("toast",
                    new XAttribute("activationType", "background"),
                    new XElement("visual",
                        new XElement("binding",
                            new XAttribute("template", "ToastGeneric"),
                            new XElement("text", title),
                            new XElement("text", content)
                            )
                        ),
                    new XElement("actions",
                        new XElement("action",
                            new XAttribute("activationType", "protocol"),
                            new XAttribute("arguments", "bingmaps:?q=microsoft"),
                            new XAttribute("content", "Open maps")
                            )
                        )
                    );

            var xml = element.ToString();
            var document = new XmlDocument();
            document.LoadXml(xml);

            ToastNotificationManager.CreateToastNotifier()
                .Show(new ToastNotification(document)
                {
                    Tag = tag,
                    Group = group,
                    ExpirationTime = DateTime.Now.AddMinutes(5),
                });
        }
    }
}