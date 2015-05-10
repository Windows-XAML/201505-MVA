using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Security.Credentials;

namespace SynonymsService
{
  class SynonymApi
  {
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
      //List<string> synonyms = new List<string>();

      //Uri uri = new Uri(string.Format(uriFormatString, Uri.EscapeDataString(term)));
      //ODataClientSettings odataSettings = new ODataClientSettings(
      //    "https://api.datamarket.azure.com/Bing/Synonyms/v1/",
      //    new NetworkCredential(apiKey, apiKey));
      //var client = new ODataClient(odataSettings);

      //string command = string.Format("?Query=%27{0}%27", term);

      //var synonymsResponse = await client
      //  .Unbound()
      //  .Function("GetSynonyms")
      //  .Set(new { Query = term})
      //  .ExecuteAsEnumerableAsync();


      //foreach (var synonym in synonymsResponse)
      //{
      //  synonyms.Add((string)synonym);
      //}

      HttpBaseProtocolFilter handler = new HttpBaseProtocolFilter();
      PasswordCredential credentials = new PasswordCredential("BingSynonymsAPIKey", apiKey, apiKey);
      handler.ServerCredential = credentials;
      handler.ProxyCredential = credentials;

      HttpClient client = new HttpClient(handler);
      client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

      Uri uri = new Uri(string.Format(uriFormatString, Uri.EscapeDataString(term)));
      string responseJson = await client.GetStringAsync(uri);

      var response = Newtonsoft.Json.JsonConvert.DeserializeObject<BingSynonymsResponse>(responseJson);

      var synonyms = (from r in response.d.results
                      select r.Synonym).ToList();

      return (synonyms);
    }

    static readonly string uriFormatString =
      "https://api.datamarket.azure.com/Bing/Synonyms/v1/GetSynonyms?Query=%27{0}%27";

    string apiKey;
  }

}
