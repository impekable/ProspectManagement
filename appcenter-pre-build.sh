#!/usr/bin/env bash
KEYPATH="${APPCENTER_SOURCE_DIRECTORY:-.}"
echo "${KEYPATH}/ProspectManagement/Constants/PrivateKeys.cs"
echo "${CLIENT_ID}"
echo "${REST_RESOURCE_NAME}"
sed -i '' "s/REST_RESOURCE_NAME/${REST_RESOURCE_NAME}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${CLIENT_ID}/g" $KEYPATH/ProspectManagement/Constants/PrivateKeys.cs
