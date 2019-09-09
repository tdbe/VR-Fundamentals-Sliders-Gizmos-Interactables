using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedSceneChange : MonoBehaviour {

    Coroutine m_SceneChangeRoutine;

    public void SceneChange (float delay) {
        m_SceneChangeRoutine = StartCoroutine (SceneChangeWithDelay (delay));
    }

    private IEnumerator SceneChangeWithDelay (float delay) {
        yield return new WaitForSeconds (delay);
        SceneManager.LoadScene(0);
    }
}