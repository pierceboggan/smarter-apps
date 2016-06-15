using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MicrosoftStore
{
	public partial class InventoryDetailPage : ContentPage
	{
		public InventoryDetailPage(Inventory selectedInventory)
		{
			BindingContext = new InventoryDetailViewModel(selectedInventory);

			InitializeComponent();
		}
	}
}

