#!/bin/sh

set -eu

# Start the app in the background
dotnet run &
APP_PID=$!

sleep 5

echo "\n\nChecking healthz endpoint…"
curl http://localhost:5155/healthz | grep -q 'OK'

# Kill the app
echo "\n\nStopping the app…"
kill "${APP_PID}"

exit 0
