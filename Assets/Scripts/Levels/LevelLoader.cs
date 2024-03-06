using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public float transitionTime = 1f;
    public TMP_Text skipText; // Reference to your skip text UI element
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        LoadNextLevel();

        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            // Activate the skip text
            if (skipText != null)
            {
                skipText.gameObject.SetActive(true);
            }

            // Check for space key press to force load the next level
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
