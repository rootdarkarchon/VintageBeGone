# VintageBeGone

A small client-side SPT/BepInEx plugin that controls the `CC_Vintage` camera effect on every loaded camera.

## Compatibility

Version 1.0.1 targets SPT 4.1. The Release build was validated against SPT 4.1.6 / EFT 0.16.9.40743. In-game visual behavior has not yet been verified on this version; compilation alone does not establish runtime compatibility. SPT 4.0 is not a validation target for this release.

## Installation

With the game closed, extract `VintageBeGone-1.0.1-SPT4.1.zip` into your SPT game folder. The plugin should be at `BepInEx/plugins/VintageBeGone/VintageBeGone.dll`. When upgrading, replace or remove any older copy of `VintageBeGone.dll` elsewhere under `BepInEx/plugins` so only one copy remains.

Existing configuration is preserved in `BepInEx/config/com.rootd.vintagebegone.cfg`. No server mod is required.

## Setting

- `General` → `Vintage Effect Enabled` (default: `false`): keep this off to disable `CC_Vintage`, or turn it on to restore effects disabled by the plugin.

While the setting is off, the plugin remembers only the effect components that were enabled before it disabled them. Turning the setting on restores those components and leaves components that were already disabled alone.

The plugin scans for `CC_Vintage` only at startup, on a setting change, and when a Unity scene loads. Cameras created later are handled through Unity's per-camera pre-cull event, without polling or repeatedly iterating over all cameras.

## Build

```powershell
dotnet build .\VintageBeGone.csproj -c Release
```

The default SPT path is `D:\Tarkov-SPT-4.1`. To use another SPT 4.1 installation, append `-p:SptPath='D:\Your-SPT-4.1'`. The plugin targets .NET Framework 4.7.2 and is written to `bin/Release/VintageBeGone.dll`.

The SPT, BepInEx, and Unity references are external (`Private=false`) and are not copied to the build output.

## In-game validation pending

- Confirm the effect is disabled by default in menus, the hideout, and raids using a fresh configuration.
- Toggle the setting repeatedly; only components disabled by the plugin should be restored, while originally disabled components stay disabled.
- Check scene transitions, newly rendering cameras, raid exit/re-entry, and plugin cleanup for correct restoration and errors.
