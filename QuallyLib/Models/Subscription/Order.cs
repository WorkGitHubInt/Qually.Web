using System;
using System.ComponentModel.DataAnnotations;

namespace QuallyLib
{
    public enum OrderType
    {
        New = 0,
        Upgrade = 1,
    }

    public class Order
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public SubscriptionLvl Lvl { get; set; }
        public decimal Sum { get; set; }
        public bool Paid { get; set; }
        public string Description { get; set; }
        public OrderType Type { get; set; } 
    }
}
