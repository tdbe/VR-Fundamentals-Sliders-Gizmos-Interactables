using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSubscribeGizmosToEvents : MonoBehaviour
{
    [SerializeField]
    string m_optionalOnlyOnesWithThisTag = "";
    [SerializeField]
    GameObject m_findGizmosUnderThisParent;
    GLHandleVisuals_Group[] m_visualsGroups;
    [SerializeField]
    DoActionOnTrigger m_scriptToSubscribeTo;//this should be generic, how do you do this in unity inspector though?

    [SerializeField]
    bool m_subscribeTriggerEnterExitEvents = true;
    [SerializeField]
    bool m_subscribeEnableDisable = false;
    [SerializeField]
    bool m_disableRadiusTriggersOfTrackedObjects = false;

    [SerializeField]
    bool m_DisableInteractableAsWellAsGizmoVisuals = true;
    
    public class VisGroupInfo{
        public bool m_wasInteractableActive = true;
        public Valve.VR.InteractionSystem.Hand.AttachmentFlags origAttachmentFlags;
        public int origLayer;
        public int nothingLayer = 28;
    }
    VisGroupInfo[] m_VisGroupInfos;

    bool m_wasDisabled = false;

    void Awake(){
        m_visualsGroups = m_findGizmosUnderThisParent.GetComponentsInChildren<GLHandleVisuals_Group>();

        Init();
    }

    void Start(){
        
    }

    void Init(){
        int visGrpIdx = 0;
        m_VisGroupInfos = new VisGroupInfo[m_visualsGroups.Length];
        foreach(GLHandleVisuals_Group visGrp in m_visualsGroups){
            m_VisGroupInfos[visGrpIdx] = (new VisGroupInfo());
            if(m_DisableInteractableAsWellAsGizmoVisuals){
                visGrp.DisableVisuals();
                GLHandlesSteamVREventsAssigner ass = visGrp.GetComponent<GLHandlesSteamVREventsAssigner>();
                if(ass!=null){
                    if(ass.getInteractable!=null){
                        //m_VisGroupInfos[visGrpIdx].m_wasInteractableActive = ass.getInteractable.enabled;
                    }
                    if(ass.getThrowable!=null){
                        //m_VisGroupInfos[visGrpIdx].origAttachmentFlags = ass.getThrowable.attachmentFlags;
                    }
                    if(ass.getGameObject!=null){
                        m_VisGroupInfos[visGrpIdx].origLayer = ass.getGameObject.layer;
                        //Debug.Log("~~~~~~~~~~~~~~~~~~ m_VisGroupInfos[visGrpIdx{"+visGrpIdx+"}].origLayer: "+m_VisGroupInfos[visGrpIdx].origLayer+"; ass.getGameObject.name: "+ass.getGameObject.transform.parent.gameObject.name+"/"+ass.getGameObject.name);
                    }
                }
            }
            visGrpIdx++;
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if(!m_wasDisabled)
            return;

        int visGrpIdx = 0;
        foreach(GLHandleVisuals_Group visGrp in m_visualsGroups){
            if((m_optionalOnlyOnesWithThisTag=="" || visGrp.CompareTag(m_optionalOnlyOnesWithThisTag)) && m_subscribeTriggerEnterExitEvents){
                m_scriptToSubscribeTo.OnTriggerEnterEvent.AddListener(visGrp.EnableVisuals);
                m_scriptToSubscribeTo.OnTriggerExitEvent.AddListener(visGrp.DisableVisuals);
                if(m_subscribeEnableDisable){
                   visGrp.gameObject.SetActive(true);
                }
                if(m_disableRadiusTriggersOfTrackedObjects)
                    visGrp.GetComponent<Collider>().enabled = false;
                if(m_DisableInteractableAsWellAsGizmoVisuals){
                    GLHandlesSteamVREventsAssigner ass = visGrp.GetComponent<GLHandlesSteamVREventsAssigner>();
                    if(ass!=null){
                        if(ass.getInteractable!=null){
                            //ass.getInteractable.enabled = m_VisGroupInfos[visGrpIdx].m_wasInteractableActive;
                        }
                        if(ass.getThrowable!=null){
                            //ass.getThrowable.attachmentFlags = m_VisGroupInfos[visGrpIdx].origAttachmentFlags;
                        }
                        if(ass.getGameObject!=null){
                            ass.getGameObject.layer = m_VisGroupInfos[visGrpIdx].origLayer;
                        }
                    }
                }
            }
            visGrpIdx++;
        }
    }

    void OnDisable(){
        m_wasDisabled = true;
        int visGrpIdx = 0;
        foreach(GLHandleVisuals_Group visGrp in m_visualsGroups){
            if((m_optionalOnlyOnesWithThisTag=="" || visGrp.CompareTag(m_optionalOnlyOnesWithThisTag) ) && m_subscribeTriggerEnterExitEvents){
                m_scriptToSubscribeTo.OnTriggerEnterEvent.RemoveListener(visGrp.EnableVisuals);
                m_scriptToSubscribeTo.OnTriggerExitEvent.RemoveListener(visGrp.DisableVisuals);
                if(m_subscribeEnableDisable){
                    visGrp.gameObject.SetActive(false);
                }
                if(m_DisableInteractableAsWellAsGizmoVisuals){
                    GLHandlesSteamVREventsAssigner ass = visGrp.GetComponent<GLHandlesSteamVREventsAssigner>();
                    if(ass!=null){
                        if(ass.getInteractable!=null){
                            //ass.getInteractable.enabled = false;
                        }
                        if(ass.getThrowable!=null){
                            //ass.getThrowable.attachmentFlags = 0;
                        }
                        if(ass.getGameObject!=null){
                            ass.getGameObject.layer = m_VisGroupInfos[visGrpIdx].nothingLayer;
                            //Debug.Log("setting to nothing layer ~~~~~~~~~~~~~~~~~~ ass.getGameObject.layer: "+ass.getGameObject.layer+"; m_VisGroupInfos[visGrpIdx{"+visGrpIdx+"}].origLayer: "+m_VisGroupInfos[visGrpIdx].origLayer+"; ass.getGameObject.name: "+ass.getGameObject.transform.parent.gameObject.name+"/"+ass.getGameObject.name);

                        }

                    }
                }
            }
            visGrpIdx++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
