#!/bin/sh

echo ######## Packing '../LibConsole' directory to 'ArchiveResource.cs' ########
chmod +x scripts/packfolder
scripts/packfolder LibConsole ArchiveResource.cs -csharp