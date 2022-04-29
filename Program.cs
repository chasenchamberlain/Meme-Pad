using NAudio.Wave;
using System;
using System.IO;
using Newtonsoft.Json;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using OBSControl;
using Microsoft.AspNetCore.Authentication.Certificate;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCertificateForwarding();
app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return AudioPlayer.Play(); ;
});

new OBSController();
app.Run();




class AudioPlayer
{
    static string AccessFileNames()
    {
        DirectoryInfo di = new DirectoryInfo(@"/Users/chase/Documents/twitch/audio/");
        FileInfo[] files = di.GetFiles("*.mp3");

        List<string> fileNames = new List<string>();
        foreach (FileInfo file in files)
        {
            fileNames.Add(file.Name);
        }
        Console.WriteLine("AUDIO FILES -> " + fileNames.Count);
        string json = JsonConvert.SerializeObject(fileNames);
        return json;
    }

    public static string Play()
    {
        AccessFileNames();
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
}
