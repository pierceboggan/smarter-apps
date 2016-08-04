using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InvoiceIt
{
    public partial class InvoicesPage : ContentPage
    {
        public InvoicesPage()
        {
            InitializeComponent();

            BindingContext = new InvoicesViewModel();
        }
    }
}
