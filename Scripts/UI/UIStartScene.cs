using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStartScene : MonoBehaviour
{
    public string TargetScene;
    AsyncOperation _sceneLoading;
    [SerializeField] Slider _loadingBar;
    [SerializeField] GameObject _btn;

    public void StartBtn()
    {
        StartCoroutine(Loading());
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public IEnumerator Loading()
    {
        _sceneLoading = SceneManager.LoadSceneAsync(TargetScene);
        _btn.SetActive(false);
        _loadingBar.gameObject.SetActive(true);

        while (!_sceneLoading.isDone)
        {
            LoadVisible(_sceneLoading);
            yield return null;
        }
    }

    public void LoadVisible(AsyncOperation load)
    {
        _loadingBar.value = load.progress;
    }
}
