#!/bin/sh

set -eu

# Make sure the file doesn't exist at the start of the tests
rm -f output.txt

# Start the app in the background
dotnet run &
APP_PID=$!

echo "\nWaiting 10 seconds…"
sleep 10

# Check if the app is running
echo "\n\nChecking healthz endpoint…"
curl -s http://localhost:5155/healthz | grep -q 'OK'

# Test demo for writing data into a file
echo "\n\nTesting writing data into a file…"
if [ -f output.txt ]; then
  echo "Test failed: the file exists before the process was started"
  exit 1
fi
curl http://localhost:5155/start
echo "\nWaiting 10 seconds…"
sleep 10
curl http://localhost:5155/stop
if [ ! -f output.txt ]; then
  echo "Test failed: the file was not created after the process was started"
  exit 1
fi
NB_LINES_BEFORE=$(wc -l output.txt | awk '{print $1}')
if [ "${NB_LINES_BEFORE}" -eq 0 ]; then
  echo "Test failed: no lines were added to the file after the process was started"
  exit 1
fi
sleep 5
NB_LINES_AFTER=$(wc -l output.txt | awk '{print $1}')
if [ "${NB_LINES_BEFORE}" -ne "${NB_LINES_AFTER}" ]; then
  echo "Test failed: new lines were added to the file after the process was stopped"
  exit 1
fi


# Kill the app
echo "\n\nStopping the app…"
kill "${APP_PID}"

exit 0
