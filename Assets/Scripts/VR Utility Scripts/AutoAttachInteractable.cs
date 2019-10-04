//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Spawns and attaches an object to the hand after the controller has
//			tracking
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

	//-------------------------------------------------------------------------
	public class AutoAttachInteractable : MonoBehaviour
	{
		[SerializeField]
		private Hand hand;
        //[SerializeField]
        //bool m_forceKeepAttached;

        public UnityEvent OnAutoAttachCalled;
       
        Hand.AttachmentFlags att;
        Throwable thr;
		//-------------------------------------------------
		void Start()
		{
			//hand = GetComponentInParent<Hand>();
            thr = GetComponent<Throwable>();
            
            if(thr == null)
                att = Hand.AttachmentFlags.ParentToHand;
            else
                att = thr.attachmentFlags;
		}

        public void AutoAttachToHand(Hand hand){
            this.hand = hand;
            if(OnAutoAttachCalled!=null)
                OnAutoAttachCalled.Invoke();
            this.enabled = (true);
        }

		//-------------------------------------------------
		void Update()
		{
			
                if (hand.isActive)// && hand.isPoseValid)
                {


                    //GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
                    //objectToAttach.SetActive(true);
                    
             
                    hand.AttachObject(gameObject, GrabTypes.Scripted, att);
                    if(thr!=null)
                        thr.SetGrabType(GrabTypes.Scripted);
                    //hand.AttachObject(gameObject, GrabTypes.Pinch);
                    //hand.TriggerHapticPulse(800);
    
                    
                    // If the player's scale has been changed the object to attach will be the wrong size.
                    // To fix this we change the object's scale back to its original, pre-attach scale.
                    //objectToAttach.transform.localScale = item.transform.localScale;

                    //Destroy(this);
                    //if(!m_forceKeepAttached)
                        this.enabled = (false);
                }
			
		}
        /*
        void HandHoverUpdate( Hand hand ){
            
        }*/
	}

