using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SynonymsService
{
    [DataContract]
    class Metadata
    {
        [DataMember(Name ="uri")]
        public string uri { get; set; }

        [DataMember(Name = "type")]
        public string type { get; set; }
    }

    [DataContract]
    class Result
    {
        [DataMember(Name = "__metadata")]
        public Metadata __metadata { get; set; }
        [DataMember(Name = "Synonym")]
        public string Synonym { get; set; }
    }

    [DataContract]
    class D
    {
        [DataMember(Name = "results")]
        public List<Result> results { get; set; }
    }

    [DataContract]
    class BingSynonymsResponse
    {
        [DataMember(Name = "d")]
        public D d { get; set; }
    }
}
