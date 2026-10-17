#!/usr/bin/env bash
# Builds the signed Play Store bundle (.aab).
# Usage: GODOT_ANDROID_KEYSTORE_RELEASE_PATH=/path/to/release.keystore \
#        GODOT_ANDROID_KEYSTORE_RELEASE_USER=<alias> \
#        GODOT_ANDROID_KEYSTORE_RELEASE_PASSWORD=<password> ./build-release.sh
set -euo pipefail
cd "$(dirname "$0")"
: "${GODOT_ANDROID_KEYSTORE_RELEASE_PATH:?set the release keystore path}"
: "${GODOT_ANDROID_KEYSTORE_RELEASE_USER:?set the keystore alias}"
: "${GODOT_ANDROID_KEYSTORE_RELEASE_PASSWORD:?set the keystore password}"
export JAVA_HOME="${JAVA_HOME:-$HOME/tools/jdk17}" ANDROID_HOME="${ANDROID_HOME:-$HOME/Android/Sdk}"
GODOT="${GODOT:-$HOME/tools/godot-4.7.2/Godot_v4.7.2-stable_mono_linux_x86_64/Godot_v4.7.2-stable_mono_linux.x86_64}"
mkdir -p build
"$GODOT" --headless --path . --export-release Android build/7WSC-1.1.3.aab
ls -la build/*.aab
