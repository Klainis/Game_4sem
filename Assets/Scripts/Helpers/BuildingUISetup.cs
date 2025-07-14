#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Утилита для автоматической настройки BuildingUI с ghost-префабами
/// </summary>
public class BuildingUISetup : EditorWindow
{
    private BuildingUI targetBuildingUI;
    private bool autoFindGhostPrefabs = true;
    private string ghostSuffix = "_Ghost";

    [MenuItem("Tools/Building System/Setup Building UI")]
    public static void ShowWindow()
    {
        GetWindow<BuildingUISetup>("Building UI Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Building UI Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetBuildingUI = (BuildingUI)EditorGUILayout.ObjectField("Target Building UI", targetBuildingUI, typeof(BuildingUI), true);

        if (targetBuildingUI == null)
        {
            EditorGUILayout.HelpBox("Please select a BuildingUI component to setup.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();

        autoFindGhostPrefabs = EditorGUILayout.Toggle("Auto Find Ghost Prefabs", autoFindGhostPrefabs);
        
        if (autoFindGhostPrefabs)
        {
            ghostSuffix = EditorGUILayout.TextField("Ghost Suffix", ghostSuffix);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Setup Building Options"))
        {
            SetupBuildingOptions();
        }

        EditorGUILayout.Space();

        if (targetBuildingUI.buildingOptions != null && targetBuildingUI.buildingOptions.Length > 0)
        {
            EditorGUILayout.LabelField("Current Building Options:", EditorStyles.boldLabel);
            
            for (int i = 0; i < targetBuildingUI.buildingOptions.Length; i++)
            {
                var option = targetBuildingUI.buildingOptions[i];
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField($"{i}. {option.name}", GUILayout.Width(150));
                
                if (option.prefab != null)
                {
                    EditorGUILayout.LabelField("✓", GUILayout.Width(20));
                }
                else
                {
                    EditorGUILayout.LabelField("✗", GUILayout.Width(20));
                }
                
                if (option.ghostPrefab != null)
                {
                    EditorGUILayout.LabelField("👻", GUILayout.Width(20));
                }
                else
                {
                    EditorGUILayout.LabelField("-", GUILayout.Width(20));
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Legend: ✓ = Has Prefab, 👻 = Has Ghost Prefab, - = No Ghost");
        }
    }

    void SetupBuildingOptions()
    {
        if (targetBuildingUI == null)
        {
            EditorUtility.DisplayDialog("Error", "Target BuildingUI is not selected!", "OK");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(targetBuildingUI);
        SerializedProperty buildingOptionsProperty = serializedObject.FindProperty("buildingOptions");

        if (buildingOptionsProperty == null)
        {
            EditorUtility.DisplayDialog("Error", "BuildingOptions property not found!", "OK");
            return;
        }

        int updatedCount = 0;
        int foundGhostCount = 0;

        for (int i = 0; i < buildingOptionsProperty.arraySize; i++)
        {
            SerializedProperty optionProperty = buildingOptionsProperty.GetArrayElementAtIndex(i);
            SerializedProperty prefabProperty = optionProperty.FindPropertyRelative("prefab");
            SerializedProperty ghostPrefabProperty = optionProperty.FindPropertyRelative("ghostPrefab");
            SerializedProperty nameProperty = optionProperty.FindPropertyRelative("name");

            if (prefabProperty.objectReferenceValue != null)
            {
                GameObject originalPrefab = prefabProperty.objectReferenceValue as GameObject;
                
                // Обновляем имя если оно пустое
                if (string.IsNullOrEmpty(nameProperty.stringValue))
                {
                    nameProperty.stringValue = originalPrefab.name;
                    updatedCount++;
                }

                // Ищем ghost-префаб если включен автопоиск
                if (autoFindGhostPrefabs && ghostPrefabProperty.objectReferenceValue == null)
                {
                    GameObject ghostPrefab = FindGhostPrefab(originalPrefab);
                    if (ghostPrefab != null)
                    {
                        ghostPrefabProperty.objectReferenceValue = ghostPrefab;
                        foundGhostCount++;
                        updatedCount++;
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.DisplayDialog("Setup Complete", 
            $"Updated {updatedCount} building options.\nFound {foundGhostCount} ghost prefabs.", "OK");
    }

    GameObject FindGhostPrefab(GameObject originalPrefab)
    {
        if (originalPrefab == null) return null;

        string originalPath = AssetDatabase.GetAssetPath(originalPrefab);
        string directory = System.IO.Path.GetDirectoryName(originalPath);
        string originalName = originalPrefab.name;
        
        // Ищем в той же папке
        string ghostName = originalName + ghostSuffix;
        string ghostPath = System.IO.Path.Combine(directory, ghostName + ".prefab");
        
        GameObject ghostPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ghostPath);
        if (ghostPrefab != null)
        {
            return ghostPrefab;
        }

        // Ищем во всем проекте
        string[] guids = AssetDatabase.FindAssets(ghostName + " t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.name == ghostName)
            {
                // Проверяем что это действительно ghost-префаб
                if (prefab.GetComponent<BuildingGhost>() != null)
                {
                    return prefab;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Создает базовую структуру BuildingOption для нового здания
    /// </summary>
    [MenuItem("Tools/Building System/Add Building Option")]
    public static void AddBuildingOption()
    {
        BuildingUI buildingUI = FindObjectOfType<BuildingUI>();
        if (buildingUI == null)
        {
            EditorUtility.DisplayDialog("Error", "No BuildingUI found in scene!", "OK");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(buildingUI);
        SerializedProperty buildingOptionsProperty = serializedObject.FindProperty("buildingOptions");

        if (buildingOptionsProperty == null)
        {
            EditorUtility.DisplayDialog("Error", "BuildingOptions property not found!", "OK");
            return;
        }

        // Добавляем новый элемент
        buildingOptionsProperty.arraySize++;
        SerializedProperty newOption = buildingOptionsProperty.GetArrayElementAtIndex(buildingOptionsProperty.arraySize - 1);
        
        // Устанавливаем значения по умолчанию
        newOption.FindPropertyRelative("name").stringValue = "New Building";
        newOption.FindPropertyRelative("prefab").objectReferenceValue = null;
        newOption.FindPropertyRelative("ghostPrefab").objectReferenceValue = null;
        newOption.FindPropertyRelative("cost").intValue = 100;
        newOption.FindPropertyRelative("icon").objectReferenceValue = null;

        serializedObject.ApplyModifiedProperties();

        EditorUtility.DisplayDialog("Success", "New building option added!", "OK");
        
        // Выделяем BuildingUI в инспекторе
        Selection.activeObject = buildingUI;
    }
}
#endif 