using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

namespace Monostar
{
    internal static class ThermalMaterialManager
    {
        private sealed class RendererState
        {
            public Renderer Renderer;
            public Component Owner;
            public Material[] OriginalMaterials;
            public Material[] VisionMaterials;
        }

        private static readonly Dictionary<int, RendererState> RendererStates = new Dictionary<int, RendererState>();

        public static int Apply(Component character)
        {
            if (character == null)
            {
                return 0;
            }

            var updated = 0;
            var renderers = character.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                var instanceId = renderer.GetInstanceID();
                if (!RendererStates.TryGetValue(instanceId, out var state))
                {
                    var originalMaterials = renderer.sharedMaterials;
                    var visionMaterials = new Material[originalMaterials.Length];
                    for (var index = 0; index < originalMaterials.Length; index++)
                    {
                        if (originalMaterials[index] != null)
                        {
                            visionMaterials[index] = new Material(originalMaterials[index])
                            {
                                name = "Monostar_PredatorVision_Red"
                            };
                            ConfigureVisionMaterial(visionMaterials[index]);
                        }
                    }

                    state = new RendererState
                    {
                        Renderer = renderer,
                        Owner = character,
                        OriginalMaterials = originalMaterials,
                        VisionMaterials = visionMaterials
                    };
                    RendererStates.Add(instanceId, state);
                }

                renderer.sharedMaterials = state.VisionMaterials;
                updated++;
            }

            return updated;
        }

        public static int Restore(Component character)
        {
            if (character == null)
            {
                return 0;
            }

            var restored = 0;
            var instanceIds = new List<int>(RendererStates.Keys);
            foreach (var instanceId in instanceIds)
            {
                var state = RendererStates[instanceId];
                if (state.Owner != character)
                {
                    continue;
                }

                if (state.Renderer != null)
                {
                    state.Renderer.sharedMaterials = state.OriginalMaterials;
                    restored++;
                }

                DestroyVisionMaterials(state.VisionMaterials);
                RendererStates.Remove(instanceId);
            }

            return restored;
        }

        public static void RestoreAll()
        {
            foreach (var state in RendererStates.Values)
            {
                if (state.Renderer != null)
                {
                    state.Renderer.sharedMaterials = state.OriginalMaterials;
                }

                DestroyVisionMaterials(state.VisionMaterials);
            }

            RendererStates.Clear();
        }

        public static void RemoveDestroyedRenderers()
        {
            var deadIds = new List<int>();
            foreach (var pair in RendererStates)
            {
                if (pair.Value.Renderer == null || pair.Value.Owner == null)
                {
                    deadIds.Add(pair.Key);
                }
            }

            foreach (var instanceId in deadIds)
            {
                DestroyVisionMaterials(RendererStates[instanceId].VisionMaterials);
                RendererStates.Remove(instanceId);
            }
        }

        private static void ConfigureVisionMaterial(Material material)
        {
            var red = new Color(1f, 0f, 0.06f);
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", red);
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", red);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", red * 3f);
            }
        }

        private static void DestroyVisionMaterials(Material[] materials)
        {
            foreach (var material in materials)
            {
                if (material != null)
                {
                    Object.Destroy(material);
                }
            }
        }
    }
}
