//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Destroys this object when it is detached from the hand
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class DestroyOnDetachedFromHand : MonoBehaviour
	{
		public float delayedDestroy = 0;
		Coroutine cor;
		//-------------------------------------------------
		private void OnDetachedFromHand( Hand hand )
		{
			if(cor!=null)
				StopCoroutine(cor);
			cor = StartCoroutine(DoActionAfterSeconds(delayedDestroy, ()=>{Destroy( gameObject );}));
			
		}

		IEnumerator DoActionAfterSeconds(float seconds, System.Action act){
			yield return new WaitForSeconds(seconds);
			act();

		}
	}
}
