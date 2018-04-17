#!/usr/bin/env bash
KEYPATH="${APPCENTER_SOURCE_DIRECTORY:-.}"
echo "${KEYPATH}/ProspectManagement/Constants/PrivateKeys.cs"
echo "${CLIENT_ID}"
echo "${REST_RESOURCE_NAME}"
sed -i '' "s/REST_RESOURCE_NAME/${REST_RESOURCE_NAME}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${CLIENT_ID}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
echo "${ENVIRONMENT}"
if [ "${ENVIRONMENT}" == "DEV" ]; then
    echo "Changing REST endpoint to mobilitye1dv.khov.com/DV910"
    sed -i '' "s#REST_ENDPOINT#mobilitye1dv.khov.com/DV910#g" $KEYPATH/ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects Dev" $KEYPATH/iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.dev $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset $KEYPATH/iOS/info.plist
elif [ "${ENVIRONMENT}" == "PY" ]; then
    echo "Changing REST endpoint to bssve1py.khov.com/PY910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1py.khov.com/PY910#g" $KEYPATH/ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects PY" $KEYPATH/iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.py $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-PY.appiconset $KEYPATH/iOS/info.plist
elif [ "${ENVIRONMENT}" == "TRN" ]; then
    echo "Changing REST endpoint to bssve1tr.khov.com/TR910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1tr.khov.com/TR910#g" $KEYPATH/ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects TRN" $KEYPATH/iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects.trn $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-TRN.appiconset $KEYPATH/iOS/info.plist
elif [ "${ENVIRONMENT}" == "PROD" ]; then
    echo "Changing REST endpoint to bssve1pd.khov.com/PD910"
    sed -i '' "s#https://REST_ENDPOINT#http://bssve1pd.khov.com/PD910#g" ../ProspectManagement/Constants/ConnectionURIs.cs
    plutil -replace CFBundleDisplayName -string "Prospects" ../iOS/info.plist
    #plutil -replace CFBundleIdentifier -string com.khov.prospects ../iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons.appiconset $KEYPATH/iOS/info.plist
fi