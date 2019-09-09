using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Uses a Valve OpenVR compatible set of procedures to grab and let go of objects, snapping, parenting, holding multiple objects, taking them away from other grabbers like your hands or other PickUpAndDrop scripts, restores original parent etc.
/// Most of the code here comes from Valve.VR.InteractionSystem.Hand
///
/// </summary>
public class PickUpAndDrop : Hand{// : MonoBehaviour {
	
	[SerializeField]
	Hand[] otherHandsThatCanGrabStuff;
	public static List<PickUpAndDrop> allPickUpAndDropScripts;

	[SerializeField]
	Transform attachmentTransform;

	private List<Hand.AttachedObject> attachedObjects = new List<Hand.AttachedObject>();

	new public ReadOnlyCollection<Hand.AttachedObject> AttachedObjects
	{
		get { return attachedObjects.AsReadOnly(); }
	}


	new public const Hand.AttachmentFlags defaultAttachmentFlags = Hand.AttachmentFlags.ParentToHand |
															  Hand.AttachmentFlags.DetachOthers |
															  Hand.AttachmentFlags.DetachFromOtherHand |
															  Hand.AttachmentFlags.SnapOnAttach;

	//public bool showDebugText = false;
	//public bool spewDebugText = false;

	// Use this for initialization
	new void Start () {
		if(allPickUpAndDropScripts == null)
			allPickUpAndDropScripts = new List<PickUpAndDrop>();
		allPickUpAndDropScripts.Add(this);

		StartCoroutine(base.Start());
	}
	
	new private void OnDestroy() {
		base.OnDestroy();
		allPickUpAndDropScripts.Remove(this);	
	}

	void LateUpdate()
	{

	}


	//-------------------------------------------------
	new private void OnInputFocus( bool hasFocus )
	{

	}


	//-------------------------------------------------
	new void FixedUpdate()
	{
	}


	//-------------------------------------------------
	new void OnDrawGizmos()
	{
	}


	private void CleanUpAttachedObjectStack()
	{
		attachedObjects.RemoveAll( l => l.attachedObject == null );
	}

	//-------------------------------------------------
	// Active GameObject attached to this Hand
	//-------------------------------------------------
	new public GameObject currentAttachedObject
	{
		get
		{
			CleanUpAttachedObjectStack();

			if ( attachedObjects.Count > 0 )
			{
				return attachedObjects[attachedObjects.Count - 1].attachedObject;
			}

			return null;
		}
	}

	//-------------------------------------------------
		// Detach this GameObject from the attached object stack of this Hand
		//
		// objectToDetach - The GameObject to detach from this Hand
		//-------------------------------------------------
	new public void DetachObject( GameObject objectToDetach, bool restoreOriginalParent = true )
	{
		int index = attachedObjects.FindIndex( l => l.attachedObject == objectToDetach );
		if ( index != -1 )
		{
			HandDebugLog( "DetachObject " + objectToDetach );

			GameObject prevTopObject = currentAttachedObject;

			Transform parentTransform = null;
			if ( attachedObjects[index].isParentedToHand )
			{
				if ( restoreOriginalParent && ( attachedObjects[index].originalParent != null ) )
				{
					parentTransform = attachedObjects[index].originalParent.transform;
				}
				attachedObjects[index].attachedObject.transform.parent = parentTransform;
			}

			attachedObjects[index].attachedObject.SetActive( true );
			attachedObjects[index].attachedObject.SendMessage( "OnDetachedFromHand", this, SendMessageOptions.DontRequireReceiver );
			//attachedObjects[index].attachedObject.SendMessage( "OnDetachedFromPUAD", this, SendMessageOptions.DontRequireReceiver );
			attachedObjects.RemoveAt( index );

			GameObject newTopObject = currentAttachedObject;

			//Give focus to the top most object on the stack if it changed
			if ( newTopObject != null && newTopObject != prevTopObject )
			{
				newTopObject.SetActive( true );
				newTopObject.SendMessage( "OnHandFocusAcquired", this, SendMessageOptions.DontRequireReceiver );
			}
		}

		CleanUpAttachedObjectStack();
	}
	
	//-------------------------------------------------
	public Transform GetAttachmentTransform( string attachmentPoint = "" )
	{
		//Transform attachmentTransform = null;

		if ( !string.IsNullOrEmpty( attachmentPoint ) )
		{
			attachmentTransform = transform.Find( attachmentPoint );
		}

		if ( !attachmentTransform )
		{
			attachmentTransform = this.transform;
		}

		return attachmentTransform;
	}


	//-------------------------------------------------
	// Attach a GameObject to this GameObject
	//
	// objectToAttach - The GameObject to attach
	// flags - The flags to use for attaching the object
	// attachmentPoint - Name of the GameObject in the hierarchy of this Hand which should act as the attachment point for this GameObject
	//-------------------------------------------------
	 public void AttachObject( GameObject objectToAttach, Hand.AttachmentFlags flags = defaultAttachmentFlags, string attachmentPoint = "" )
	{
		if ( flags == 0 )
		{
			flags = defaultAttachmentFlags;
		}

		//Make sure top object on stack is non-null
		CleanUpAttachedObjectStack();

		//Detach the object if it is already attached so that it can get re-attached at the top of the stack
		DetachObject( objectToAttach );

		if(( ( flags & Hand.AttachmentFlags.DetachFromOtherHand ) == Hand.AttachmentFlags.DetachFromOtherHand ))
		{
			//Detach from the other hand if requested
			foreach(Hand otherHand in otherHandsThatCanGrabStuff)
			{
				if ( otherHand )
				{
					otherHand.DetachObject( objectToAttach );
				}
			}
			foreach(PickUpAndDrop pUAD in allPickUpAndDropScripts){
				if ( pUAD && !pUAD.Equals(this))
				{
					pUAD.DetachObject( objectToAttach );
				}
			}
		}

		if ( ( flags & Hand.AttachmentFlags.DetachOthers ) == Hand.AttachmentFlags.DetachOthers )
		{
			//Detach all the objects from the stack
			while ( attachedObjects.Count > 0 )
			{
				DetachObject( attachedObjects[0].attachedObject );
			}
		}

		if ( currentAttachedObject )
		{
			currentAttachedObject.SendMessage( "OnHandFocusLost", this, SendMessageOptions.DontRequireReceiver );
		}

		Hand.AttachedObject attachedObject = new Hand.AttachedObject();
		attachedObject.attachedObject = objectToAttach;
		attachedObject.originalParent = objectToAttach.transform.parent != null ? objectToAttach.transform.parent.gameObject : null;
		if ( ( flags & Hand.AttachmentFlags.ParentToHand ) == Hand.AttachmentFlags.ParentToHand )
		{
			//Parent the object to the hand
			objectToAttach.transform.parent = GetAttachmentTransform( attachmentPoint );
			attachedObject.isParentedToHand = true;
		}
		else
		{
			attachedObject.isParentedToHand = false;
		}
		attachedObjects.Add( attachedObject );

		if ( ( flags & Hand.AttachmentFlags.SnapOnAttach ) == Hand.AttachmentFlags.SnapOnAttach )
		{
			objectToAttach.transform.localPosition = Vector3.zero;
			objectToAttach.transform.localRotation = Quaternion.identity;
		}

		HandDebugLog( "AttachObject " + objectToAttach );
		objectToAttach.SendMessage( "OnAttachedToHand", this, SendMessageOptions.DontRequireReceiver );
		//objectToAttach.SendMessage( "OnAttachedToPUAD", this, SendMessageOptions.DontRequireReceiver );

		//UpdateHovering();
	}

	/*
	//-------------------------------------------------
	private void UpdateHovering()
	{
		if ( ( noSteamVRFallbackCamera == null ) && ( controller == null ) )
		{
			return;
		}

		if ( hoverLocked )
			return;

		if ( applicationLostFocusObject.activeSelf )
			return;

		float closestDistance = float.MaxValue;
		Interactable closestInteractable = null;

		// Pick the closest hovering
		float flHoverRadiusScale = playerInstance.transform.lossyScale.x;
		float flScaledSphereRadius = hoverSphereRadius * flHoverRadiusScale;

		// if we're close to the floor, increase the radius to make things easier to pick up
		float handDiff = Mathf.Abs( transform.position.y - playerInstance.trackingOriginTransform.position.y );
		float boxMult = Util.RemapNumberClamped( handDiff, 0.0f, 0.5f * flHoverRadiusScale, 5.0f, 1.0f ) * flHoverRadiusScale;

		// null out old vals
		for ( int i = 0; i < overlappingColliders.Length; ++i )
		{
			overlappingColliders[i] = null;
		}

		Physics.OverlapBoxNonAlloc(
			hoverSphereTransform.position - new Vector3( 0, flScaledSphereRadius * boxMult - flScaledSphereRadius, 0 ),
			new Vector3( flScaledSphereRadius, flScaledSphereRadius * boxMult * 2.0f, flScaledSphereRadius ),
			overlappingColliders,
			Quaternion.identity,
			hoverLayerMask.value
		);

		// DebugVar
		int iActualColliderCount = 0;

		foreach ( Collider collider in overlappingColliders )
		{
			if ( collider == null )
				continue;

			Interactable contacting = collider.GetComponentInParent<Interactable>();

			// Yeah, it's null, skip
			if ( contacting == null )
				continue;

			// Ignore this collider for hovering
			IgnoreHovering ignore = collider.GetComponent<IgnoreHovering>();
			if ( ignore != null )
			{
				if ( ignore.onlyIgnoreHand == null || ignore.onlyIgnoreHand == this )
				{
					continue;
				}
			}

			// Can't hover over the object if it's attached
			if ( attachedObjects.FindIndex( l => l.attachedObject == contacting.gameObject ) != -1 )
				continue;

			// Occupied by another hand, so we can't touch it
			if ( otherHand && otherHand.hoveringInteractable == contacting )
				continue;

			// Best candidate so far...
			float distance = Vector3.Distance( contacting.transform.position, hoverSphereTransform.position );
			if ( distance < closestDistance )
			{
				closestDistance = distance;
				closestInteractable = contacting;
			}
			iActualColliderCount++;
		}

		// Hover on this one
		hoveringInteractable = closestInteractable;

		if ( iActualColliderCount > 0 && iActualColliderCount != prevOverlappingColliders )
		{
			prevOverlappingColliders = iActualColliderCount;
			HandDebugLog( "Found " + iActualColliderCount + " overlapping colliders." );
		}
	}
	*/

	private void HandDebugLog( string msg )
	{
		if ( spewDebugText )
		{
			Debug.Log( "Hand (" + this.name + "): " + msg );
		}
	}

}
