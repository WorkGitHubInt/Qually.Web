using System.Collections.Generic;
using QuallyLib;

namespace AdminQuallyMVC.Models
{
    public class UpdateSafe
    {
        public int Id { get; set; }
        public QuallyLib.Program Program { get; set; }
        public string Version { get; set; }
    }

    public class UpdateModel
    {
        public IEnumerable<UpdateSafe> Updates { get; set; }
        public IEnumerable<QuallyLib.Program> Programs { get; set; }
    }
}
