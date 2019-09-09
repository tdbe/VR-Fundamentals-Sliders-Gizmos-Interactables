using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WavesPropagator : MonoBehaviour {

    [SerializeField]
    Transform[] m_waveObjects; 
    Queue<Transform> m_wavePool;
    int m_wavePoolCount;

    //[SerializeField]
    Vector3 m_waveDirection;
    [SerializeField]
    float m_waveSpeed;
    Vector3 m_waveLookAt;

    [SerializeField]
    Transform m_waveStartPos;
    [SerializeField]
    Transform m_waveEndPos;
    [SerializeField]
    Transform m_waveFadePos;
    float m_distStartToFadePos;
    float m_distStartToEnd;

    [SerializeField]
    AnimationCurve m_diveAnimCurve;
    [SerializeField]
    AnimationCurve m_riseAnimCurve;
    [SerializeField]
    float m_diveRate;

    // Use this for initialization
    void Start ()
    {
        m_waveDirection = (m_waveEndPos.position - m_waveStartPos.position).normalized;
        m_waveLookAt = m_waveDirection.normalized * 999999;
        m_distStartToFadePos = Vector3.Distance(m_waveStartPos.position, m_waveFadePos.position);
        m_distStartToEnd = Vector3.Distance(m_waveStartPos.position, m_waveEndPos.position);
        m_wavePool = new Queue<Transform>();
		//for(int i = m_waveObjects.Length-1; i >= 0; i--)
		for(int i = 0; i < m_waveObjects.Length; i++)
        {
            m_waveObjects[i].LookAt(m_waveLookAt);
            m_wavePool.Enqueue(m_waveObjects[i]);
        }
        m_wavePoolCount = m_wavePool.Count;

    }
	
	// Update is called once per frame
	void Update ()
    {
        
        int i = 0;
        Vector3 initialDive = new Vector3(0, -1.5f, 0);

        //NOTE: lesson: if you change the contents of a generic (enumerator-able) list or queue (collection), the enumerator gets invalidated.
        // This means a foreach will fail if the collection iterator got invalidated, or if you use an IEnumerator.MoveNext manually.
        //foreach (Transform trans in m_wavePool.TakeWhile)
        IEnumerator enumerator = m_wavePool.GetEnumerator();
        while (m_wavePool.Count>0)
        {
            Transform trans = null;
            //if (i >= m_wavePool.Count - 1)
            if (i <= m_wavePoolCount - 1 &&
                enumerator.MoveNext())
            {
                trans = (Transform)enumerator.Current;
            }
            else
            {
                break;
            }

            Vector3 diveAmount = Vector3.zero;
            float dst = Vector3.Distance(trans.position, m_waveStartPos.position);
            if (dst > m_distStartToFadePos)
            {
                diveAmount.y = m_diveAnimCurve.Evaluate((m_waveFadePos.position.z - trans.position.z))*m_diveRate;
            }
            else if (dst < 12.5f)
            {
                diveAmount.y = -initialDive.y + m_riseAnimCurve.Evaluate((m_waveFadePos.position.z - trans.position.z)) * m_diveRate;

            }

            trans.position += (m_waveDirection + diveAmount) * m_waveSpeed * Time.deltaTime;
#if UNITY_EDITOR
            m_waveDirection = (m_waveEndPos.position - m_waveStartPos.position).normalized;
            m_waveLookAt = m_waveDirection.normalized * 999999;
            m_distStartToEnd = Vector3.Distance(m_waveStartPos.position, m_waveEndPos.position);
            m_distStartToFadePos = Vector3.Distance(m_waveStartPos.position, m_waveFadePos.position);
            trans.LookAt(m_waveLookAt);
#endif

            i++;
        }

        Transform t0 = m_wavePool.Peek();
        float dist = Vector3.Distance(t0.position, m_waveStartPos.position);
        if (dist > m_distStartToEnd)
        {
            t0.position = m_waveStartPos.position + initialDive;
            m_wavePool.Enqueue(m_wavePool.Dequeue());
        }

    }
}
