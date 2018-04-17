#!/usr/bin/env bash
if [ -f "$HOME"/.bash_profile ]; then
source "$HOME"/.bash_profile
fi
echo "${PMGMT_CLIENT_ID}"
echo "${PMGMT_REST_RESOURCE_NAME}"
sed -i '' "s/REST_RESOURCE_NAME/${PMGMT_REST_RESOURCE_NAME}/g" ../ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${PMGMT_CLIENT_ID}/g" ../ProspectManagement/Constants/PrivateKeys.cs
echo "$1"
if [ "$1" == "LOCALDEV" ]; then
    echo "Changing REST endpoint to njrdbkedev1:7101/context-root-JR550027"
    sed -i '' "s#https://REST_ENDPOINT#http://njrdbkedev1:7101/context-root-JR550027#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects Local" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.dev ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset ../iOS/info.plist
elif [ "$1" == "DEV" ]; then
    echo "Changing REST endpoint to mobilitye1dv.khov.com/DV910"
    sed -i '' "s#REST_ENDPOINT#mobilitye1dv.khov.com/DV910#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects Dev" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.dev ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset ../iOS/info.plist
elif [ "$1" == "PY" ]; then
    echo "Changing REST endpoint to bssve1py.khov.com/PY910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1py.khov.com/PY910#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects PY" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.py ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-PY.appiconset ../iOS/info.plist
elif [ "$1" == "TRN" ]; then
    echo "Changing REST endpoint to bssve1tr.khov.com/TR910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1tr.khov.com/TR910#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects TRN" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.trn ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-TRN.appiconset ../iOS/info.plist
elif [ "$1" == "PROD" ]; then
    echo "Changing REST endpoint to bssve1pd.khov.com/PD910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1pd.khov.com/PD910#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons.appiconset ../iOS/info.plist
fi
