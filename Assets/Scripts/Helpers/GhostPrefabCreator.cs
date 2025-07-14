#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

/// <summary>
/// Утилита для создания ghost-префабов из обычных префабов зданий.
/// Используется только в редакторе Unity.
/// </summary>
public class GhostPrefabCreator : EditorWindow
{
    private GameObject sourcePrefab;
    private string ghostPrefabName = "";
    private bool createInSameFolder = true;
    private string customPath = "Assets/Prefabs/Ghost/";

    [MenuItem("Tools/Building System/Create Ghost Prefab")]
    public static void ShowWindow()
    {
        GetWindow<GhostPrefabCreator>("Ghost Prefab Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Ghost Prefab Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sourcePrefab = (GameObject)EditorGUILayout.ObjectField("Source Building Prefab", sourcePrefab, typeof(GameObject), false);

        if (sourcePrefab != null)
        {
            if (string.IsNullOrEmpty(ghostPrefabName))
            {
                ghostPrefabName = sourcePrefab.name + "_Ghost";
            }
        }

        ghostPrefabName = EditorGUILayout.TextField("Ghost Prefab Name", ghostPrefabName);
        
        createInSameFolder = EditorGUILayout.Toggle("Create in same folder", createInSameFolder);
        
        if (!createInSameFolder)
        {
            customPath = EditorGUILayout.TextField("Custom Path", customPath);
        }

        EditorGUILayout.Space();

        if (sourcePrefab == null)
        {
            EditorGUILayout.HelpBox("Please select a source building prefab to create ghost version.", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Create Ghost Prefab"))
        {
            CreateGhostPrefab();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Ghost prefabs contain only:\n" +
            "• MeshRenderer components\n" +
            "• RangeVisualizer (if present)\n" +
            "• BuildingGhost script\n" +
            "• BoxCollider (as trigger)\n\n" +
            "All other components are removed automatically.",
            MessageType.Info
        );
    }

    void CreateGhostPrefab()
    {
        if (sourcePrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Source prefab is not selected!", "OK");
            return;
        }

        // Создаем копию оригинального префаба
        GameObject ghostObject = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
        
        if (ghostObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to instantiate prefab!", "OK");
            return;
        }

        try
        {
            // Добавляем компонент BuildingGhost (он сам очистит ненужные компоненты)
            if (ghostObject.GetComponent<BuildingGhost>() == null)
            {
                ghostObject.AddComponent<BuildingGhost>();
            }

            // Определяем путь сохранения
            string savePath;
            if (createInSameFolder)
            {
                string sourcePath = AssetDatabase.GetAssetPath(sourcePrefab);
                string sourceDirectory = System.IO.Path.GetDirectoryName(sourcePath);
                savePath = System.IO.Path.Combine(sourceDirectory, ghostPrefabName + ".prefab");
            }
            else
            {
                if (!AssetDatabase.IsValidFolder(customPath))
                {
                    // Создаем папку если её нет
                    string[] pathParts = customPath.Split('/');
                    string currentPath = pathParts[0];
                    
                    for (int i = 1; i < pathParts.Length; i++)
                    {
                        string nextPath = currentPath + "/" + pathParts[i];
                        if (!AssetDatabase.IsValidFolder(nextPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                        }
                        currentPath = nextPath;
                    }
                }
                
                savePath = System.IO.Path.Combine(customPath, ghostPrefabName + ".prefab");
            }

            // Сохраняем как новый префаб
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(ghostObject, savePath);
            
            if (newPrefab != null)
            {
                EditorUtility.DisplayDialog("Success", 
                    $"Ghost prefab created successfully!\nPath: {savePath}", "OK");
                
                // Выделяем созданный префаб в Project window
                EditorGUIUtility.PingObject(newPrefab);
                Selection.activeObject = newPrefab;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Failed to save ghost prefab!", "OK");
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Error creating ghost prefab: {e.Message}", "OK");
        }
        finally
        {
            // Удаляем временный объект из сцены
            DestroyImmediate(ghostObject);
        }
    }

    /// <summary>
    /// Создает ghost-префабы для всех зданий в указанной папке
    /// </summary>
    [MenuItem("Tools/Building System/Create All Ghost Prefabs")]
    public static void CreateAllGhostPrefabs()
    {
        string buildingPrefabsPath = EditorUtility.OpenFolderPanel("Select Building Prefabs Folder", "Assets/Prefabs", "");
        
        if (string.IsNullOrEmpty(buildingPrefabsPath))
            return;

        // Конвертируем абсолютный путь в относительный
        buildingPrefabsPath = "Assets" + buildingPrefabsPath.Substring(Application.dataPath.Length);

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { buildingPrefabsPath });
        
        if (prefabGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No prefabs found in selected folder!", "OK");
            return;
        }

        int created = 0;
        int skipped = 0;

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null) continue;

            // Пропускаем если это уже ghost-префаб
            if (prefab.name.Contains("Ghost") || prefab.GetComponent<BuildingGhost>() != null)
            {
                skipped++;
                continue;
            }

            // Проверяем, есть ли компоненты зданий
            if (prefab.GetComponent<BuildingBase>() == null && 
                prefab.GetComponent<ProductionBuilding>() == null &&
                prefab.GetComponent<GoldMine>() == null &&
                prefab.GetComponent<Cannon>() == null &&
                prefab.GetComponent<Wall>() == null)
            {
                skipped++;
                continue;
            }

            try
            {
                // Создаем ghost-префаб
                GameObject ghostObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                ghostObject.AddComponent<BuildingGhost>();

                string directory = System.IO.Path.GetDirectoryName(prefabPath);
                string ghostPath = System.IO.Path.Combine(directory, prefab.name + "_Ghost.prefab");

                PrefabUtility.SaveAsPrefabAsset(ghostObject, ghostPath);
                DestroyImmediate(ghostObject);

                created++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create ghost for {prefab.name}: {e.Message}");
                skipped++;
            }
        }

        EditorUtility.DisplayDialog("Batch Creation Complete", 
            $"Created {created} ghost prefabs.\nSkipped {skipped} prefabs.", "OK");
    }
}
#endif 