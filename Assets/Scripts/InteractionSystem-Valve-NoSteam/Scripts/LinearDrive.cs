//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Drives a linear mapping based on position between 2 positions
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[ExecuteInEditMode]
	[RequireComponent( typeof( Interactable ) )]
	public class LinearDrive : MonoBehaviour
	{
		public bool makeSureSliderStaysInsideLineSegmentBounds = true;
		public bool autoKeepAttachedToHand = true;
		public Transform startPosition;
		public Transform endPosition;
		public LinearMapping linearMapping;
		public bool repositionGameObject = true;
		public bool maintainMomemntum = true;
		public float momemtumDampenRate = 5.0f;

        protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

        protected float initialMappingOffset;
        protected int numMappingChangeSamples = 5;
        protected float[] mappingChangeSamples;
        protected float prevMapping = 0.0f;
        protected float mappingChangeRate;
        protected int sampleCount = 0;

        protected Interactable interactable;

		public Transform followTargetObject;
		bool m_isGrabbed;
		Rigidbody rb;
		//[SerializeField]
		//float m_rigidBodySleepThreshold = 0.033f;
		//float m_distThisFrame;

        protected virtual void Awake()
        {
            mappingChangeSamples = new float[numMappingChangeSamples];
            interactable = GetComponent<Interactable>();
			rb = GetComponent<Rigidbody>();
			//rb.sleepThreshold = m_rigidBodySleepThreshold;
        }

        protected virtual void Start()
		{
			if ( linearMapping == null )
			{
				linearMapping = GetComponent<LinearMapping>();
			}

			if ( linearMapping == null )
			{
				linearMapping = gameObject.AddComponent<LinearMapping>();
			}

            initialMappingOffset = linearMapping.value;

			if ( repositionGameObject )
			{
				UpdateLinearMapping( transform );
			}
		}

        protected virtual void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                initialMappingOffset = linearMapping.value - CalculateLinearMapping( hand.transform );
				sampleCount = 0;
				mappingChangeRate = 0.0f;
				if(autoKeepAttachedToHand)
                	hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            }
		}

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            UpdateLinearMapping(hand.transform);

            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
				m_isGrabbed = false;
            }
			else{
				m_isGrabbed = true;
			}
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
			m_isGrabbed = false;
            CalculateMappingChangeRate();
        }


        protected void CalculateMappingChangeRate()
		{
			//Compute the mapping change rate
			mappingChangeRate = 0.0f;
			int mappingSamplesCount = Mathf.Min( sampleCount, mappingChangeSamples.Length );
			if ( mappingSamplesCount != 0 )
			{
				for ( int i = 0; i < mappingSamplesCount; ++i )
				{
					mappingChangeRate += mappingChangeSamples[i];
				}
				mappingChangeRate /= mappingSamplesCount;
			}
		}

        protected void UpdateLinearMapping( Transform updateTransform )
		{
			prevMapping = linearMapping.value;
			linearMapping.value = Mathf.Clamp01( initialMappingOffset + CalculateLinearMapping( updateTransform ) );

			mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = ( 1.0f / Time.deltaTime ) * ( linearMapping.value - prevMapping );
			sampleCount++;

			if ( repositionGameObject )
			{
				transform.position = Vector3.Lerp( startPosition.position, endPosition.position, linearMapping.value );
			}


		}

		//https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
		public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
		{
			//Get heading
			Vector3 heading = (end - origin);
			float magnitudeMax = heading.magnitude;
			heading.Normalize();

			//Do projection from the point but clamp it
			Vector3 lhs = point - origin;
			float dotP = Vector3.Dot(lhs, heading);
			dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
			return origin + heading * dotP;
		}

		//https://forum.unity.com/threads/how-to-check-a-vector3-position-is-between-two-other-vector3-along-a-line.461474/
		// Distance to point (p) from line segment (end points a b)
		float DistanceLineSegmentPoint( Vector3 a, Vector3 b, Vector3 p )
		{
			// If a == b line segment is a point and will cause a divide by zero in the line segment test.
			// Instead return distance from a
			if (a == b)
				return Vector3.Distance(a, p);
			
			// Line segment to point distance equation
			Vector3 ba = b - a;
			Vector3 pa = a - p;
			return (pa - ba * (Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba))).magnitude;
		}

        protected float CalculateLinearMapping( Transform updateTransform )
		{
			Vector3 direction = endPosition.position - startPosition.position;
			float length = direction.magnitude;
			direction.Normalize();

			Vector3 displacement = updateTransform.position - startPosition.position;

			return Vector3.Dot( displacement, direction ) / length;
		}

        
		protected virtual void Update()
        {
	
			
			if(Application.isPlaying)
					return;

			// if(followTargetObject!=null)
			// 	m_distThisFrame = Vector3.Distance(followTargetObject.position,transform.position);
			// else 
			// 	m_distThisFrame = m_rigidBodySleepThreshold;

            if ( maintainMomemntum && mappingChangeRate != 0.0f )
			{
				//Dampen the mapping change rate and apply it to the mapping
				mappingChangeRate = Mathf.Lerp( mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime );
				linearMapping.value = Mathf.Clamp01( linearMapping.value + ( mappingChangeRate * Time.deltaTime ) );

				if ( repositionGameObject )
				{
					transform.position = Vector3.Lerp( startPosition.position, endPosition.position, linearMapping.value );
				}
			}
			else if(m_isGrabbed == false && followTargetObject!=null 
				//&& m_distThisFrame >= m_rigidBodySleepThreshold && rb !=null
			){
				transform.position = followTargetObject.position;
				rb.Sleep();
			}


		}

		void LateUpdate(){
			if(makeSureSliderStaysInsideLineSegmentBounds){// && m_distThisFrame >= m_rigidBodySleepThreshold){
				Vector3 oldPos = transform.position;
				transform.position = FindNearestPointOnLine(startPosition.position, endPosition.position, transform.position);
				if(oldPos!=transform.position && rb !=null){
					rb.Sleep();	
				}
			}
		}

		#if UNITY_EDITOR
			protected virtual void OnDrawGizmos()
			{
				if(Application.isPlaying)
					return;
				Update();
			}
		#endif
	}
}
