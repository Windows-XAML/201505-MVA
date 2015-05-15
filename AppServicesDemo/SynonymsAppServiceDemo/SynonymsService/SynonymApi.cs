using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Security.Credentials;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace SynonymsService
{
    class SynonymApi
    {
        static readonly string uriFormatString =
          "https://api.datamarket.azure.com/Bing/Synonyms/v1/GetSynonyms?Query=%27{0}%27";

        string apiKey;

        public SynonymApi(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("Sorry, I think they make you have a key for this API");
            }
            this.apiKey = apiKey;
        }
        public async Task<IEnumerable<string>> GetSynonymsAsync(string term)
        {
            // Call synonyms OData service on Azure Marketplace at
            //  "https://api.datamarket.azure.com/Bing/Synonyms/v1/",

            HttpBaseProtocolFilter handler = new HttpBaseProtocolFilter();
            PasswordCredential credentials = new PasswordCredential("BingSynonymsAPIKey", apiKey, apiKey);
            handler.ServerCredential = credentials;
            handler.ProxyCredential = credentials;

            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

            Uri uri = new Uri(string.Format(uriFormatString, Uri.EscapeDataString(term)));
            string responseJson = await client.GetStringAsync(uri);

            var response = JSONHelper.DeserializeObject<BingSynonymsResponse>(responseJson);

            var synonyms = (from r in response.d.results
                            select r.Synonym).ToList();

            return (synonyms);
        }

    }
}
