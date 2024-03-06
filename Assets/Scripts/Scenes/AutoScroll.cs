using System.Collections;
using UnityEngine;
using TMPro;

public class AutoScroll : MonoBehaviour
{
    float speed = 80f;
    float boundaryTextEnd = 2300.0f;

    RectTransform rectTransform;
    [SerializeField] TMP_Text mainText;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = mainText.rectTransform;
        StartCoroutine(AutoScrollText());
    }

    IEnumerator AutoScrollText()
    {
        Vector3 startPosition = rectTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / (boundaryTextEnd / speed);
            rectTransform.localPosition = Vector3.Lerp(startPosition, new Vector3(startPosition.x, boundaryTextEnd, startPosition.z), elapsedTime);
            yield return null;
        }
    }
}
