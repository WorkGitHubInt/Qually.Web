using QuallyLib;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AdminQuallyMVC.ViewModels
{
    public class SubViewModel
    {
        public List<SubModel> SubModels { get; set; }
        public List<SelectListItem> Models { get; set; } = new List<SelectListItem>();
    }
}
