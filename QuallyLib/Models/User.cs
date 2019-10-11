using System;
using System.ComponentModel.DataAnnotations;

namespace QuallyLib
{
    public enum SortState
    {
        LvlAsc,
        LvlDesc,
    }

    public enum SubscriptionLvl
    {
        [Display(Name = "Уровень 0")]
        Level_No = 0,
        [Display(Name = "Уровень 1")]
        Level_1 = 1,
        [Display(Name = "Уровень 2")]
        Level_2 = 2,
        [Display(Name = "Уровень 3")]
        Level_3 = 3
    }

    public class User
    {
        public int Id { get; set; }
        public string PCID { get; set; }
        public string USBId { get; set; }
        public SubscriptionLvl Lvl { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime SubscriptionExpDate { get; set; }
        public bool Active { get; set; }
        public bool TrialUse { get; set; }
        public bool Trial { get; set; }
        public bool UnlimitedSub { get; set; }
        public bool IsChanged { get; set; }
        public string Hash { get; set; }
    }
}
