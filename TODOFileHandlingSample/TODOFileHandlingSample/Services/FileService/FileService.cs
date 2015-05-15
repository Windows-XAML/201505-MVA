using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TODOFileHandlingSample.Services.FileService
{
    class FileService
    {
        FileHelper _helper = new FileHelper();

        public async Task<List<T>> ReadAsync<T>(string key)
        {
            try { return await _helper.ReadFileAsync<List<T>>(key, FileHelper.StorageStrategies.Local); }
            catch { return new List<T>(); }
        }

        public async Task WriteAsync<T>(string key, List<T> items)
        {
            await _helper.WriteFileAsync(key, items, FileHelper.StorageStrategies.Local);
        }
    }
}
