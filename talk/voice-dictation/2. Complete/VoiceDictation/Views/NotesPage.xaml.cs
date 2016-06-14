using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VoiceDictation
{
	public partial class NotesPage : ContentPage
	{
		public NotesPage()
		{
			BindingContext = new NotesViewModel();

			InitializeComponent();
		}
	}
}

