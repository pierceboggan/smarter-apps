using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ExpenseIt
{
    public class ExpensesViewModel
    {
        public ObservableCollection<Expense> Expenses { get; set; }

        public ExpensesViewModel()
        {
            Expenses = new ObservableCollection<Expense>();
        }

        Command addExpenseCommand;
        public Command AddExpenseCommand
        {
            get { return addExpenseCommand ?? (addExpenseCommand = new Command(async () => await ExecuteAddExpenseCommandAsync())); }
        }

        async Task ExecuteAddExpenseCommandAsync()
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