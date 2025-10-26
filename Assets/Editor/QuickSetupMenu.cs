using UnityEngine;
using UnityEditor;

/// <summary>
/// Menú de configuración rápida que ejecuta todos los pasos necesarios en orden
/// </summary>
public class QuickSetupMenu : EditorWindow
{
    private Vector2 scrollPosition;
    private bool step1Done = false;
    private bool step2Done = false;
    private bool step3Done = false;
    private bool step4Done = false;
    
    [MenuItem("VR Setup/🚀 Quick Setup Wizard")]
    public static void ShowWindow()
    {
        QuickSetupMenu window = GetWindow<QuickSetupMenu>("VR Setup Wizard");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Header
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = Color.white;
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("VR Forest Setup Wizard", headerStyle);
        EditorGUILayout.LabelField("Para Meta Quest 2 y 3", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(20);
        
        // Instructions
        EditorGUILayout.HelpBox(
            "Este asistente te guiará paso a paso para configurar tu proyecto VR.\n" +
            "Sigue los pasos en orden para mejores resultados.",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // Step 1: Check Project
        DrawStep(
            "PASO 1: Verificar Proyecto",
            "Revisa que todos los archivos necesarios estén presentes.",
            step1Done,
            () => {
                FixMaterialsAndShaders.CheckProjectIssues();
                step1Done = true;
            },
            "Verificar Proyecto"
        );
        
        GUILayout.Space(10);
        
        // Step 2: Fix Materials
        DrawStep(
            "PASO 2: Arreglar Materiales",
            "Convierte los materiales a shaders compatibles con URP.",
            step2Done,
            () => {
                FixMaterialsAndShaders.FixAllMaterials();
                step2Done = true;
            },
            "Arreglar Materiales"
        );
        
        GUILayout.Space(10);
        
        // Step 3: Configure for Quest
        DrawStep(
            "PASO 3: Configurar para Meta Quest",
            "Ajusta las configuraciones del proyecto para Android/Quest.",
            step3Done,
            () => {
                MetaQuestProjectSetup.ConfigureForMetaQuest();
                step3Done = true;
            },
            "Configurar Proyecto"
        );
        
        GUILayout.Space(10);
        
        // Step 3.5: Manual XR Plugin
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚠ PASO 3.5: Habilitar Oculus XR Plugin (MANUAL)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Debes hacer esto manualmente:", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("1. Ve a Edit → Project Settings", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("2. Selecciona 'XR Plug-in Management'", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("3. Haz clic en la pestaña 'Android' (ícono de Android)", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("4. Marca la casilla 'Oculus'", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button("Abrir XR Plug-in Management", GUILayout.Height(30)))
        {
            MetaQuestProjectSetup.OpenXRPluginManagement();
        }
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(10);
        
        // Step 4: Create Scene
        DrawStep(
            "PASO 4: Crear Escena VR",
            "Genera la escena del bosque con todos los elementos VR.",
            step4Done,
            () => {
                VRForestSceneSetup.CreateVRForestScene();
                step4Done = true;
            },
            "Crear Escena VR"
        );
        
        GUILayout.Space(20);
        
        // Final instructions
        if (step1Done && step2Done && step3Done && step4Done)
        {
            EditorGUILayout.HelpBox(
                "✅ ¡Todos los pasos completados!\n\n" +
                "Ahora puedes:\n" +
                "1. Conectar tu Meta Quest con USB\n" +
                "2. Ir a File → Build Settings\n" +
                "3. Hacer clic en 'Build And Run'\n\n" +
                "¡Disfruta tu bosque en VR! 🌲🥽",
                MessageType.Info
            );
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Abrir Build Settings", GUILayout.Height(40)))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        
        GUILayout.Space(10);
        
        // Reset button
        if (GUILayout.Button("Reiniciar Wizard"))
        {
            step1Done = false;
            step2Done = false;
            step3Done = false;
            step4Done = false;
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawStep(string title, string description, bool isDone, System.Action action, string buttonText)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        if (isDone)
        {
            titleStyle.normal.textColor = Color.green;
            title = "✓ " + title;
        }
        
        EditorGUILayout.LabelField(title, titleStyle);
        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
        
        GUILayout.Space(5);
        
        EditorGUI.BeginDisabledGroup(isDone);
        if (GUILayout.Button(isDone ? "Completado" : buttonText, GUILayout.Height(30)))
        {
            action?.Invoke();
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndVertical();
    }
}
