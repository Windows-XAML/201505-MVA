using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template10.Services.FileService
{
    static public class FileHelper
    {
        /// <summary>Returns if a file is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Boolean: true if found, false if not found</returns>
        static public async Task<bool> FileExistsAsync(string key, StorageStrategies location = StorageStrategies.Local)
        {
            return (await GetIfFileExistsAsync(key, location)) != null;
        }

        static public async Task<bool> FileExistsAsync(string key, Windows.Storage.StorageFolder folder)
        {
            return (await GetIfFileExistsAsync(key, folder)) != null;
        }

        /// <summary>Deletes a file in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        static public async Task<bool> DeleteFileAsync(string key, StorageStrategies location = StorageStrategies.Local)
        {
            var _File = await GetIfFileExistsAsync(key, location);
            if (_File != null)
                await _File.DeleteAsync();
            return !(await FileExistsAsync(key, location));
        }

        /// <summary>Reads and deserializes a file into specified type T</summary>
        /// <typeparam name="T">Specified type into which to deserialize file content</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Specified type T</returns>
        static public async Task<T> ReadFileAsync<T>(string key, StorageStrategies location = StorageStrategies.Local)
        {
            try
            {
                // fetch file
                var _File = await GetIfFileExistsAsync(key, location);
                if (_File == null)
                    return default(T);
                // read content
                var _String = await Windows.Storage.FileIO.ReadTextAsync(_File);
                // convert to obj
                var _Result = Deserialize<T>(_String);
                return _Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error Reading File: {0}", ex.Message));
                throw ex;
            }
        }

        /// <summary>Serializes an object and write to file in specified storage strategy</summary>
        /// <typeparam name="T">Specified type of object to serialize</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="value">Instance of object to be serialized and written</param>
        /// <param name="location">Location storage strategy</param>
        static public async Task<bool> WriteFileAsync<T>(string key, T value, StorageStrategies location = StorageStrategies.Local)
        {
            try
            {
                // create file
                var _File = await CreateFileAsync(key, location, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                // convert to string
                var _String = Serialize(value);
                // save string to file
                await Windows.Storage.FileIO.WriteTextAsync(_File, _String);
                // result
                return await FileExistsAsync(key, location);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error Writing File: {0}", ex.Message));
                throw ex;
            }
        }

        static private async Task<Windows.Storage.StorageFile> CreateFileAsync(string key, StorageStrategies location = StorageStrategies.Local,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.OpenIfExists)
        {
            switch (location)
            {
                case StorageStrategies.Local:
                    return await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(key, option);
                case StorageStrategies.Roaming:
                    return await Windows.Storage.ApplicationData.Current.RoamingFolder.CreateFileAsync(key, option);
                case StorageStrategies.Temporary:
                    return await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(key, option);
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        static private async Task<Windows.Storage.StorageFile> GetIfFileExistsAsync(string key, Windows.Storage.StorageFolder folder,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.FailIfExists)
        {
            Windows.Storage.StorageFile retval;
            try
            {
                retval = await folder.GetFileAsync(key);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }
            return retval;
        }

        /// <summary>Returns a file if it is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>StorageFile</returns>
        static private async Task<Windows.Storage.StorageFile> GetIfFileExistsAsync(string key,
            StorageStrategies location = StorageStrategies.Local,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.FailIfExists)
        {
            Windows.Storage.StorageFile retval;
            try
            {
                switch (location)
                {
                    case StorageStrategies.Local:
                        retval = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.Roaming:
                        retval = await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.Temporary:
                        retval = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFileAsync(key);
                        break;
                    default:
                        throw new NotSupportedException(location.ToString());
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }

            return retval;
        }

        static private string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item,
                Formatting.None, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
        }

        static private T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public enum StorageStrategies { Local, Roaming, Temporary }
    }
}
