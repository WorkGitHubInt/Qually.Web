using System.ComponentModel.DataAnnotations;

namespace QuallyLib
{
    public class Key
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
        public bool Activated { get; set; }
        public SubscriptionLvl Lvl { get; set; }
        public int Duration { get; set; }
    }
}
