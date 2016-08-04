using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace InvoiceIt
{
    public class InvoicesViewModel
    {
        public ObservableCollection<Invoice> Invoices { get; set; }

        public InvoicesViewModel()
        {
            Invoices = new ObservableCollection<Invoice>();
        }

        Command addInvoiceCommand;
        public Command AddInvoiceCommand
        {
            get { return addInvoiceCommand ?? (addInvoiceCommand = new Command(async () => await ExecuteAddInvoiceCommandAsync())); }
        }

        async Task ExecuteAddInvoiceCommandAsync()
        {
            try
            {
                // 1. Add camera logic.
               
                // 2. Add  OCR logic.
               
                // 3. Add to data-bound collection.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}