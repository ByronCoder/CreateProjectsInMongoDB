using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateProjectsInMongoDB.Models
{
   public class Project
    {
        
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string sourceLink { get; set; }
        public string demoLink { get; set; }
    }
}
