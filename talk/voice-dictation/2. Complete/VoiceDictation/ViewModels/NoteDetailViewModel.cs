using System;
using System.Threading.Tasks;

using Acr.UserDialogs;
using Xamarin.Forms;

namespace VoiceDictation
{
	public class NoteDetailViewModel : BaseViewModel
	{
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
				Acr.UserDialogs.UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		#region Microsoft Cognitive Services
		bool recording;
		public bool Recording
		{
			get { return recording; }
			set { recording = value; OnPropertyChanged("RecordAudioButtonText"); }
		}

		public string RecordAudioButtonText
		{
			get 
			{
				if (Recording)
					return "Stop Recording";
				else
					return "Start Recording";
			}
		}

		Command recordAudioCommand;
		public Command RecordAudioCommand
		{
			get { return recordAudioCommand ?? (recordAudioCommand = new Command(async () => await ExecuteRecordAudioCommandAsync())); }
		}

		async Task ExecuteRecordAudioCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				if (!Recording)
				{
					DependencyService.Get<IAudioRecorderService>().StartRecording();

					Recording = !Recording;
				}
				else
				{
					DependencyService.Get<IAudioRecorderService>().StopRecording();

					Recording = !Recording;

					UserDialogs.Instance.ShowLoading("Converting Speech to Text");
					var speechToText = await BingSpeechApi.SpeechToTextAsync("voice-dictation-app", "b079a0014389449dbf71c891cb65d4bd");
					NoteText = speechToText.Lexical;
					UserDialogs.Instance.HideLoading();
				}
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
		#endregion
	}
}

