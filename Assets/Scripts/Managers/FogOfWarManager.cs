using UnityEngine;
using System.Collections;

public class SafeMiniMapFog : MonoBehaviour
{
    [Header("Textures")]
    public RenderTexture fogTexture;
    public RenderTexture exploredTexture;
    public RenderTexture miniMapOutput;

    [Header("Settings")]
    [Range(0.1f, 2f)] public float updateRate = 0.5f;
    public bool useCoroutine = true;

    private Texture2D _cachedTexture;
    private bool _isUpdating;

    IEnumerator Start()
    {
        // Инициализация с задержкой на первый кадр
        yield return null;

        if (miniMapOutput == null)
        {
            miniMapOutput = new RenderTexture(512, 512, 0, RenderTextureFormat.Default)
            {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm
            };
            miniMapOutput.Create();
        }

        _cachedTexture = new Texture2D(
            miniMapOutput.width,
            miniMapOutput.height,
            TextureFormat.RGBA32,
            false
        );

        if (useCoroutine)
            StartCoroutine(SafeUpdateMiniMap());
    }

    void Update()
    {
        if (!useCoroutine && !_isUpdating)
            StartCoroutine(UpdateSingleFrame());
    }

    IEnumerator SafeUpdateMiniMap()
    {
        while (true)
        {
            yield return StartCoroutine(UpdateSingleFrame());
            yield return new WaitForSeconds(1f / updateRate);
        }
    }

    IEnumerator UpdateSingleFrame()
    {
        if (_isUpdating || _cachedTexture == null)
            yield break;

        _isUpdating = true;

        // Переносим тяжелые операции на конец кадра
        yield return new WaitForEndOfFrame();

        try
        {
            RenderTexture.active = miniMapOutput;
            _cachedTexture.ReadPixels(new Rect(0, 0, miniMapOutput.width, miniMapOutput.height), 0, 0);
            _cachedTexture.Apply();

            // Оптимизированный пиксельный анализ (только каждую 4-ю строку/столбец)
            for (int y = 0; y < _cachedTexture.height; y += 4)
            {
                for (int x = 0; x < _cachedTexture.width; x += 4)
                {
                    Vector2 uv = new Vector2(
                        (float)x / _cachedTexture.width,
                        (float)y / _cachedTexture.height
                    );

                    Color fogPixel = SampleRT(fogTexture, uv);
                    Color exploredPixel = SampleRT(exploredTexture, uv);
                    Color originalPixel = _cachedTexture.GetPixel(x, y);

                    Color finalColor = GetFoggedColor(originalPixel, fogPixel, exploredPixel);
                    _cachedTexture.SetPixel(x, y, finalColor);
                }
                yield return null; // Разбиваем на кадры
            }

            _cachedTexture.Apply();
            Graphics.Blit(_cachedTexture, miniMapOutput);
        }
        finally
        {
            _isUpdating = false;
            RenderTexture.active = null;
        }
    }

    Color SampleRT(RenderTexture rt, Vector2 uv)
    {
        RenderTexture.active = rt;
        Texture2D tempTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tempTex.ReadPixels(new Rect(uv.x * rt.width, uv.y * rt.height, 1, 1), 0, 0);
        tempTex.Apply();
        return tempTex.GetPixel(0, 0);
    }

    Color GetFoggedColor(Color original, Color fog, Color explored)
    {
        if (fog.grayscale > 0.1f) return original;
        if (explored.grayscale > 0.1f) return Color.Lerp(original, Color.gray, 0.5f);
        return Color.black;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (_cachedTexture != null)
            DestroyImmediate(_cachedTexture);
    }
}