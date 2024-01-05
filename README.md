# Dotnet Kill process demo

Run the project by using:

```sh
dotnet run
```

If you call the following URL: http://localhost:5155/healthz, you should get a 200 response with `"OK"` as body.
This means that the application is running as expected.

## Process that writes to a file demo

This is a very simple process.
It's writing in a loop, every 2sec a line in a file called `output.txt`.

To start the process, just call the following URL: http://localhost:5155/start
This will start the process and write to the file.
To stop the process, just call the following URL: http://localhost:5155/stop
This will stop the process, and no more line will be written to the file.

## Streaming demo

For the streaming demo, you will need to get a media file.
I used the famous Big Buck Bunny video, available from here: https://peach.blender.org/download/

Or the direct link: https://download.blender.org/demo/movies/BBB/bbb_sunflower_1080p_60fps_normal.mp4.zip

You will need to unzip it and put the video file in the current directory, then converting it to a webm file and keep only the audio part.

This can be done using the following commands:

```sh
wget -O bbb_sunflower_1080p_60fps_normal.mp4.zip https://download.blender.org/demo/movies/BBB/bbb_sunflower_1080p_60fps_normal.mp4.zip
unzip bbb_sunflower_1080p_60fps_normal.mp4.zip
ffmpeg -y -i bbb_sunflower_1080p_60fps_normal.mp4 -vn -c:a libvorbis -q:a 4 bbb_sunflower_1080p_60fps_normal.webm
```

Similar to the previous demo, there are 2 processes which you can start and stop.

The first one to call is the handler process, which will read the stream and write it to a file called `output.webm`.
This is the endpoint to call: http://localhost:5155/start-handler

The second one is the stream process, which will stream the audio file and stream it to the first process.
This is the endpoint to call: http://localhost:5155/start-stream

To stop the processes, just call the following URLs:

- http://localhost:5155/stop-handler
- http://localhost:5155/stop-stream
