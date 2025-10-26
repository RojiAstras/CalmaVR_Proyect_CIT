using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Verifica y arregla materiales y shaders del proyecto
/// </summary>
public class FixMaterialsAndShaders : EditorWindow
{
    [MenuItem("VR Setup/Fix Materials and Shaders")]
    public static void FixAllMaterials()
    {
        int fixedCount = 0;
        int totalCount = 0;
        
        // Find all materials in the project
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/TriForge Assets" });
        
        Debug.Log($"Checking {materialGuids.Length} materials...");
        
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null)
            {
                totalCount++;
                
                // Check if shader is missing or broken
                if (material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
                {
                    Debug.LogWarning($"Material with broken shader: {material.name}");
                    
                    // Try to fix by assigning URP/Lit shader
                    Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpLit != null)
                    {
                        material.shader = urpLit;
                        EditorUtility.SetDirty(material);
                        fixedCount++;
                        Debug.Log($"✓ Fixed: {material.name} → URP/Lit");
                    }
                }
                // Check if using Built-in shader (needs conversion to URP)
                else if (material.shader.name.Contains("Standard") || 
                         material.shader.name.Contains("Legacy"))
                {
                    Debug.LogWarning($"Material using Built-in shader: {material.name} ({material.shader.name})");
                    
                    // Convert to URP shader
                    Shader urpShader = GetURPEquivalent(material.shader.name);
                    if (urpShader != null)
                    {
                        material.shader = urpShader;
                        EditorUtility.SetDirty(material);
                        fixedCount++;
                        Debug.Log($"✓ Converted: {material.name} → {urpShader.name}");
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        string message = $"Verificación completa!\n\n" +
                        $"Materiales revisados: {totalCount}\n" +
                        $"Materiales arreglados: {fixedCount}\n\n";
        
        if (fixedCount > 0)
        {
            message += "Los materiales han sido actualizados para usar shaders URP.";
        }
        else
        {
            message += "Todos los materiales están correctos!";
        }
        
        EditorUtility.DisplayDialog("Fix Materials", message, "OK");
        Debug.Log($"=== Fix Materials Complete: {fixedCount}/{totalCount} fixed ===");
    }
    
    private static Shader GetURPEquivalent(string builtInShaderName)
    {
        // Map Built-in shaders to URP equivalents
        Dictionary<string, string> shaderMap = new Dictionary<string, string>()
        {
            { "Standard", "Universal Render Pipeline/Lit" },
            { "Standard (Specular setup)", "Universal Render Pipeline/Lit" },
            { "Legacy Shaders/Diffuse", "Universal Render Pipeline/Simple Lit" },
            { "Legacy Shaders/Specular", "Universal Render Pipeline/Simple Lit" },
            { "Legacy Shaders/Bumped Diffuse", "Universal Render Pipeline/Lit" },
            { "Legacy Shaders/Bumped Specular", "Universal Render Pipeline/Lit" },
            { "Mobile/Diffuse", "Universal Render Pipeline/Simple Lit" },
            { "Nature/Tree Creator Leaves", "Universal Render Pipeline/Nature/SpeedTree8" },
            { "Nature/Tree Creator Bark", "Universal Render Pipeline/Nature/SpeedTree8" },
        };
        
        // Try exact match first
        if (shaderMap.ContainsKey(builtInShaderName))
        {
            return Shader.Find(shaderMap[builtInShaderName]);
        }
        
        // Try partial match
        foreach (var kvp in shaderMap)
        {
            if (builtInShaderName.Contains(kvp.Key))
            {
                return Shader.Find(kvp.Value);
            }
        }
        
        // Default to URP/Lit
        return Shader.Find("Universal Render Pipeline/Lit");
    }
    
    [MenuItem("VR Setup/Check Project Issues")]
    public static void CheckProjectIssues()
    {
        List<string> issues = new List<string>();
        List<string> warnings = new List<string>();
        
        // Check if XR Origin prefab exists
        string xrOriginPath = "Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(xrOriginPath) == null)
        {
            issues.Add("❌ XR Origin prefab no encontrado. Importa los Starter Assets del XR Interaction Toolkit.");
        }
        
        // Check if forest assets exist
        string[] requiredPaths = new string[]
        {
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Prefabs/P_fwOF_Tree_M_2.prefab",
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Textures/Terrain/TL_fwOF_Grass_01 1.terrainlayer"
        };
        
        foreach (string path in requiredPaths)
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(path) == null)
            {
                issues.Add($"❌ Asset no encontrado: {path}");
            }
        }
        
        // Check build target
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            warnings.Add("⚠ Build target no es Android. Usa 'Configure Project for Meta Quest' para cambiar.");
        }
        
        // Check scripting backend
        if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
        {
            warnings.Add("⚠ Scripting backend debería ser IL2CPP para Meta Quest.");
        }
        
        // Display results
        string message = "";
        
        if (issues.Count == 0 && warnings.Count == 0)
        {
            message = "✅ ¡Todo está bien!\n\nNo se encontraron problemas en el proyecto.\n\nPuedes proceder a crear la escena VR.";
            EditorUtility.DisplayDialog("Check Project", message, "OK");
        }
        else
        {
            if (issues.Count > 0)
            {
                message += "PROBLEMAS ENCONTRADOS:\n\n";
                foreach (string issue in issues)
                {
                    message += issue + "\n";
                }
                message += "\n";
            }
            
            if (warnings.Count > 0)
            {
                message += "ADVERTENCIAS:\n\n";
                foreach (string warning in warnings)
                {
                    message += warning + "\n";
                }
            }
            
            EditorUtility.DisplayDialog("Check Project", message, "OK");
        }
        
        // Log to console
        Debug.Log("=== Project Check Results ===");
        if (issues.Count > 0)
        {
            Debug.LogWarning($"Found {issues.Count} issues:");
            foreach (string issue in issues) Debug.LogWarning(issue);
        }
        if (warnings.Count > 0)
        {
            Debug.LogWarning($"Found {warnings.Count} warnings:");
            foreach (string warning in warnings) Debug.LogWarning(warning);
        }
        if (issues.Count == 0 && warnings.Count == 0)
        {
            Debug.Log("✅ No issues found!");
        }
    }
}
