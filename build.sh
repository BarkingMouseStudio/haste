#!/usr/bin/env bash

UNITY_PATH="/Applications/Unity/Unity.app"

UNITYENGINE_PATH="${UNITY_PATH}/Contents/Frameworks/Managed/UnityEngine.dll"
UNITYEDITOR_PATH="${UNITY_PATH}/Contents/Frameworks/Managed/UnityEditor.dll"

PROJECT_PATH=$(pwd)
SCRIPTS=$(find . -type f -name "*.cs")

HASTE_FREE_NAME="HasteFree.dll"
HASTE_PRO_NAME="HastePro.dll"

echo "Building Haste Free... ${PROJECT_PATH}/${HASTE_FREE_NAME}"
mcs -r:$UNITYENGINE_PATH,$UNITYEDITOR_PATH -target:library -out:$HASTE_FREE_NAME $SCRIPTS

echo "Building Haste Pro... ${PROJECT_PATH}/${HASTE_PRO_NAME}"
mcs -r:$UNITYENGINE_PATH,$UNITYEDITOR_PATH -target:library -define:IS_HASTE_PRO -out:$HASTE_PRO_NAME $SCRIPTS