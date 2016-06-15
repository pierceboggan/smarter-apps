using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MicrosoftStore
{
	public class InventoryDetailViewModel : BaseViewModel
	{
		Inventory inventory;
		public Inventory Inventory
		{
			get { return inventory; }
			set { inventory = value; OnPropertyChanged("Inventory"); }
		}

		public InventoryDetailViewModel(Inventory selectedInventory)
		{
			Title = "Product Detail";

			Inventory = selectedInventory;
		}

		Command addToCartCommand;
		public Command AddToCartCommand
		{
			get { return addToCartCommand ?? (addToCartCommand = new Command(async () => await ExecuteAddToCartCommandAsync())); }
		}

		async Task ExecuteAddToCartCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				var app = (MicrosoftStore.App) Application.Current;
				app.Cart.Add(Inventory);

				Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Added to Cart");
			}
			catch (Exception ex)
			{
				Acr.UserDialogs.UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}

