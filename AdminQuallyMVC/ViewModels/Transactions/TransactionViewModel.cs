using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuallyLib;

namespace AdminQuallyMVC.ViewModels.Transactions
{
    public class TransactionViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
