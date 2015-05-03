using ActionCenterDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ActionCenterDemo.Repositories
{
    class MessageItemRepository
    {
        private List<MessageItem> _messageItems;

        private MessageItemRepository()
        {
            LoadData();
        }

        public void LoadData()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("data"))
            {
                ApplicationData.Current.LocalSettings.Values["data"] =
                    Newtonsoft.Json.JsonConvert.SerializeObject(new List<MessageItem>());
            }
            var data = ApplicationData.Current.LocalSettings.Values["data"] as string;
            _messageItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageItem>>(data);
        }

        private static MessageItemRepository _instance;

        public static MessageItemRepository GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MessageItemRepository();
            }
            return _instance;
        }

        public IEnumerable<Models.MessageItem> GetAllMessageItems()
        {
            return _messageItems;
        }

        public async Task AddAsync(MessageItem messageItem)
        {
            _messageItems.Add(messageItem);
            await PersistAsync();
        }
        public async Task RemoveAsync(string iD)
        {
            var target = (from m in _messageItems
                          where m.ID == iD
                          select m).FirstOrDefault();
            if (target != null) _messageItems.Remove(target);
            await PersistAsync();
        }

        public async Task UpdateAsync(MessageItem messageItem)
        {
            var target = (from m in _messageItems
                          where m.ID == messageItem.ID
                          select m).FirstOrDefault();
            if (target != null)
            {
                int idx = _messageItems.IndexOf(target);
                _messageItems.Remove(target);
                _messageItems.Insert(idx, messageItem);
            }
            await PersistAsync();
        }

        public IEnumerable<MessageItem> Find(Func<MessageItem, bool> predicate)
        {
            return _messageItems.Where(predicate);
        }

        private async Task PersistAsync()
        {
            ApplicationData.Current.LocalSettings.Values["data"] = Newtonsoft.Json.JsonConvert.SerializeObject(_messageItems);
        }

        public void SaveChanges()
        {
            ApplicationData.Current.LocalSettings.Values["data"] = Newtonsoft.Json.JsonConvert.SerializeObject(_messageItems);
        }
    }
}
