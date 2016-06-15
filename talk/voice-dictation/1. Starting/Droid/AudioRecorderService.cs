using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.IO;


// Ported from: http://www.edumobile.org/android/audio-recording-in-wav-format-in-android-programming/
[assembly: Xamarin.Forms.Dependency(typeof(VoiceDictation.Droid.AudioRecorderService))]
namespace VoiceDictation.Droid
{
	public class AudioRecorderService : IAudioRecorderService
	{
		int RECORDER_BPP = 16;
		string AUDIO_RECORDER_FILE_EXT_WAV = ".wav";
		string AUDIO_RECORDER_FOLDER = "AudioRecorder";
		string AUDIO_RECORDER_TEMP_FILE = "record_temp.raw";
		int RECORDER_SAMPLERATE = 44100;
		ChannelIn RECORDER_CHANNELS = ChannelIn.Stereo;
		Encoding RECORDER_AUDIO_ENCODING = Encoding.Pcm16bit;

		AudioRecord recorder;
		int bufferSize;
		bool isRecording;
		CancellationTokenSource token;

		public void Init()
		{
			bufferSize = AudioRecord.GetMinBufferSize(8000, ChannelIn.Mono, Encoding.Pcm16bit);
		}

		public void StartRecording()
		{
			Init();

			var context = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
			var audioManager = (AudioManager)context.GetSystemService(Context.AudioService);
			RECORDER_SAMPLERATE = Int32.Parse(audioManager.GetProperty(AudioManager.PropertyOutputSampleRate));

			if (recorder != null)
				recorder.Release();
			// Calculate buffer size
			//bufferSize = AudioRecord.GetMinBufferSize(RECORDER_SAMPLERATE, ChannelIn.Mono, Encoding.Pcm16bit);
			recorder = new AudioRecord(AudioSource.Mic, RECORDER_SAMPLERATE, RECORDER_CHANNELS, RECORDER_AUDIO_ENCODING, bufferSize);
			recorder.StartRecording();
			isRecording = true;

			var token = new CancellationTokenSource();
			Task.Run(() => WriteAudioDataToFile(), token.Token);
		}

		void WriteAudioDataToFile()
		{
			byte[] data = new byte[bufferSize];
			var filename = GetTempFilename();
			FileOutputStream os = null;

			System.Diagnostics.Debug.WriteLine(filename);

			try
			{
				os = new FileOutputStream(filename);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			int read = 0;
			if (os != null)
			{
				while (isRecording)
				{
					read = recorder.Read(data, 0, bufferSize);

					try
					{
						os.Write(data);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.Message);
					}
				}

				try
				{
					os.Close();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
			}
		}

		public void StopRecording()
		{
			if (recorder != null)
			{
				isRecording = false;

				recorder.Stop();
				// token.Cancel();

				recorder.Release();
				recorder = null;
			}

			CopyWaveFile(GetTempFilename(), GetFilename());
		}

		string GetFilename()
		{
			var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			return System.IO.Path.Combine(path, "SmartCoffee.wav");
		}

		string GetTempFilename()
		{
			var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			return System.IO.Path.Combine(path, "temp.raw");
		}

		void DeleteTempFile()
		{
			var file = new Java.IO.File(GetTempFilename());
			file.Delete();
		}

		void CopyWaveFile(string tempFile, string permanentFile)
		{
			FileInputStream input = null;
			FileOutputStream output = null;
			long totalAudioLen = 0;
			long totalDataLen = totalAudioLen + 36;
			long longSampleRate = RECORDER_SAMPLERATE;
			int channels = 2;
			long byteRate = RECORDER_BPP * RECORDER_SAMPLERATE * channels / 8;

			byte[] data = new byte[bufferSize];

			try
			{
				input = new FileInputStream(tempFile);
				output = new FileOutputStream(permanentFile);
				totalAudioLen = input.Channel.Size();
				totalDataLen = totalAudioLen + 36;

				System.Diagnostics.Debug.WriteLine($"File Size: {totalDataLen}");

				WriteWaveFileHeader(output, totalAudioLen, totalDataLen, longSampleRate, channels, byteRate);

				while (input.Read(data) != -1)
				{
					output.Write(data);
				}

				input.Close();
				output.Close();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		void WriteWaveFileHeader(FileOutputStream output, long totalAudioLen, long totalDataLen, long longSampleRate,
								 int channels, long byteRate)
		{
			byte[] header = new byte[44];

			header[0] = Convert.ToByte('R'); // RIFF/WAVE header
			header[1] = Convert.ToByte('I');//  (byte)'I';
			header[2] = Convert.ToByte('F');
			header[3] = Convert.ToByte('F');
			header[4] = (byte)(totalDataLen & 0xff);
			header[5] = (byte)((totalDataLen >> 8) & 0xff);
			header[6] = (byte)((totalDataLen >> 16) & 0xff);
			header[7] = (byte)((totalDataLen >> 24) & 0xff);
			header[8] = Convert.ToByte('W');
			header[9] = Convert.ToByte('A');
			header[10] = Convert.ToByte('V');
			header[11] = Convert.ToByte('E');
			header[12] = Convert.ToByte('f');// 'fmt ' chunk
			header[13] = Convert.ToByte('m');
			header[14] = Convert.ToByte('t');
			header[15] = (byte)' ';
			header[16] = 16; // 4 bytes: size of 'fmt ' chunk
			header[17] = 0;
			header[18] = 0;
			header[19] = 0;
			header[20] = 1; // format = 1
			header[21] = 0;
			header[22] = Convert.ToByte(channels);
			header[23] = 0;
			header[24] = (byte)(longSampleRate & 0xff);
			header[25] = (byte)((longSampleRate >> 8) & 0xff);
			header[26] = (byte)((longSampleRate >> 16) & 0xff);
			header[27] = (byte)((longSampleRate >> 24) & 0xff);
			header[28] = (byte)(byteRate & 0xff);
			header[29] = (byte)((byteRate >> 8) & 0xff);
			header[30] = (byte)((byteRate >> 16) & 0xff);
			header[31] = (byte)((byteRate >> 24) & 0xff);
			header[32] = (byte)(2 * 16 / 8); // block align
			header[33] = 0;
			header[34] = Convert.ToByte(RECORDER_BPP); // bits per sample
			header[35] = 0;
			header[36] = Convert.ToByte('d');
			header[37] = Convert.ToByte('a');
			header[38] = Convert.ToByte('t');
			header[39] = Convert.ToByte('a');
			header[40] = (byte)(totalAudioLen & 0xff);
			header[41] = (byte)((totalAudioLen >> 8) & 0xff);
			header[42] = (byte)((totalAudioLen >> 16) & 0xff);
			header[43] = (byte)((totalAudioLen >> 24) & 0xff);

			output.Write(header, 0, 44);
		}
	}
}