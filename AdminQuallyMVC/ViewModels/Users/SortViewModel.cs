using QuallyLib;

namespace AdminQuallyMVC.ViewModels.Users
{
    public class SortViewModel
    {
        public SortState LvlSort { get; set; } // значение для сортировки по имени
        public SortState Current { get; set; }     // значение свойства, выбранного для сортировки

        public SortViewModel(SortState sortOrder)
        {
            LvlSort = sortOrder == SortState.LvlAsc ? SortState.LvlDesc : SortState.LvlAsc;
            Current = sortOrder;
        }
    }
}
