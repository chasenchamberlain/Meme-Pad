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
                                              "https://www.chasennash.com");
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
