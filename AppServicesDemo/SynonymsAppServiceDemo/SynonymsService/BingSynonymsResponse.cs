using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynonymsService
{
  class Metadata
  {
    public string uri { get; set; }
    public string type { get; set; }
  }

  class Result
  {
    public Metadata __metadata { get; set; }
    public string Synonym { get; set; }
  }

  class D
  {
    public List<Result> results { get; set; }
  }

  class BingSynonymsResponse
  {
    public D d { get; set; }
  }
}
