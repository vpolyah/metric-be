using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class ConfigurationModel
    {
        public string DbAddress { get; set; }
        public string DbName { get; set; }
        public string CollectionName { get; set; }
        public string GroupCollectionName { get; set; }
    }
}
