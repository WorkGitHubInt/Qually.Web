using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuallyLib;

namespace AdminQuallyMVC.ViewModels
{
    public class ConfirmModel
    {
        public decimal TotalSum { get; set; }
        public decimal DifSum { get; set; }
        public int Days { get; set; }
        public string UserId { get; set; }
        public int SubModelId { get; set; }
        public SubscriptionLvl UserLvl { get; set; }
        public SubscriptionLvl PayLvl { get; set; }
    }
}
