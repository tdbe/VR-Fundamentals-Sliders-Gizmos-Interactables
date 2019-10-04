//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Basic throwable object
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	[RequireComponent( typeof( Rigidbody ) )]
    [RequireComponent( typeof(VelocityEstimator))]
	public class Throwable : MonoBehaviour
	{
        public bool hoverLockIfGrabbed = true;
        public bool autoGrabObject = false;
        [Header("Only let go on click (toggle style behaviour), instead of let go on input up.")]
        public bool dontLetGoOnInputUp = false;
        public void DontLetGoOnInputUp(bool val){
            dontLetGoOnInputUp = val;
        }
        [Header("While this is true, this object will not let itself go from the hand. However the hand can still be commanded to let go of all objects it's holding.")]
        public bool neverLetGo = false; 
        public void NeverLetGo(bool val){
            neverLetGo = val;
        }
        
        int inputUpIgnored;
        GrabTypes grabTypeIfKnown = GrabTypes.None;
        public void SetGrabType(GrabTypes type){
            grabTypeIfKnown = type;
        }
        

        //public bool autoLetGoIfAutoGrabbed = true;
        Hand lastUsedHand;
        [Space(15)]
        [EnumFlags]
		[Tooltip( "The flags used to attach this object to the hand." )]
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
        public Transform attachmentOffset;

		[Tooltip( "How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)" )]
        public float catchingSpeedThreshold = -1;

        public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

        [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
        public float releaseVelocityTimeOffset = -0.011f;

        public float scaleReleaseVelocity = 1.1f;

		[Tooltip( "When detaching the object, should it return to its original parent?" )]
		public bool restoreOriginalParent = false;

        

		protected VelocityEstimator velocityEstimator;
        protected bool attached = false;
        protected float attachTime;
        protected Vector3 attachPosition;
        protected Quaternion attachRotation;
        protected Transform attachEaseInTransform;

		public UnityEvent onPickUp;
        public UnityEvent onDetachFromHand;
        public UnityEvent<Hand> onHeldUpdate;

        
        protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        protected new Rigidbody rigidbody;

        [HideInInspector]
        public Interactable interactable;


        //-------------------------------------------------
        protected virtual void Awake()
		{
			velocityEstimator = GetComponent<VelocityEstimator>();
            interactable = GetComponent<Interactable>();



            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 50.0f;


            if(attachmentOffset != null)
            {
                // remove?
                //interactable.handFollowTransform = attachmentOffset;
            }

		}


        //-------------------------------------------------
        protected virtual void OnHandHoverBegin( Hand hand )
		{
			bool showHint = false;

            // "Catch" the throwable by holding down the interaction button instead of pressing it.
            // Only do this if the throwable is moving faster than the prescribed threshold speed,
            // and if it isn't attached to another hand
            if ( !attached && catchingSpeedThreshold != -1)
            {
                float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

                GrabTypes bestGrabType = hand.GetBestGrabbingType();

                if ( bestGrabType != GrabTypes.None )
				{
					if (rigidbody.velocity.magnitude >= catchingThreshold)
					{
						hand.AttachObject( gameObject, bestGrabType, attachmentFlags );
						showHint = false;
					}
				}
			}
            // automatically grab any object if you were holding down the trigger before hovering the object.
            else if(!attached && autoGrabObject){
                GrabTypes bestGrabType = hand.GetBestGrabbingType();
                if ( bestGrabType != GrabTypes.None )
				{
                    hand.AttachObject( gameObject, bestGrabType, attachmentFlags );
                    showHint = false;
                }
            }

			if ( showHint )
			{
                hand.ShowGrabHint();
			}
		}



        public void ManuallyDetatchThisObject(){
            if(lastUsedHand!=null){
                //Debug.Log("ManuallyDetatchThisObject: "+lastUsedHand);

                lastUsedHand.DetachObject(gameObject);
                lastUsedHand = null;
                //OnDetachedFromHand(lastUsedHand);
                //OnHandHoverEnd( lastUsedHand );
            }
        }


        /*
        public void ManuallyDetatchThisObject(Hand hand ){
            lastUsedHand = hand;
            ManuallyDetatchThisObject();
        }*/
/*
        void Update(){
            rigidbody.isKinematic = true;
        }
*/
        //-------------------------------------------------
        protected virtual void OnHandHoverEnd( Hand hand )
		{
            // if(autoLetGoIfAutoGrabbed && autoGrabObject){
            //     ManuallyDetatchThisObject(hand);
            // }
            if(hand!=null)
                hand.HideGrabHint();
            
		}


        //-------------------------------------------------
        protected virtual void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            
            if (startingGrabType != GrabTypes.None)
            {
				hand.AttachObject( gameObject, startingGrabType, attachmentFlags, attachmentOffset );
                hand.HideGrabHint();
            }
		}

        //-------------------------------------------------
        protected virtual void OnAttachedToHand( Hand hand )
		{
            //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());
            lastUsedHand = hand;


            hadInterpolation = this.rigidbody.interpolation;

            attached = true;

			onPickUp.Invoke();

            if (!hoverLockIfGrabbed)
                hand.HoverUnlock(null);
            else
                hand.HoverLock( null );
            
            rigidbody.interpolation = RigidbodyInterpolation.None;
            
		    velocityEstimator.BeginEstimatingVelocity();

			attachTime = Time.time;
			attachPosition = transform.position;
			attachRotation = transform.rotation;
            if(hand.m_IndexFingerCollider!=null)
                hand.m_IndexFingerCollider.enabled = false;

            if(dontLetGoOnInputUp)
            {
                inputUpIgnored = 0;
            }
		}


        //-------------------------------------------------
        protected virtual void OnDetachedFromHand(Hand hand)
        {
            if(!attached)
                return;
                
            attached = false;
            lastUsedHand = null;

            onDetachFromHand.Invoke();

            hand.HoverUnlock(null);
            
            rigidbody.interpolation = hadInterpolation;

            Vector3 velocity;
            Vector3 angularVelocity;

            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
            if(hand.m_IndexFingerCollider!=null)
                hand.m_IndexFingerCollider.enabled = true;
            
            if(dontLetGoOnInputUp)
            {
                inputUpIgnored = 0;
            }

            grabTypeIfKnown = GrabTypes.None;
        }


        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
                releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

            switch (releaseVelocityStyle)
            {
                case ReleaseStyle.ShortEstimation:
                    velocityEstimator.FinishEstimatingVelocity();
                    velocity = velocityEstimator.GetVelocityEstimate();
                    angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                    break;
                case ReleaseStyle.AdvancedEstimation:
                    hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                    break;
                case ReleaseStyle.GetFromHand:
                    velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                    angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                    break;
                default:
                case ReleaseStyle.NoChange:
                    velocity = rigidbody.velocity;
                    angularVelocity = rigidbody.angularVelocity;
                    break;
            }

            if (releaseVelocityStyle != ReleaseStyle.NoChange)
                velocity *= scaleReleaseVelocity;
        }

        bool GetGrabPinchInputClicked(Hand hand){
            bool inputForGrab = OculusInputManager.Instance.GetGrabPinchClick(OculusInputManager.Instance.GetOculusHand(hand.handType))
            ||
            OculusInputManager.Instance.GetGrabGripClick(OculusInputManager.Instance.GetOculusHand(hand.handType));
            return inputForGrab;
        }

        //-------------------------------------------------
        protected virtual void HandAttachedUpdate(Hand hand)
        {
            //int index = hand.attachedObjects.FindIndex(l => l.attachedObject == objectToDetach);
            //if (index != -1)
            bool inputForGrab = GetGrabPinchInputClicked(hand);

            if(neverLetGo){

            }
            else
            if(dontLetGoOnInputUp && inputForGrab && 
                ((grabTypeIfKnown == GrabTypes.Scripted || grabTypeIfKnown == GrabTypes.Trigger) || inputUpIgnored > 0))
            {               
                hand.DetachObject(gameObject, restoreOriginalParent);
                //Debug.Log("Detatching: "+gameObject.name);
            }
            else
            if ( hand.IsGrabEnding(this.gameObject, dontLetGoOnInputUp))
            {
                //Debug.Log("Detatching2: "+gameObject.name);

                hand.DetachObject(gameObject, restoreOriginalParent);

                // Uncomment to detach ourselves late in the frame.
                // This is so that any vehicles the player is attached to
                // have a chance to finish updating themselves.
                // If we detach now, our position could be behind what it
                // will be at the end of the frame, and the object may appear
                // to teleport behind the hand when the player releases it.
                //StartCoroutine( LateDetach( hand ) );
            }

            if(dontLetGoOnInputUp && inputForGrab)
                inputUpIgnored++;

            if (onHeldUpdate != null)
                onHeldUpdate.Invoke(hand);
        }


        //-------------------------------------------------
        protected virtual IEnumerator LateDetach( Hand hand )
		{
			yield return new WaitForEndOfFrame();

			hand.DetachObject( gameObject, restoreOriginalParent );
		}


        //-------------------------------------------------
        protected virtual void OnHandFocusAcquired( Hand hand )
		{
			gameObject.SetActive( true );
			velocityEstimator.BeginEstimatingVelocity();
		}


        //-------------------------------------------------
        protected virtual void OnHandFocusLost( Hand hand )
		{
			gameObject.SetActive( false );
			velocityEstimator.FinishEstimatingVelocity();
		}
	}

    public enum ReleaseStyle
    {
        NoChange,
        GetFromHand,
        ShortEstimation,
        AdvancedEstimation,
    }
}
