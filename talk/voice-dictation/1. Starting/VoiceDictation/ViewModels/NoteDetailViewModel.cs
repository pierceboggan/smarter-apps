using System;
using System.Threading.Tasks;

using Acr.UserDialogs;
using Xamarin.Forms;

namespace VoiceDictation
{
	public class NoteDetailViewModel : BaseViewModel
	{
        const string COGNITIVE_SUBSCRIPTION_KEY = "b079a0014389449dbf71c891cb65d4bd";
		string noteTitle;
		string noteText;

		public Note Note { get; set; }
		public string NoteTitle
		{
			get { return noteTitle; }
			set { noteTitle = value; OnPropertyChanged("NoteTitle"); }
		}

		public string NoteText
		{
			get { return noteText; }
			set { noteText = value; OnPropertyChanged("NoteText"); }
		}

		public NoteDetailViewModel(Note note = null)
		{
			if (note != null)
			{
				Note = note;

				NoteTitle = note.Title;
				NoteText = note.Text;
			}
			else
			{
				NoteTitle = "New Note";
			}
		}

		Command saveItemCommand;
		public Command SaveItemCommand
		{
			get { return saveItemCommand ?? (saveItemCommand = new Command(async () => await ExecuteSaveItemCommand())); }
		}

		async Task ExecuteSaveItemCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				if (Note != null)
				{
					Note.Title = NoteTitle;
					Note.Text = NoteText;

					return;
				}

				var note = new Note
				{
					Title = NoteTitle,
					Text = NoteText,
					Created = DateTime.Now
				};

				MessagingCenter.Send<NoteDetailViewModel, Note>(this, "ItemsChanged", note);
			}
			catch (Exception ex)
			{
				UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
    }
}

