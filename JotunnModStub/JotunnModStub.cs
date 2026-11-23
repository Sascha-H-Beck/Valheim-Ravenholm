using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Reflection;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace Monostar
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class Monostar : BaseUnityPlugin
    {
        public const string PluginGUID = "de.monostar.zerocodefx";
        public const string PluginName = "Monostar";
        public const string PluginVersion = "0.0.3";
        private const string ToggleVisionButton = "Monostar_ToggleVision";

        private ConfigEntry<KeyboardShortcut> ToggleVisionKey;
        private bool IsVisionActive;
        private float nextCharacterScan;

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            ToggleVisionKey = Config.Bind(
                "Predator Vision Controls",
                "ToggleVisionKey",
                new KeyboardShortcut(KeyCode.F8),
                "Keyboard shortcut used to toggle the red vision outline.");
            InputManager.Instance.AddButton(PluginGUID, new ButtonConfig
            {
                Name = ToggleVisionButton,
                Shortcut = ToggleVisionKey.Value,
                Hint = "Toggle Vision"
            });

            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("Monostar is Enabled");

            // To learn more about Jotunn's features, go to
            // https://valheim-modding.github.io/Jotunn/tutorials/overview.html
        }

        private void Update()
        {
            var rawKeyPressed = Input.GetKeyDown(ToggleVisionKey.Value.MainKey) && !IsTextInputFocused();
            if (rawKeyPressed || IsToggleVisionPressed())
            {
                Jotunn.Logger.LogInfo($"{ToggleVisionKey.Value.MainKey} pressed by Monostar.");
                IsVisionActive = !IsVisionActive;
                if (IsVisionActive)
                {
                    UpdateCharacterHighlights(true);
                    nextCharacterScan = Time.time + 0.5f;
                }
                else
                {
                    ThermalMaterialManager.RestoreAll();
                }

                Jotunn.Logger.LogInfo($"Predator vision {(IsVisionActive ? "enabled" : "disabled")}.");
            }

            if (IsVisionActive && Time.time >= nextCharacterScan)
            {
                nextCharacterScan = Time.time + 0.5f;
                UpdateCharacterHighlights();
            }
        }

        private void UpdateCharacterHighlights(bool logResults = false)
        {
            var characterType = FindType("Character");
            var getAllCharacters = characterType == null
                ? null
                : Array.Find(characterType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), method =>
                {
                    return method.Name == "GetAllCharacters" && method.GetParameters().Length == 0;
                });
            var characters = getAllCharacters?.Invoke(null, null) as IEnumerable;
            if (characters == null && characterType != null)
            {
                characters = UnityEngine.Object.FindObjectsOfType(characterType) as IEnumerable;
            }

            if (characters == null)
            {
                Jotunn.Logger.LogWarning("Monostar could not find the active character list.");
                return;
            }

            var processedCharacters = 0;
            var updatedRenderers = 0;
            foreach (var character in characters)
            {
                if (!(character is Component characterComponent) || characterComponent == null)
                {
                    continue;
                }

                processedCharacters++;
                var type = characterComponent.GetType();
                var isPlayer = type.GetMethod("IsPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var isDead = type.GetMethod("IsDead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (isPlayer != null && (bool)isPlayer.Invoke(characterComponent, null))
                {
                    ThermalMaterialManager.Restore(characterComponent);
                    continue;
                }

                if (isDead != null && (bool)isDead.Invoke(characterComponent, null))
                {
                    ThermalMaterialManager.Restore(characterComponent);
                    continue;
                }

                updatedRenderers += ThermalMaterialManager.Apply(characterComponent);
            }

            ThermalMaterialManager.RemoveDestroyedRenderers();
            if (logResults)
            {
                Jotunn.Logger.LogInfo($"Predator vision scanned {processedCharacters} characters and applied red material to {updatedRenderers} renderers.");
            }
        }

        private static bool IsToggleVisionPressed()
        {
            var zInputType = FindType("ZInput");
            var getButtonDown = zInputType?.GetMethod("GetButtonDown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
            return getButtonDown != null && (bool)getButtonDown.Invoke(null, new object[] { ToggleVisionButton });
        }

        private static bool IsTextInputFocused()
        {
            var chatType = FindType("Chat");
            var chatInstance = chatType?.GetField("instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
            var hasFocus = chatType?.GetMethod("HasFocus", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (chatInstance != null && hasFocus != null && (bool)hasFocus.Invoke(chatInstance, null))
            {
                return true;
            }

            var textInputType = FindType("TextInput");
            var isVisible = textInputType?.GetMethod("IsVisible", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return isVisible != null && (bool)isVisible.Invoke(null, null);
        }

        private static Type FindType(string name)
        {
            Type fallback = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name);
                if (type != null)
                {
                    fallback = fallback ?? type;
                    if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                    {
                        return type;
                    }
                }

                try
                {
                    type = Array.Find(assembly.GetTypes(), candidate => candidate.Name == name);
                    if (type != null)
                    {
                        fallback = fallback ?? type;
                        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                        {
                            return type;
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            return fallback;
        }

        private void OnDestroy()
        {
            ThermalMaterialManager.RestoreAll();
        }

    }
}