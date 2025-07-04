using System.Collections;
using UnityEngine;

public class FogProjector : MonoBehaviour
{
    public Material projectorMaterial;
    public float blendSpeed;
    public int textureScale;

    public RenderTexture fogTexture;

    private RenderTexture prevTexture;
    private RenderTexture currTexture;
    private Projector projector;

    private float blendAmount;

    private void Awake()
    {
        projector = GetComponent<Projector>();
        projector.enabled = true;

        prevTexture = GenerateTexture();
        currTexture = GenerateTexture();

        // Материалы проектора не инстанцированы, поэтому ресурс материала изменяется.
        // Инстанцируем его здесь, чтобы нам не приходилось вручную вносить или отменять эти изменения.
        projector.material = new Material(projectorMaterial);

        projector.material.SetTexture("_PrevTexture", prevTexture);
        projector.material.SetTexture("_CurrTexture", currTexture);
        StartNewBlend();
    }

    RenderTexture GenerateTexture()
    {
        RenderTexture rt = new RenderTexture(
        fogTexture.width * textureScale,
        fogTexture.height * textureScale,
        0,
        fogTexture.format)
        { filterMode = FilterMode.Bilinear };
        rt.antiAliasing = fogTexture.antiAliasing;

        return rt;
    }

    public void StartNewBlend()
    {
        StopCoroutine(BlendFog());
        blendAmount = 0;
        // Меняем текстуры местами
        Graphics.Blit(currTexture, prevTexture);
        Graphics.Blit(fogTexture, currTexture);
        StartCoroutine(BlendFog());
    }

    IEnumerator BlendFog()
    {
        while (blendAmount < 1)
        {
            // увеличиваем степень интерполяции
            blendAmount += Time.deltaTime * blendSpeed;
            // Устанавливаем свойство blend, чтобы шейдер знал, насколько нужно интерполировать
            // при проверке значения альфа
            projector.material.SetFloat("_Blend", blendAmount);
            yield return null;
        }
        // после завершения смешивания поменяйте текстуры местами и начните новое смешивание
        StartNewBlend();
    }
}