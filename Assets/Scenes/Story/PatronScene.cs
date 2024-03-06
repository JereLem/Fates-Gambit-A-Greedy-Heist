using System.Collections;
using TMPro;
using UnityEngine;

public class PatronScene : MonoBehaviour
{
    public TMP_Text adrianText;
    public TMP_Text patronText;

    private string[] adrianLines = {
        "Who are you? What do you want?",
        "And what's your interest in me, then?",
        "I’m listening...",
        "What's in it for me?",
        "I’ll play your game, for now. But don't think I won't be watching my back."
    };

    private string[] patronLines = {
        "I am a humble admirer of talent, Adrian. Someone who sees potential where others see only shadows.",
        "I have a proposition for you, Adrian. A chance to elevate your craft to new heights. Will you hear me out?",
        "Within lies an opportunity that awaits you at the grand masquerade. A taste of the intrigue that Eldoria conceals beneath its glittering surface.",
        "Power, Adrian. The power to unravel secrets and reshape destinies. But remember, every choice has its consequences.",
        "As you should, Adrian. As you should..."
    };

    private void Start()
    {
        StartCoroutine(AnimateDialogue());
    }

    IEnumerator AnimateDialogue()
    {
        for (int i = 0; i < Mathf.Max(adrianLines.Length, patronLines.Length); i++)
        {
            if (i < adrianLines.Length)
            {
                adrianText.text = adrianLines[i];
                adrianText.gameObject.SetActive(true);
                yield return new WaitForSeconds(5f);
                adrianText.gameObject.SetActive(false);
            }

            if (i < patronLines.Length)
            {
                patronText.text = patronLines[i];
                patronText.gameObject.SetActive(true);
                yield return new WaitForSeconds(5f);
                patronText.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
