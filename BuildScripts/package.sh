#!/bin/bash
dir="RELEASES"
if [ ! -d "${dir}" ]; then
    mkdir ${dir}
fi

read -p "VERSION: " ver

zip -j ${dir}/JellyNews-v${ver}.zip \
    Jellyfin.Plugin.JellyNews/bin/Release/net8.0/Jellyfin.Plugin.JellyNews.dll \
    Jellyfin.Plugin.JellyNews/bin/Release/net8.0/publish/SQLitePCL.pretty.dll

echo '---'

echo "CHECKSUM: `md5sum ${dir}/JellyNews-v${ver}.zip`"