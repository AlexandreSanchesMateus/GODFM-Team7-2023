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

    public static void LoadMainMenu(bool transition = false) => Instance.StartCoroutine(LoadSceneAtIndex(0, transition));

    public static void LoadGame(bool transition = false) => Instance.StartCoroutine(LoadSceneAtIndex(1, transition));

    public static void LoadEndGame(bool transition = false) => Instance.StartCoroutine(LoadSceneAtIndex(2, transition));

    public static void LoadNextScene(bool transition = false)
    {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextBuildIndex < SceneManager.sceneCount)
            Instance.StartCoroutine(LoadSceneAtIndex(nextBuildIndex, transition));
        else
            throw new System.Exception("CustomSceneManager ERROR : Cound not found next scene at index " + nextBuildIndex);
    }

    public static void LoadPreviusScene(bool transition = false) {
        int nextBuildIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if (nextBuildIndex > 0)
            Instance.StartCoroutine(LoadSceneAtIndex(nextBuildIndex, transition));
        else
            throw new System.Exception("CustomSceneManager ERROR : There is no previous scene. BuildIndex must be positif > " + nextBuildIndex);
    }

    private static IEnumerator LoadSceneAtIndex(int buildIndex, bool hasTransition)
    {
        Debug.Log("azert");

        if (hasTransition)
        {
            Instance._transitionAnimator.SetBool("Transition", true);
            yield return new WaitForSeconds(Instance.transitionTime);
        }

        Debug.Log("2");
        SceneManager.LoadScene(buildIndex);
        Debug.Log("3");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Instance._transitionAnimator.SetBool("Transition", false);
    }
}
