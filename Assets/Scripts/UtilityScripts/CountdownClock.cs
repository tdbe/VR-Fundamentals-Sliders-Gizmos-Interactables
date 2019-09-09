using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountdownClock : MonoBehaviour {
    public bool active = true;
    public bool updatedManually = false;
    float m_manualIncrement;
    Queue<float> m_manualTimeIncrements;
    [SerializeField]
    Text m_text;
    [SerializeField]
    MeshRenderer[] m_background;
    Color startColor;
    [SerializeField]
    float m_colorScale = .26f;
    public float TotalSeconds = 900; //15 minutes;
    public bool directionForward = false;
    public bool stopWhenLimitReached = false;

    public float changeTextColorAtTime = -1;
    private float defaultChangeTextColorAtTime;
    public Color newTextColor = Color.green;
    public Color originalTextColor;
    public float currentTimeSeconds;

    public UnityEvent OnTimerGoalReached;
    public UnityEvent OnTimerFullyComplete;
    public bool m_FinishedSanitize;

    Vector3 origScale;

    void Start () {
        startColor = m_background[0].material.color;
        originalTextColor = m_text.color;
        origScale = transform.localScale;
        defaultChangeTextColorAtTime = changeTextColorAtTime;
        Init ();
    }

    void Init () {
        transform.localScale = origScale;
        m_manualTimeIncrements = new Queue<float> ();
        m_FinishedSanitize = false;
        m_text.color = originalTextColor;
        changeTextColorAtTime = defaultChangeTextColorAtTime;
        if (!directionForward)
            currentTimeSeconds = TotalSeconds;
        else
            currentTimeSeconds = 0;

    }

    public void Fade(){
        transform.localScale *=.5f;
    }

    public void ResetAndDisable () {
        Init ();
        active = true;
        gameObject.SetActive (false);
    }

    private void Update () {
        if (!active)
            return;

        if (!updatedManually) {
            AutoUpdate ();
        } else {
            ManualUpdate ();
        }

        if (changeTextColorAtTime >= 0) {
            if ( (directionForward && currentTimeSeconds >= changeTextColorAtTime)||
                (!directionForward && currentTimeSeconds <= changeTextColorAtTime)
            ) {
                changeTextColorAtTime = -1;
                m_text.color = newTextColor;
                if (OnTimerGoalReached != null)
                    m_FinishedSanitize = true;
                OnTimerGoalReached.Invoke ();
            }
        }

        UpdateLevelTimer (currentTimeSeconds);
    }

    private void ManualUpdate () {
        if (m_manualTimeIncrements.Count == 0)
            return;

        float incrementMax = m_manualTimeIncrements.Peek ();
        m_manualIncrement += Time.deltaTime;
        if (m_manualIncrement >= incrementMax) {
            //m_manualIncrement=incrementMax;
            m_manualIncrement = 0;
            m_manualTimeIncrements.Dequeue ();
        } else {
            currentTimeSeconds += Time.deltaTime;
        }
    }

    void AutoUpdate () {
        if (!directionForward) {
            currentTimeSeconds -= Time.deltaTime;
            if (stopWhenLimitReached && currentTimeSeconds <= 0) {
                currentTimeSeconds = 0;
                active = false;
                if (OnTimerFullyComplete != null)
                    OnTimerFullyComplete.Invoke ();
            }
        } else {
            currentTimeSeconds += Time.deltaTime;
            if (stopWhenLimitReached && currentTimeSeconds >= TotalSeconds) {
                currentTimeSeconds = TotalSeconds;
                active = false;
                if (OnTimerFullyComplete != null)
                    OnTimerFullyComplete.Invoke ();
            }
        }
    }

    public void SetSecondsManually (float time) {
        //if(time > currentTimeSeconds){
        //}
        //currentTimeSeconds += time;
        m_manualTimeIncrements.Enqueue (time);
    }

    void UpdateLevelTimer (float totalSeconds) { //Naming the parameter totalSeconds is very confusing as it is actually the currentSeconds being passed into it
        if (!active)
            return;

        int minutes = Mathf.FloorToInt (totalSeconds / 60f);
        int seconds = Mathf.RoundToInt (totalSeconds % 60f);

        string formatedSeconds = seconds.ToString ();

        if (seconds == 60) {
            seconds = 0;
            minutes += 1;
        }

        m_text.text = minutes.ToString ("00") + ":" + seconds.ToString ("00");

        if (currentTimeSeconds >= 0 && currentTimeSeconds <= TotalSeconds) {
            float h, s, v;
            Color.RGBToHSV (startColor, out h, out s, out v);
            float slider = 0;
            if (!directionForward)
                slider = (1 - currentTimeSeconds / TotalSeconds); // 0 to 1
            else
                slider = (currentTimeSeconds / TotalSeconds);
            slider *= m_colorScale;
            if (!directionForward)
                h -= slider;
            else
                h += slider;

            foreach (MeshRenderer img in m_background)
                img.material.color = Color.HSVToRGB (h, s, v);
        }
    }
}