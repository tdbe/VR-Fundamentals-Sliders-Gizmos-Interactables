//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Destroys this object when it enters a trigger
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class DestroyObject : MonoBehaviour
	{
		public string info = "DestroyGameObject(GameObject gameObject) - if gameObject is null, destroys this.gameObject";

		//-------------------------------------------------
		void Start()
		{
			
		}


		//-------------------------------------------------
		public void DestroyGameObject(GameObject gameObject){
			if(gameObject==null)
				Destroy(this.gameObject);
			else
				Destroy(gameObject);			
		}
	}
}
