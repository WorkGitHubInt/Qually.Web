using System.Collections.Generic;
using QuallyLib;

namespace AdminQuallyMVC.ViewModels.Users
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
