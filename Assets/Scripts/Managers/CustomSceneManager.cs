using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    private static CustomSceneManager Instance;

    [SerializeField] private Animator _transitionAnimator;
    [SerializeField] private float transitionTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public static void LoadMainMenu(bool transition = false) => LoadSceneAtIndex(0, transition);

    public static void LoadGame(bool transition = false) => LoadSceneAtIndex(1, transition);

    public static void LoadEndGame(bool transition = false) => LoadSceneAtIndex(2, transition);

    public static void LoadNextScene(bool transition = false)
    {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextBuildIndex < SceneManager.sceneCount)
            LoadSceneAtIndex(nextBuildIndex, transition);
        else
            throw new System.Exception("CustomSceneManager ERROR : Cound not found next scene at index " + nextBuildIndex);
    }

    public static void LoadPreviusScene(bool transition = false) {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if (nextBuildIndex > 0)
            LoadSceneAtIndex(nextBuildIndex, transition);
        else
            throw new System.Exception("CustomSceneManager ERROR : There is no previous scene. BuildIndex must be positif > " + nextBuildIndex);
    }

    private static IEnumerator LoadSceneAtIndex(int buildIndex, bool hasTransition)
    {
        if (hasTransition)
        {
            Instance._transitionAnimator.SetBool("Transition", true);
            yield return new WaitForSeconds(Instance.transitionTime);
        }

        SceneManager.LoadScene(buildIndex);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Instance._transitionAnimator.SetBool("Transition", false);
    }
}
