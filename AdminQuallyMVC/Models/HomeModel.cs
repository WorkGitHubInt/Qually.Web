using System.Collections.Generic;

namespace AdminQuallyMVC.Models
{
    public class HomeModel
    {
        public string Admin { get; set; }
        public int Orders { get; set; }
        public int Transactions { get; set; }
        public int Users { get; set; }
        public int ActiveUsers { get; set; }
        public int UnlimitedUsers { get; set; }
        public int TrialUsers { get; set; }
        public int Keys { get; set; }
        public int Messages { get; set; }
        public decimal Income { get; set; }
        public IEnumerable<QuallyLib.Program> Programs { get; set; }
    }
}
