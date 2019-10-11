namespace AdminQuallyMVC.ViewModels.Orders
{
    public class FilterViewModel
    {
        public string SelectedId { get; private set; }

        public FilterViewModel(string pcid)
        {
            SelectedId = pcid;
        }
    }
}
