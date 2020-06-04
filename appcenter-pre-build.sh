#!/usr/bin/env bash
KEYPATH="${APPCENTER_SOURCE_DIRECTORY:-.}"
echo "${KEYPATH}/ProspectManagement/Constants/PrivateKeys.cs"
echo "CLIENT_ID is ${CLIENT_ID}"
echo "REST_RESOURCE_NAME is ${REST_RESOURCE_NAME}"
echo "AZURE_KEY is ${AZURE_KEY}"
echo "APPCENTER_SECRET is ${APPCENTER_SECRET}"
echo "REST_ENDPOINT is ${REST_ENDPOINT}"
echo "COGNITIVE_VISION_URI is ${COGNITIVE_VISION_URI}"
echo "COGNITIVE_VISION_KEY is ${COGNITIVE_VISION_KEY}"
echo "TWILIO_ACCOUNT_SID is ${TWILIO_ACCOUNT_SID}"
echo "TWILIO_AUTHTOKEN is ${TWILIO_AUTHTOKEN}"
echo "AZURE_NOTIFICATION_CONNECTION_STRING is ${AZURE_NOTIFICATION_CONNECTION_STRING}"
echo "E1CRMWEBAPP_URI is ${E1CRMWEBAPP_URI}"
echo "APS_ENVIRONMENT is ${APS_ENVIRONMENT}"
sed -i '' "s/TWILIO_ACCOUNT_SID/${TWILIO_ACCOUNT_SID}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/TWILIO_AUTHTOKEN/${TWILIO_AUTHTOKEN}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s#COGNITIVE_VISION_URI#${COGNITIVE_VISION_URI}#g" $KEYPATH/ProspectManagement/Constants/ConnectionURIs.cs
sed -i '' "s/COGNITIVE_VISION_KEY/${COGNITIVE_VISION_KEY}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s#REST_ENDPOINT#${REST_ENDPOINT}#g" $KEYPATH/ProspectManagement/Constants/ConnectionURIs.cs
sed -i '' "s/REST_RESOURCE_NAME/${REST_RESOURCE_NAME}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${CLIENT_ID}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/AZURE_KEY/${AZURE_KEY}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/APPCENTER_SECRET/${APPCENTER_SECRET}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/AZURE_NOTIFICATION_CONNECTION_STRING/${AZURE_NOTIFICATION_CONNECTION_STRING}/g" $KEYPATH/ProspectManagement/Constants/AzurePushConstants.cs
sed -i '' "s/E1CRMWEBAPP_URI/${E1CRMWEBAPP_URI}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
echo "${ENVIRONMENT}"
if [ "${ENVIRONMENT}" == "DEV" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects Dev" $KEYPATH/iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.dev $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-DEV.appiconset $KEYPATH/iOS/info.plist
    plutil -replace aps-environment -string "development" ../iOS/entitlements.plist
elif [ "${ENVIRONMENT}" == "PY" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects PY" $KEYPATH/iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.py $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-PY.appiconset $KEYPATH/iOS/info.plist
    plutil -replace aps-environment -string "production" ../iOS/entitlements.plist
elif [ "${ENVIRONMENT}" == "TRN" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects TRN" $KEYPATH/iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects.trn $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons-TRN.appiconset $KEYPATH/iOS/info.plist
    plutil -replace aps-environment -string "production" ../iOS/entitlements.plist
elif [ "${ENVIRONMENT}" == "PROD" ]; then
    plutil -replace CFBundleDisplayName -string "Prospects" $KEYPATH/iOS/info.plist
    plutil -replace CFBundleIdentifier -string com.khov.prospects $KEYPATH/iOS/info.plist
    plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons.appiconset $KEYPATH/iOS/info.plist
    plutil -replace aps-environment -string "production" ../iOS/entitlements.plist
fi