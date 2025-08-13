#!/bin/bash
# Usage: Run from repo root. Edits all files and renames files/folders from $SOURCE to $TARGET

SOURCE="ContainerTestImage"
TARGET="ContainerTestImage"

# Replace content in all files (excluding binary files)
find . -type f ! -path '*/.git/*' -exec grep -Iq . {} \; -and -print | while read -r file; do
    sed -i "s/${SOURCE}/${TARGET}/g" "$file"
done

# Rename files and directories
find . -depth -name "*${SOURCE}*" | while read -r path; do
    newpath="$(dirname "$path")/$(basename "$path" | sed "s/${SOURCE}/${TARGET}/g")"
    mv "$path" "$newpath"
done
