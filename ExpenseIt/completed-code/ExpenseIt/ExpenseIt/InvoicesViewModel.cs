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
                using (var stream = photo.GetStream())
                    text = await client.RecognizeTextAsync(stream);

                double total = 0.0;
                foreach (var region in text.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            if (word.Text.Contains("$"))
                            {
                                try
                                {
                                    var number = Double.Parse(word.Text.Replace("$", ""));

                                    total = (number > total) ? number : total;
                                }
                                catch { }
                            }
                        }
                    }
                }

                // 3. Add to data-bound collection.
                Invoices.Add(new Invoice
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