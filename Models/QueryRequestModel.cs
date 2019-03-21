using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class QueryRequestModel
    {
        public string Name { get; set; }
        public string Query { get; set; }
        public string GroupId { get; set; }
    }
}
