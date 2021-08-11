#!/bin/sh
dotnet run -p /c/path/to/dispel-murk/Dispel.CommandLine -- -o site /c/path/to/file.log
# scp *.html *.css server.com:/var/www/logs/
# or:
# rsync -vzt *.html *.css server.com:/var/www/limits/logs/