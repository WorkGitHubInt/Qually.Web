using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuallyLib
{
    public class SubModel
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public SubscriptionLvl Lvl { get; set; }
        public decimal Sum { get; set; }
        public string Title { get; set; }
    }
}
