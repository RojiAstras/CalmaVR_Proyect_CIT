using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Configura automáticamente el proyecto de Unity para Meta Quest
/// </summary>
public class MetaQuestProjectSetup : EditorWindow
{
    [MenuItem("VR Setup/Configure Project for Meta Quest")]
    public static void ConfigureForMetaQuest()
    {
        bool changesMade = false;
        
        // 1. Switch to Android platform
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            Debug.Log("Switching build target to Android...");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            changesMade = true;
        }
        
        // 2. Configure Player Settings for Android
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23; // Android 6.0
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        
        // ARM64 is required for Quest
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        
        // Graphics settings
        PlayerSettings.colorSpace = ColorSpace.Linear; // Better for VR
        PlayerSettings.Android.blitType = AndroidBlitType.Auto;
        
        // VR specific settings
        PlayerSettings.virtualRealitySupported = true;
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing; // Better performance
        
        Debug.Log("✓ Android platform settings configured");
        changesMade = true;
        
        // 3. Configure Quality Settings for VR
        ConfigureQualitySettings();
        
        // 4. Configure Graphics Settings
        ConfigureGraphicsSettings();
        
        // 5. Set company and product name if still default
        if (PlayerSettings.companyName == "DefaultCompany")
        {
            PlayerSettings.companyName = "VRDeveloper";
        }
        
        if (PlayerSettings.productName == "My project")
        {
            PlayerSettings.productName = "VR Forest Experience";
        }
        
        Debug.Log("✓ Company and product names set");
        
        // 6. Configure XR settings
        Debug.Log("⚠ IMPORTANTE: Debes habilitar manualmente el XR Plugin:");
        Debug.Log("   1. Ve a Edit → Project Settings → XR Plug-in Management");
        Debug.Log("   2. En la pestaña Android, habilita 'Oculus'");
        Debug.Log("   3. Configura las opciones de Oculus según necesites");
        
        if (changesMade)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        EditorUtility.DisplayDialog(
            "Meta Quest Configuration", 
            "Proyecto configurado para Meta Quest!\n\n" +
            "✓ Plataforma Android configurada\n" +
            "✓ Configuraciones de calidad optimizadas\n" +
            "✓ Configuraciones gráficas ajustadas\n\n" +
            "⚠ IMPORTANTE:\n" +
            "Debes habilitar manualmente 'Oculus' en:\n" +
            "Edit → Project Settings → XR Plug-in Management → Android\n\n" +
            "Luego conecta tu Meta Quest y usa Build And Run.", 
            "OK"
        );
    }
    
    private static void ConfigureQualitySettings()
    {
        // Get current quality level
        string[] qualityLevels = QualitySettings.names;
        
        // Set to Medium or High quality (good balance for Quest)
        for (int i = 0; i < qualityLevels.Length; i++)
        {
            if (qualityLevels[i].Contains("Medium") || qualityLevels[i].Contains("High"))
            {
                QualitySettings.SetQualityLevel(i, true);
                break;
            }
        }
        
        // VR optimizations
        QualitySettings.vSyncCount = 0; // VR handles vsync
        QualitySettings.antiAliasing = 2; // 2x MSAA (good for VR)
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        QualitySettings.shadows = ShadowQuality.HardOnly; // Soft shadows are expensive
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowDistance = 50f; // Reduce for better performance
        QualitySettings.shadowCascades = 2;
        
        // LOD settings
        QualitySettings.lodBias = 1.0f;
        QualitySettings.maximumLODLevel = 0;
        
        Debug.Log("✓ Quality settings optimized for VR");
    }
    
    private static void ConfigureGraphicsSettings()
    {
        // Enable GPU Skinning for better performance
        PlayerSettings.gpuSkinning = true;
        
        // Set graphics jobs
        PlayerSettings.graphicsJobs = true;
        
        Debug.Log("✓ Graphics settings configured");
    }
    
    [MenuItem("VR Setup/Open XR Plugin Management")]
    public static void OpenXRPluginManagement()
    {
        SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
    }
}
