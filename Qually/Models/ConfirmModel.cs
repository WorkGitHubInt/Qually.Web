using Microsoft.AspNetCore.Mvc.Rendering;

namespace Qually.Models
{
    public class ConfirmModel
    {
        public SelectList SubModelList { get; set; }
        public int? SelectedValue { get; set; }
        public string UserId { get; set; }
    }
}
