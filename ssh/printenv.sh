#!/bin/sh

dotnet_pid=$(pgrep dotnet)
echo $dotnet_pid
cat /proc/$dotnet_pid/environ | tr '\0' '\n'
