#!/bin/bash
cp /var/local/config/appsettings.json /app/appsettings.json

dotnet VideoSrtSearchSystem.dll
