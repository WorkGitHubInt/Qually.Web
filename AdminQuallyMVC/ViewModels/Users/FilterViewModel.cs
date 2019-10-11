using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AdminQuallyMVC.ViewModels.Users
{
    public class FilterViewModel
    {
        public SelectList Lvls { get; private set; }
        public int? SelectedLvl { get; private set; }
        public string SelectedId { get; private set; }

        public FilterViewModel(int? lvl, string pcid)
        {
            List<int> ints = new List<int>();
            for (int i = 0; i < Enum.GetValues(typeof(QuallyLib.SubscriptionLvl)).Length; i++)
            {
                ints.Add(i);
            }
            Lvls = new SelectList(ints);
            SelectedLvl = lvl;
            SelectedId = pcid;
        }
    }
}
