using Newtonsoft.Json;
using NAudio.Wave;


namespace AudioPlayer
{
    public class AudioPlayerController
    {
        public string AccessFileNames()
        {
            DirectoryInfo di = new DirectoryInfo(@"/Users/chase/Documents/twitch/audio/");
            FileInfo[] files = di.GetFiles("*.mp3");

            List<string> fileNames = new List<string>();
            foreach (FileInfo file in files)
            {
                fileNames.Add(file.Name);
            }
            fileNames.Sort((x, y) => string.Compare(x, y));
            Console.WriteLine("AUDIO FILES -> " + fileNames.Count);

            string jsonFileNames = JsonConvert.SerializeObject(fileNames);
            return jsonFileNames;
        }

        public string TestPlay()
        {
            using (var audioFile = new AudioFileReader(@"/Users/chase/Documents/twitch/audio/bonk.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }

            return AccessFileNames();
        }

        public void Play(string fileName)
        {
            string filePrefix = @"/Users/chase/Documents/twitch/audio/";
            // Console.WriteLine(filePrefix + fileName);
            using (var audioFile = new AudioFileReader(filePrefix + fileName))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

    }
}