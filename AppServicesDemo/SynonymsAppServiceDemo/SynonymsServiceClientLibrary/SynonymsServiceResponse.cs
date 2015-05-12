using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace SynonymsServiceClientLibrary
{
    public class SynonymsServiceResponse
    {
        public AppServiceResponseStatus Status { get; set; }
        public List<string> Synonyms { get; set; }
    }
}
