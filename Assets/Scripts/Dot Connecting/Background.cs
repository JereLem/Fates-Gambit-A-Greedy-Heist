using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    float duration = 5.0f;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ChangeBackgroundColor()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(45 / 255f, 45 / 255f, 45 / 255f);
        }
        StartCoroutine(ReturnOriginalColor(duration));
    }
    private IEnumerator ReturnOriginalColor(float duration)
    {
        yield return new WaitForSeconds(duration);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
        }
        Debug.Log("sprite color: " + spriteRenderers[0].color);
    }
}
