using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace ExpenseIt
{
    public class Expense
    {
        public double Total { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Photo { get; set; }
    }
}
