using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace InvoiceIt
{
    public class Invoice
    {
        public double Total { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Photo { get; set; }
    }
}
