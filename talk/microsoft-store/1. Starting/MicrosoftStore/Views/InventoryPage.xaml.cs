using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MicrosoftStore
{
	public partial class InventoryPage : ContentPage
	{
		public InventoryPage()
		{
			InitializeComponent();

			BindingContext = new InventoryViewModel();
		}
	}
}

