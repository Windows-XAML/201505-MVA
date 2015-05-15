using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SynonymsService
{
    class JSONHelper
    {
        public static T DeserializeObject<T>(string objString)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static string SerializeObject<T>(T obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer =

            new DataContractJsonSerializer(typeof(T));
            jsonSer.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            var json = sr.ReadToEnd();
            return json;
        }
    }
}
