# VintageBeGone

A small client-side SPT/BepInEx plugin that controls the `CC_Vintage` camera effect on every loaded camera.

## Setting

- `General.Enabled` (default: `true`): enables `CC_Vintage`. Set it to `false` to disable the effect.

While the setting is off, the plugin remembers only the effect components that were enabled before it disabled them. Turning the setting on restores those components and leaves components that were already disabled alone.

The plugin scans for `CC_Vintage` only at startup, on a setting change, and when a Unity scene loads. Cameras created later are handled through Unity's per-camera pre-cull event, without polling or repeatedly iterating over all cameras.

## Build

```powershell
dotnet build .\VintageBeGone.csproj -c Release -p:SptPath='D:\Tarkov-SPT'
```

The SPT, BepInEx, and Unity references are external (`Private=false`) and are not copied to the build output.