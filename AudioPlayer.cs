using Newtonsoft.Json;
using NAudio.Wave;

namespace AudioPlayer
{
    public class AudioPlayerController
    {
        static Random rnd = new Random();
        private WaveOutEvent? outputDevice;

        public string AccessFileNames()
        {
            // DirectoryInfo di = new DirectoryInfo(@"/Users/chase/Documents/twitch/audio/");
            // FileInfo[] files = di.GetFiles("*.mp3");

            var files = Directory.EnumerateFiles(@"/Users/chase/Documents/twitch/audio/", "*.*", SearchOption.AllDirectories)
                .Where(s => s.ToLower().EndsWith(".mp3") || s.ToLower().EndsWith(".wav"));

            List<string> fileNames = new List<string>();
            foreach (string file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }
            fileNames.Sort((x, y) => string.Compare(x, y));
            Console.WriteLine("AUDIO FILES -> " + fileNames.Count);

            string jsonFileNames = JsonConvert.SerializeObject(fileNames);
            return jsonFileNames;
        }

        public string Play(string fileName)
        {
            string filePrefix = @"/Users/chase/Documents/twitch/audio/";
            // Console.WriteLine(filePrefix + fileName);
            using (var audioFile = new AudioFileReader(filePrefix + fileName))
            using (outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
            return "200";
        }

        public string StopAudio()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
            }
            return "200";
        }

        public string PlayRandomAudio()
        {
            string jsonAudioNames = AccessFileNames();
            List<string> allAudio = new List<string>();
            if (jsonAudioNames != " ")
            {
                allAudio = JsonConvert.DeserializeObject<List<string>>(jsonAudioNames);
            }
            int r;
            if (allAudio != null)
            {
                r = rnd.Next(allAudio.Count);
                Play(allAudio[r]);
            }

            return "200";
        }

    }
}