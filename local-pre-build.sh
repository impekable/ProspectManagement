#!/usr/bin/env bash
if [ -f "$HOME"/.bash_profile ]; then
source "$HOME"/.bash_profile
fi
echo "${PMGMT_CLIENT_ID}"
echo "${PMGMT_REST_RESOURCE_NAME}"
sed -i '' "s/REST_RESOURCE_NAME/${PMGMT_REST_RESOURCE_NAME}/g" ../ProspectManagement/Constants/PrivateKeys.cs
sed -i '' "s/CLIENT_ID/${PMGMT_CLIENT_ID}/g" ../ProspectManagement/Constants/PrivateKeys.cs
