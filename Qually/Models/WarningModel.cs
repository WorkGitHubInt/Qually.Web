using QuallyLib;

namespace Qually.Models
{
    public class WarningModel
    {
        public decimal TotalSum { get; set; }
        public decimal DiffSum { get; set; }
        public int Days { get; set; }
        public string UserId { get; set; }
        public SubModel SubModel { get; set; }
        public SubscriptionLvl UserLvl { get; set; }
        public SubscriptionLvl PayLvl { get; set; }
    }
}
