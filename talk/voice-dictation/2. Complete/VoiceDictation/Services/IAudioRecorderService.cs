using System;
namespace VoiceDictation
{
	public interface IAudioRecorderService
	{
		void StartRecording();
		void StopRecording();
	}
}

