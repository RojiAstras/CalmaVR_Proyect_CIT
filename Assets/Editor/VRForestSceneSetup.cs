using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class VRForestSceneSetup : EditorWindow
{
    [MenuItem("VR Setup/Create VR Forest Scene")]
    public static void CreateVRForestScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Remove default camera (XR Origin will have its own)
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            DestroyImmediate(mainCamera);
        }
        
        // 1. Add XR Origin (VR Rig)
        string xrOriginPath = "Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
        GameObject xrOriginPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(xrOriginPath);
        
        if (xrOriginPrefab != null)
        {
            GameObject xrOrigin = PrefabUtility.InstantiatePrefab(xrOriginPrefab) as GameObject;
            xrOrigin.transform.position = new Vector3(0, 0, 0);
            
            // Add VR Locomotion Setup script
            if (xrOrigin.GetComponent<VRLocomotionSetup>() == null)
            {
                xrOrigin.AddComponent<VRLocomotionSetup>();
            }
            
            // Add Terrain Boundary Protection script
            TerrainBoundaryProtection boundaryProtection = xrOrigin.AddComponent<TerrainBoundaryProtection>();
            boundaryProtection.safeAreaSize = new Vector2(95f, 95f); // Slightly smaller than terrain
            boundaryProtection.safeAreaCenter = Vector2.zero;
            boundaryProtection.minimumHeight = -5f;
            boundaryProtection.respawnPosition = new Vector3(0, 2, 0);
            
            Debug.Log("XR Origin added successfully with locomotion and boundary protection");
        }
        else
        {
            Debug.LogError("XR Origin prefab not found at: " + xrOriginPath);
        }
        
        // 2. Create Terrain
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = 513;
        terrainData.size = new Vector3(100, 20, 100);
        terrainData.baseMapResolution = 1024;
        terrainData.SetDetailResolution(1024, 16);
        
        // Create terrain GameObject
        GameObject terrainGO = Terrain.CreateTerrainGameObject(terrainData);
        terrainGO.name = "Forest Terrain";
        terrainGO.transform.position = new Vector3(-50, 0, -50);
        
        // Get terrain component
        Terrain terrain = terrainGO.GetComponent<Terrain>();
        
        // Apply terrain layers from the forest template
        string[] terrainLayerPaths = new string[]
        {
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Textures/Terrain/TL_fwOF_Grass_01 1.terrainlayer",
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Textures/Terrain/TL_fwOF_Soil_01.terrainlayer",
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Textures/Terrain/TL_fwOF_TerrainMoss_01.terrainlayer"
        };
        
        TerrainLayer[] layers = new TerrainLayer[terrainLayerPaths.Length];
        for (int i = 0; i < terrainLayerPaths.Length; i++)
        {
            layers[i] = AssetDatabase.LoadAssetAtPath<TerrainLayer>(terrainLayerPaths[i]);
        }
        terrain.terrainData.terrainLayers = layers;
        
        // Create some basic terrain height variation
        CreateTerrainHeights(terrainData);
        
        Debug.Log("Terrain created successfully");
        
        // 3. Add some trees and rocks
        AddForestObjects(terrain);
        
        // 4. Setup Lighting
        GameObject directionalLight = GameObject.Find("Directional Light");
        if (directionalLight != null)
        {
            Light light = directionalLight.GetComponent<Light>();
            light.intensity = 1.5f;
            light.color = new Color(1f, 0.95f, 0.85f);
            directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        
        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.53f, 0.69f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.5f, 0.5f, 0.5f);
        RenderSettings.ambientGroundColor = new Color(0.08f, 0.17f, 0.03f);
        
        // Enable fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.005f;
        RenderSettings.fogColor = new Color(0.53f, 0.69f, 0.83f);
        
        Debug.Log("Lighting configured");
        
        // 5. Save the scene
        string scenePath = "Assets/Scenes/VRForestScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log("VR Forest Scene created successfully at: " + scenePath);
        EditorUtility.DisplayDialog("Success", "VR Forest Scene created successfully!\n\nThe scene is ready for Meta Quest.\n\nPath: " + scenePath, "OK");
    }
    
    private static void CreateTerrainHeights(TerrainData terrainData)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];
        
        // Create gentle rolling hills
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * 3;
                float yCoord = (float)y / height * 3;
                
                // Use Perlin noise for natural terrain
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] = sample * 0.15f; // Keep it relatively flat for walking
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
    }
    
    private static void AddForestObjects(Terrain terrain)
    {
        // Load prefabs
        string[] treePaths = new string[]
        {
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Prefabs/P_fwOF_Tree_M_2.prefab",
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Prefabs/P_fwOF_TreeSapling_02B.prefab"
        };
        
        string[] rockPaths = new string[]
        {
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Prefabs/P_fwOF_Rock_01.prefab",
            "Assets/TriForge Assets/Fantasy Worlds - DEMO Content/Prefabs/P_fwOF_Stone_01.prefab"
        };
        
        // Create parent object for organization
        GameObject forestObjects = new GameObject("Forest Objects");
        
        // Add trees
        for (int i = 0; i < 15; i++)
        {
            foreach (string path in treePaths)
            {
                GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (treePrefab != null)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(-40f, 40f),
                        0,
                        Random.Range(-40f, 40f)
                    );
                    
                    // Sample terrain height
                    randomPos.y = terrain.SampleHeight(randomPos);
                    
                    GameObject tree = PrefabUtility.InstantiatePrefab(treePrefab) as GameObject;
                    tree.transform.position = randomPos;
                    tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    tree.transform.parent = forestObjects.transform;
                    
                    // Ensure trees have colliders
                    if (tree.GetComponent<Collider>() == null)
                    {
                        CapsuleCollider collider = tree.AddComponent<CapsuleCollider>();
                        collider.radius = 0.5f;
                        collider.height = 5f;
                        collider.center = new Vector3(0, 2.5f, 0);
                    }
                }
            }
        }
        
        // Add rocks
        for (int i = 0; i < 10; i++)
        {
            foreach (string path in rockPaths)
            {
                GameObject rockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (rockPrefab != null)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(-40f, 40f),
                        0,
                        Random.Range(-40f, 40f)
                    );
                    
                    randomPos.y = terrain.SampleHeight(randomPos);
                    
                    GameObject rock = PrefabUtility.InstantiatePrefab(rockPrefab) as GameObject;
                    rock.transform.position = randomPos;
                    rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    rock.transform.parent = forestObjects.transform;
                    
                    // Ensure rocks have colliders
                    if (rock.GetComponent<Collider>() == null)
                    {
                        MeshCollider collider = rock.AddComponent<MeshCollider>();
                        collider.convex = true;
                    }
                }
            }
        }
        
        Debug.Log("Forest objects added");
    }
}
