using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Presets;
using UnityEngine;

public class TransferGameObjectComponents
{
    private void OnPostprocessPrefab(GameObject gameObject)
    {
        // Recursive call to apply the template
        //SetupAllComponents(gameObject, Path.GetDirectoryName(assetPath), context);
    }

    private static void SetupAllComponents(GameObject go, string folder, AssetImportContext context = null)
    {
        // Don't apply templates to the templates!
        if (go.name == "Template" || go.name.Contains("Base"))
            return;

        // If we reached the root, stop
        if (string.IsNullOrEmpty(folder))
            return;

        // We add the path as a dependency so this gets reimported if the prefab changes
        var templatePath = string.Join("/", folder, "Template.prefab");
        if (context != null)
            context.DependsOnArtifact(templatePath);

        // If the file doesn't exist, check in our parent folder
        if (!File.Exists(templatePath))
        {
            SetupAllComponents(go, Path.GetDirectoryName(folder), context);
            return;
        }

        // Apply the template
        var template = AssetDatabase.LoadAssetAtPath<GameObject>(templatePath);
        if (template)
            ApplyTemplateRecursive(go, template);
    }

    private static void ApplyTemplateRecursive(GameObject go, GameObject template)
    {
        // If this is a variant, apply the base prefab first
        var templateSource = PrefabUtility.GetCorrespondingObjectFromSource(template);
        if (templateSource)
            ApplyTemplateRecursive(go, templateSource);

        // Apply the overrides from this prefab
        ApplyTemplate(go, template);
    }

    private static void ApplyTemplate(GameObject go, GameObject template)
    {
        // Get all the components in the object
        foreach (var comp in go.GetComponents<Component>())
        {
            // Try to get the corresponding component in the template
            if (!template.TryGetComponent(comp.GetType(), out var templateComp))
                continue;

            // Get all the modifications
            var overrides = new List<string>();
            var changes = PrefabUtility.GetPropertyModifications(templateComp);
            if (changes == null || changes.Length == 0)
                continue;

            // Filter only the ones that are for this component
            var target = PrefabUtility.GetCorrespondingObjectFromSource(templateComp);
            foreach (var change in changes)
            {
                if (change.target == target)
                    overrides.Add(change.propertyPath);
            }

            // Create the preset
            var preset = new Preset(templateComp);
            // Apply only the selected ones
            if (overrides.Count > 0)
                preset.ApplyTo(comp, overrides.ToArray());
        }
    }
}
