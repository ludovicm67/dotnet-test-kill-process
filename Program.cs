using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ProcessHandler>();

var app = builder.Build();

app.MapControllers();

// Start "write to file" process
app.MapGet("/start", (ProcessHandler handler) =>
{
    handler.StartProcess();
    return Results.Ok("Process started");
});

// Stop "write to file" process
app.MapGet("/stop", (ProcessHandler handler) =>
{
    handler.StopProcess();
    return Results.Ok("Process stopped");
});

// Start "stream" process ; this will read a file and stream it
app.MapGet("/start-stream", (ProcessHandler handler) =>
{
    handler.StartStreamProcess();
    return Results.Ok("Stream process started");
});

// Stop "stream" process
app.MapGet("/stop-stream", (ProcessHandler handler) =>
{
    handler.StopStreamProcess();
    return Results.Ok("Stream process stopped");
});

// Start "handler" process ; this will read the stream and write to a file
app.MapGet("/start-handler", (ProcessHandler handler) =>
{
    handler.StartHandlerProcess();
    return Results.Ok("Handler process started");
});

// Stop "handler" process
app.MapGet("/stop-handler", (ProcessHandler handler) =>
{
    handler.StopHandlerProcess();
    return Results.Ok("Handler process stopped");
});

// Health endpoint
app.MapGet("/healthz", () =>
{
    return Results.Ok("OK");
});

app.Run();

public class ProcessHandler
{
    private Process? process;
    private Process? streamProcess;
    private Process? handlerProcess;

    public void StartProcess()
    {
        if (process == null || process.HasExited)
        {
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"while true; do echo 'Some data…' >> output.txt; sleep 2; done\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
        }
    }

    public void StopProcess()
    {
        process?.Kill();
    }

    public void StartStreamProcess()
    {
        if (streamProcess == null || streamProcess.HasExited)
        {
            streamProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-re -i bbb_sunflower_1080p_60fps_normal.webm -f mpegts udp://127.0.0.1:20696",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            streamProcess.Start();
        }
    }

    public void StopStreamProcess()
    {
        streamProcess?.Kill();
    }

    public void StartHandlerProcess()
    {
        if (handlerProcess == null || handlerProcess.HasExited)
        {
            handlerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-y -i udp://127.0.0.1:20696?timeout=8000000 output.webm",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                }
            };
            handlerProcess.Start();
        }
    }

    public void StopHandlerProcess()
    {
        // Stop the ffmpeg process by sending q to stdin (safe kill)
        if (handlerProcess != null && !handlerProcess.HasExited)
        {
            handlerProcess.StandardInput.WriteLine("q");
            StreamWriter sw = handlerProcess.StandardInput;
            sw.WriteLine("q"); // Sending 'q' to FFmpeg's stdin
            sw.Close();
            handlerProcess.WaitForExit();
        }
        // handlerProcess?.Kill();
    }
}
