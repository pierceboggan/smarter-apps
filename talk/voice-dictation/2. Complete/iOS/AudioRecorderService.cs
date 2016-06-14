using System;

using AVFoundation;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(VoiceDictation.iOS.AudioRecorderService))]
namespace VoiceDictation.iOS
{
	public class AudioRecorderService : IAudioRecorderService
	{
		AVAudioRecorder recorder;
		NSError error;
		NSUrl url;
		NSDictionary settings;

		public void StartRecording()
		{
			if (recorder == null)
				InitializeRecorder();

			recorder.Record();
		}

		public void StopRecording()
		{
			if (recorder == null)
				throw new Exception("You must first start recording.");

			recorder.Stop();
		}

		void InitializeRecorder()
		{
			var audioSession = AVAudioSession.SharedInstance();
			var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
			if (err != null)
			{
				Console.WriteLine("audioSession: {0}", err);
				return;
			}
			err = audioSession.SetActive(true);
			if (err != null)
			{
				Console.WriteLine("audioSession: {0}", err);
				return;
			}

			var localStorage = PCLStorage.FileSystem.Current.LocalStorage.Path;
			string audioFilePath = localStorage + "/SmartCoffee.wav";

			Console.WriteLine("Audio File Path: " + audioFilePath);

			url = NSUrl.FromFilename(audioFilePath);

			//set up the NSObject Array of values that will be combined with the keys to make the NSDictionary
			var values = new NSObject[]
			{
				NSNumber.FromFloat (44100.0f), //Sample Rate
  				NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.LinearPCM), //AVFormat
   				NSNumber.FromInt32 (2), //Channels
    			NSNumber.FromInt32 (16), //PCMBitDepth
    			NSNumber.FromBoolean (false), //IsBigEndianKey
    			NSNumber.FromBoolean (false) //IsFloatKey
			};

			//Set up the NSObject Array of keys that will be combined with the values to make the NSDictionary
			var keys = new NSObject[]
			{
				AVAudioSettings.AVSampleRateKey,
				AVAudioSettings.AVFormatIDKey,
				AVAudioSettings.AVNumberOfChannelsKey,
				AVAudioSettings.AVLinearPCMBitDepthKey,
				AVAudioSettings.AVLinearPCMIsBigEndianKey,
				AVAudioSettings.AVLinearPCMIsFloatKey
			};

			//Set Settings with the Values and Keys to create the NSDictionary
			settings = NSDictionary.FromObjectsAndKeys(values, keys);

			//Set recorder parameters
			recorder = AVAudioRecorder.Create(url, new AudioSettings(settings), out error);
			//Set Recorder to Prepare To Record
			recorder.PrepareToRecord();
		}
	}
}