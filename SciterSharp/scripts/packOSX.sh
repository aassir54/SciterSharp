#!/bin/sh

echo ######## Packing '../LibConsole' directory to 'ArchiveResource.cs' ########
cd "$(dirname "$0")"
chmod +x packfolder
./packfolder ../LibConsole ../ArchiveResource.cs -csharp
