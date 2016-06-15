using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VoiceDictation
{
	public partial class NoteDetailPage : ContentPage
	{
		public NoteDetailPage(Note selectedNote = null)
		{
			InitializeComponent();

			BindingContext = new NoteDetailViewModel(selectedNote);
		}
	}
}

