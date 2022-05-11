using NAudio.Wave;
using System;
using System.IO;
using Newtonsoft.Json;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using OBSControl;
using AudioPlayer;
using Microsoft.AspNetCore.Authentication.Certificate;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://chasennash.com",
                                              "https://www.chasennash.com", "http://localhost:3000").AllowAnyHeader();
                      });
});

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();
// app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
// app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

// --------------------- Audio Calls -------------------

app.MapGet("/getaudionames", () =>
{
    return AP.AccessFileNames();
});

app.MapPost("/playaudio", (string fileName) =>
{
    // Console.WriteLine(fileName);
    return AP.Play(fileName);
});

app.MapGet("/stopaudio", () =>
{
    return AP.StopAudio();
});

app.MapGet("playrandomaudio", () =>
{
    return AP.PlayRandomAudio();
});

// --------------------- Video Calls -------------------

app.MapPost("/playvideo", (string fileName) =>
{
    // Console.WriteLine(fileName);
    return OBSC.PlayVideo(fileName);
});

app.MapGet("/stopvideo", () =>
{
    // Console.WriteLine(fileName);
    return OBSC.StopVideo();
});

app.MapGet("/getvideonames", () =>
{
    return OBSC.ScenesMediaKids();
});

app.MapGet("/playrandomvideo", () =>
{
    return OBSC.PlayRandomVideo();
});

app.Run();
