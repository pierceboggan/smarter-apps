using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VoiceDictation
{
	public class Note : INotifyPropertyChanged
	{
		string title;
		string text;

		public string Title
		{
			get { return title; }
			set { title = value; OnPropertyChanged(); }
		}

		public string Text
		{
			get { return text; }
			set { text = value; OnPropertyChanged(); }
		}

		public DateTime Created { get; set; }

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var changed = PropertyChanged;
			if (changed == null)
				return;

			changed(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}

