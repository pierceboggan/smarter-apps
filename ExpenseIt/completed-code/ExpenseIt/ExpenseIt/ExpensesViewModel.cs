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
                await CrossMedia.Current.Initialize();

                MediaFile photo;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Receipts",
                        Name="Receipt"
                    });
                }
                else
                {
                    photo = await CrossMedia.Current.PickPhotoAsync();
                }

                // 2. Add  OCR logic.
                OcrResults text;

                var client = new VisionServiceClient("ebccaf8faed7407eb5b2108193d7b13a");
                using (var photoStream = photo.GetStream())
                {
                    text = await client.RecognizeTextAsync(photoStream);
                }

                double total = 0.0;
                foreach(var region in text.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            if (word.Text.Contains("$"))
                            {
                                var number = Double.Parse(word.Text.Replace("$", ""));

                                total = (number > total) ? number : total;
                            }
                        }
                    }
                }

                // 3. Add to data-bound collection.
                Expenses.Add(new Expense
                {
                    Total = total,
                    Photo = photo.Path,
                    TimeStamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}