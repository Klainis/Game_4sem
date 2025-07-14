#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using TMPro.EditorUtilities;

/// <summary>
/// Утилита для создания TextMeshPro шрифта с поддержкой кириллицы
/// </summary>
public class CyrillicFontCreator : EditorWindow
{
    [MenuItem("Tools/UI System/Create Cyrillic Font")]
    public static void ShowWindow()
    {
        GetWindow<CyrillicFontCreator>("Cyrillic Font Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Cyrillic Font Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Этот инструмент создаст шрифт TextMeshPro с поддержкой кириллицы.\n" +
            "После создания шрифта он будет автоматически применен ко всем UIButton компонентам.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Cyrillic Font"))
        {
            CreateCyrillicFont();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Apply Cyrillic Font to UI"))
        {
            ApplyCyrillicFontToUI();
        }
    }

    void CreateCyrillicFont()
    {
        // Используем Liberation Sans как основу
        Font sourceFont = Resources.Load<Font>("Fonts/LiberationSans");
        if (sourceFont == null)
        {
            // Пробуем найти Arial
            sourceFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        if (sourceFont == null)
        {
            EditorUtility.DisplayDialog("Error", "Не удалось найти исходный шрифт!", "OK");
            return;
        }

        // Символы для кириллицы
        string cyrillicCharacters = GetCyrillicCharacterSet();

        Debug.Log($"[CyrillicFontCreator] Creating font with {cyrillicCharacters.Length} characters");
        Debug.Log($"[CyrillicFontCreator] Characters: {cyrillicCharacters}");

        // Создаем TMP шрифт программно
        CreateTMPFont(sourceFont, cyrillicCharacters);
    }

    void CreateTMPFont(Font sourceFont, string characterSet)
    {
        // Путь для сохранения
        string fontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Cyrillic.asset";
        
        // Создаем папку если нужно
        string directory = System.IO.Path.GetDirectoryName(fontPath);
        if (!AssetDatabase.IsValidFolder(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        // Копируем существующий шрифт как основу
        TMP_FontAsset existingFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (existingFont == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Не найден базовый шрифт LiberationSans SDF!\n" +
                "Убедитесь, что TextMeshPro правильно импортирован.", "OK");
            return;
        }

        // Создаем копию существующего шрифта
        TMP_FontAsset fontAsset = Object.Instantiate(existingFont);
        fontAsset.name = "LiberationSans SDF - Cyrillic";
        
        // Сохраняем ассет
        AssetDatabase.CreateAsset(fontAsset, fontPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", 
            "Базовый шрифтовый ассет создан!\n\n" +
            "Следующие шаги:\n" +
            "1. Выберите созданный ассет в Project (он уже выделен)\n" +
            "2. В инспекторе найдите раздел 'Font Atlas'\n" +
            "3. В поле 'Character Set' выберите 'Custom Characters'\n" +
            "4. Вставьте символы: АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя\n" +
            "5. Установите 'Atlas Resolution' на 1024x1024\n" +
            "6. Нажмите 'Generate Font Atlas'\n\n" +
            "После этого используйте кнопку 'Apply Cyrillic Font to UI'", "OK");

        // Выделяем созданный ассет
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    string GetCyrillicCharacterSet()
    {
        // Базовые ASCII символы
        string basicChars = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        
        // Кириллические символы
        string cyrillicChars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        
        // Дополнительные символы
        string additionalChars = "№";
        
        return basicChars + cyrillicChars + additionalChars;
    }

    void ApplyCyrillicFontToUI()
    {
        // Ищем кириллический шрифт по пути
        string fontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Cyrillic.asset";
        TMP_FontAsset cyrillicFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
        
        if (cyrillicFont == null)
        {
            // Пробуем найти через Resources
            cyrillicFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Cyrillic");
        }
        
        if (cyrillicFont == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Кириллический шрифт не найден!\n" +
                "Сначала создайте шрифт с помощью кнопки 'Create Cyrillic Font' и сгенерируйте Font Atlas.", "OK");
            return;
        }

        // Находим все TextMeshProUGUI компоненты в сцене и префабах
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        int updatedCount = 0;

        foreach (var text in allTexts)
        {
            text.font = cyrillicFont;
            EditorUtility.SetDirty(text);
            updatedCount++;
        }

        // Сохраняем изменения
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Success", 
            $"Кириллический шрифт применен к {updatedCount} текстовым компонентам!\n\n" +
            "Если символы всё ещё отображаются как квадратики, убедитесь что:\n" +
            "1. Вы сгенерировали Font Atlas с кириллическими символами\n" +
            "2. В Character Set были добавлены русские буквы", "OK");
        
        Debug.Log($"[CyrillicFontCreator] Applied cyrillic font to {updatedCount} TextMeshPro components");
    }
}
#endif 