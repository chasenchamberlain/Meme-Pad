using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System;
// using System.Timers;
// using System.Collections.Generic;
// using Timer = System.Timers.Timer;

namespace OBSControl
{
    public class OBSController
    {
        private readonly OBSWebsocket obs;
        private static OBSController _instance;
        private bool isConnected = true;
        // private Timer timer;

        public OBSController()
        {
            _instance = this;
            isConnected = false;
            obs = new OBSWebsocket();
            obs.Connected += OBS_Connected;
            obs.Disconnected += OBS_Disconnected;

            ConnectDisconnect();
            Console.WriteLine("OBS is created? + {0}", isConnected);

            foreach (string scene in this.GetSceneKids())
            {
                Console.WriteLine(scene);
            }
            Console.WriteLine("VIDEO FILES -> " + this.GetSceneKids().Count);

            obs.RestartMedia("Its Working");
        }

        // public void InitTimer()
        // {
        //     Console.WriteLine("In InitTimer");
        //     timer = new Timer();
        //     timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
        //     timer.Interval = 10000;
        // }

        // public void Timer_Tick(Object source, ElapsedEventArgs e)
        // {
        //     if (!obs.IsConnected)
        //     {
        //         ConnectDisconnect();
        //     }
        // }

        public List<string> GetScenes()
        {
            List<string> scenesString = new List<string>();
            if (!isConnected) return scenesString;

            List<OBSScene> scenes = obs.ListScenes();

            foreach (OBSScene scene in scenes)
            {
                Console.WriteLine(scene.Name);
                scenesString.Add(scene.Name);
            }
            scenesString.Sort((x, y) => string.Compare(x, y));
            return scenesString;
        }

        public List<string> GetSources()
        {
            List<string> sourceString = new List<string>();
            if (!isConnected) return sourceString;

            List<OBSScene> scenes = obs.ListScenes();
            foreach (OBSScene scene in scenes)
            {
                foreach (SceneItem source in scene.Items)
                {
                    sourceString.Add(source.SourceName);
                    if (source.GroupChildren != null)
                    {
                        foreach (SceneItem subfield in source.GroupChildren)
                        {
                            sourceString.Add(subfield.SourceName);
                        }
                    }
                }
            }
            List<SourceInfo> sources = obs.GetSourcesList();
            foreach (SourceInfo source in sources)
            {
                sourceString.Add(source.Name);
            }

            sourceString.Sort((x, y) => string.Compare(x, y));
            return sourceString.Distinct().ToList();
        }

        public List<string> GetSceneKids()
        {
            List<string> sourceString = new List<string>();
            if (!isConnected) return sourceString;

            // List<MediaSource> mediaSources = obs.GetMediaSourcesList();

            foreach (MediaSource ms in obs.GetMediaSourcesList())
            {
                sourceString.Add(ms.SourceName);
            }
            sourceString.Sort((x, y) => string.Compare(x, y));
            return sourceString.Distinct().ToList();
        }

        public void ConnectDisconnect()
        {
            string path = Directory.GetCurrentDirectory();
            string[] lines = System.IO.File.ReadAllLines(path + "/sensitive.txt");

            Console.WriteLine("In connect disconnect");
            if (!isConnected)
            {
                try
                {
                    obs.Connect("ws://" + lines[0] + ":4444", lines[1]);
                    if (obs.IsConnected)
                    {
                        Version pluginVersion = new Version(obs.GetVersion().PluginVersion);
                        if (pluginVersion.CompareTo(new Version("4.9.0")) < 0)
                        {
                            throw new ErrorResponseException("Incompatible plugin Version. Please update your OBS-Websocket plugin");
                        }
                    }
                }
                catch (AuthFailureException)
                {
                    isConnected = false;
                }
            }
            else
            {
                obs.Disconnect();
                isConnected = false;
            }
        }

        private void OBS_Connected(object sender, EventArgs e)
        {
            isConnected = true;
            Console.WriteLine("is Connected");

        }

        private void OBS_Disconnected(object sender, EventArgs e)
        {
            isConnected = false;
            Console.WriteLine("is Disconnected");
        }
    }
}