using System.Collections.Generic;
using QuallyLib;

namespace AdminQuallyMVC.ViewModels.Orders
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
