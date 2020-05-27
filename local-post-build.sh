#!/usr/bin/env bash


plutil -replace CFBundleDisplayName -string "Prospects" ../iOS/info.plist
plutil -replace CFBundleIdentifier -string com.khov.prospects ../iOS/info.plist
plutil -replace XSAppIconAssets -string Resources/Assets.xcassets/AppIcons.appiconset ../iOS/info.plist