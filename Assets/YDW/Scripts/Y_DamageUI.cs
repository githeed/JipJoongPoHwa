using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Y_DamageUI : MonoBehaviour
{
    // 모델 바디 컬러
    private Renderer[] renderers;
    private Color originalColor;
    public Color hitColor = new Color(1f, 0f, 0f, 0.5f);
    public float colorChangeDuration = 0.5f;

    // 화면 가장자리에 적색 비네팅
    public Image VigImage;
    float currTime;
    public float vigDuration = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponents<SkinnedMeshRenderer>();

        // Store the original color
        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator ChangeColorTemporarily()
    {
        // Change to the hit color
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = hitColor;
            }
        }

        // Wait for the duration
        yield return new WaitForSeconds(colorChangeDuration);

        // Change back to the original color
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }
    }

    public IEnumerator RedVignette()
    {
        VigImage.gameObject.SetActive(true);
        while (currTime < colorChangeDuration / 2)
        {
            currTime += Time.deltaTime;

            float p = currTime / colorChangeDuration;
            p = easeOutExpo(p);
            float alpha = Mathf.Lerp(0f, 1f, p);
            SetAlpha(alpha);

            yield return null;
        }
        SetAlpha(1f);

        while (currTime < colorChangeDuration / 2)
        {
            currTime += Time.deltaTime;

            float p = currTime / colorChangeDuration;
            p = easeOutExpo(p);
            float alpha = Mathf.Lerp(1f, 0f, p);
            SetAlpha(alpha);

            yield return null;
        }
        SetAlpha(0f);
        currTime = 0;
        VigImage.gameObject.SetActive(false);

    }

    public void SetAlpha(float alpha)
    {
        Color vigColor = VigImage.color;

        vigColor.a = alpha;

        VigImage.color = vigColor;
    }

    public float easeOutExpo(float x)
    {
        return x == 1 ? 1 : 1 - (float)Mathf.Pow(2, -10 * x);
    }
}
