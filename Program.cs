using NAudio.Wave;
using System;
using System.IO;
using Newtonsoft.Json;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using OBSControl;
using AudioPlayer;
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

OBSController OBSC = new OBSController();
AudioPlayerController AP = new AudioPlayerController();

app.MapGet("/", () =>
{
    return AP.AccessFileNames(); ;
});

app.MapGet("/itsworking", () =>
{
    OBSC.ItsWorking();
});

app.MapGet("/getaudionames", () =>
{
    return AP.AccessFileNames();
});

app.MapPost("/playaudio", (string fileName) =>
{
    // Console.WriteLine(fileName);
    AP.Play(fileName);
});

app.MapPost("/playvideo", (string fileName) =>
{
    // Console.WriteLine(fileName);
    OBSC.playVideo(fileName);
});

app.MapPost("/stopvideo", (string fileName) =>
{
    // Console.WriteLine(fileName);
    OBSC.stopVideo(fileName);
});

app.MapGet("/getvideonames", () =>
{
    return OBSC.scenesMediaKids();
});

// app.MapGet("/getAllMediaSources", () =>
// {
//     return OBSC.GetSceneKids();
// });

app.Run();
