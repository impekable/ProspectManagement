#!/usr/bin/env bash
if [ -f "$HOME"/.bash_profile ]; then
source "$HOME"/.bash_profile "$1" "PMGMT"
fi
echo "CLIENT_ID is ${CLIENT_ID}"
echo "REST_RESOURCE_NAME is ${REST_RESOURCE_NAME}"
echo "AZURE_KEY is ${AZURE_KEY}"
echo "APPCENTER_SECRET is ${APPCENTER_SECRET}"
echo "REST_ENDPOINT is ${REST_ENDPOINT}"
sed -i '' "s#REST_ENDPOINT#${REST_ENDPOINT}#g" ../ProspectManagement/Constants/ConnectionURIs.cs
sed -i '' "s/REST_RESOURCE_NAME/${REST_RESOURCE_NAME}/g" ../ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${CLIENT_ID}/g" ../ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/AZURE_KEY/${AZURE_KEY}/g" ../ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/APPCENTER_SECRET/${APPCENTER_SECRET}/g" ../ProspectManagement/Constants/PrivateKeys.cs
echo "$1"
if [ "$1" == "LOCALDEV" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects Local" ../iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.dev ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset ../iOS/info.plist
elif [ "$1" == "DEV" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects-Dev" ../iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.dev ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset ../iOS/info.plist
elif [ "$1" == "PY" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects-PY" ../iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.py ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-PY.appiconset ../iOS/info.plist
elif [ "$1" == "TRN" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects-Training" ../iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.trn ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-TRN.appiconset ../iOS/info.plist
elif [ "$1" == "PROD" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects" ../iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons.appiconset ../iOS/info.plist
fi