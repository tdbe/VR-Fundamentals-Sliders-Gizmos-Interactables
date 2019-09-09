using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndTurnOffDummy : MonoBehaviour {
    public float delayedFade = 7;
    public float fadeTime = 3;
    public string shaderParamFade1 = "_Darken";
    public string shaderParamFade2 = "_SeeThru";
    float m_origShaderParam1;
    float m_origShaderParam2;
    public
    SkinnedMeshRenderer objectRenderer;

    public bool startedToFade = false;
    Coroutine m_cor1; //This is the do action coroutine.. I had no idea based on the naming
    Coroutine m_cor2; //This is the fade coroutine.. again no idea
    //-------------------------------------------------

    void Start () {
        if (m_origShaderParam1 == 0)
            m_origShaderParam1 = objectRenderer.material.GetFloat (shaderParamFade1);
        if (m_origShaderParam2 == 0)
            m_origShaderParam2 = objectRenderer.material.GetFloat (shaderParamFade2);
    }

    private void OnEnable () {
        if (m_origShaderParam1 == 0)
            m_origShaderParam1 = objectRenderer.material.GetFloat (shaderParamFade1);
        if (m_origShaderParam2 == 0)
            m_origShaderParam2 = objectRenderer.material.GetFloat (shaderParamFade2);
        StartFadeTimer ();
    }

    private void StartFadeTimer () {
        startedToFade = false;
        objectRenderer.material.SetFloat (shaderParamFade1, m_origShaderParam1);
        objectRenderer.material.SetFloat (shaderParamFade2, m_origShaderParam2);
        if (m_cor1 != null)
            StopCoroutine (m_cor1);
        m_cor1 = StartCoroutine (DoActionAfterSeconds (delayedFade, () => { //Bit complicated action yes?
            startedToFade = true;
            if (m_cor2 != null)
                StopCoroutine (m_cor2);
            m_cor2 = StartCoroutine (FadeMaterial (fadeTime));
        }));
    }

    IEnumerator DoActionAfterSeconds (float seconds, System.Action act) {
        yield return new WaitForSeconds (seconds);
        act ();

    }

    IEnumerator FadeMaterial (float seconds) {
        float maxSeconds = seconds;

        while (seconds > 0.0f) {
            float fade = maxSeconds / seconds;
            objectRenderer.material.SetFloat (shaderParamFade1, m_origShaderParam1 * fade);
            objectRenderer.material.SetFloat (shaderParamFade2, m_origShaderParam1 * fade);
            seconds -= Time.deltaTime;
            yield return new WaitForEndOfFrame ();
        }
        objectRenderer.material.SetFloat (shaderParamFade1, 0);
        objectRenderer.material.SetFloat (shaderParamFade2, 0);
        startedToFade = false;
        gameObject.SetActive (false);
    }

    public void ResetVaues () { //This does not actually reset since it sets the gameobject active
        if (m_cor1 != null)
            StopCoroutine (m_cor1);
        if (m_cor2 != null)
            StopCoroutine (m_cor2);

        startedToFade = false;
        gameObject.SetActive (true);
        objectRenderer.material.SetFloat (shaderParamFade1, m_origShaderParam1);
        objectRenderer.material.SetFloat (shaderParamFade2, m_origShaderParam2);
    }
}