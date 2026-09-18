using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VintageBeGone
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class VintageBeGonePlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.rootd.vintagebegone";
        public const string PluginName = "VintageBeGone";
        public const string PluginVersion = "1.0.1";

        private readonly HashSet<CC_Vintage> _disabledByPlugin = new HashSet<CC_Vintage>();
        private readonly Dictionary<int, CameraEffects> _cameraEffects = new Dictionary<int, CameraEffects>();
        private ConfigEntry<bool> _vintageEnabled;

        private void Awake()
        {
            _vintageEnabled = Config.Bind(
                "General",
                "Vintage Effect Enabled",
                false,
                "Enable the CC_Vintage camera effect. Keep this off to disable it on all loaded and newly rendering cameras.");

            _vintageEnabled.SettingChanged += OnSettingChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Camera.onPreCull += OnCameraPreCull;

            ApplyConfiguredStateToLoadedEffects();
            Logger.LogInfo($"{PluginName} {PluginVersion} loaded. CC_Vintage is {DescribeState()}.");
        }

        private void OnSettingChanged(object sender, EventArgs eventArgs)
        {
            _cameraEffects.Clear();
            ApplyConfiguredStateToLoadedEffects();
            Logger.LogInfo($"CC_Vintage is now {DescribeState()}.");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _cameraEffects.Clear();
            ApplyConfiguredStateToLoadedEffects();
        }

        private void OnCameraPreCull(Camera camera)
        {
            if (_vintageEnabled.Value || camera == null)
                return;

            int cameraId = camera.GetInstanceID();
            if (!_cameraEffects.TryGetValue(cameraId, out CameraEffects entry) || entry.Camera != camera)
            {
                entry = new CameraEffects(camera, camera.GetComponents<CC_Vintage>());
                _cameraEffects[cameraId] = entry;
            }

            foreach (CC_Vintage effect in entry.Effects)
                DisableAndRemember(effect);
        }

        private void ApplyConfiguredStateToLoadedEffects()
        {
            RemoveDestroyedEntries();

            if (_vintageEnabled.Value)
            {
                RestoreRememberedEffects();
                return;
            }

            CC_Vintage[] effects = Resources.FindObjectsOfTypeAll<CC_Vintage>();
            foreach (CC_Vintage effect in effects)
            {
                if (IsLoadedSceneComponent(effect))
                    DisableAndRemember(effect);
            }
        }

        private void DisableAndRemember(CC_Vintage effect)
        {
            if (effect == null || !effect.enabled)
                return;

            _disabledByPlugin.Add(effect);
            effect.enabled = false;
            Logger.LogDebug($"Disabled CC_Vintage on '{effect.gameObject.name}' (instance {effect.GetInstanceID()}).");
        }

        private void RestoreRememberedEffects()
        {
            foreach (CC_Vintage effect in _disabledByPlugin)
            {
                if (effect != null)
                    effect.enabled = true;
            }

            _disabledByPlugin.Clear();
        }

        private void RemoveDestroyedEntries()
        {
            _disabledByPlugin.RemoveWhere(effect => effect == null);
        }

        private static bool IsLoadedSceneComponent(CC_Vintage effect)
        {
            if (effect == null || effect.gameObject == null)
                return false;

            Scene scene = effect.gameObject.scene;
            return scene.IsValid() && scene.isLoaded;
        }

        private string DescribeState()
        {
            return _vintageEnabled.Value ? "enabled" : "disabled";
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= OnCameraPreCull;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_vintageEnabled != null)
                _vintageEnabled.SettingChanged -= OnSettingChanged;

            _cameraEffects.Clear();
            RestoreRememberedEffects();
        }

        private sealed class CameraEffects
        {
            public CameraEffects(Camera camera, CC_Vintage[] effects)
            {
                Camera = camera;
                Effects = effects;
            }

            public Camera Camera { get; }

            public CC_Vintage[] Effects { get; }
        }
    }
}
