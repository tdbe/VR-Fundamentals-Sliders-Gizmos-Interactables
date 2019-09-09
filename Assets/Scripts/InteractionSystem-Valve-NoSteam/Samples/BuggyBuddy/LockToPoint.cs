using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Valve.VR.InteractionSystem.Sample
{
    public class LockToPoint : MonoBehaviour
    {
        public bool disableOnStart;
        public Transform snapTo;
        public bool snapToOriginalPositionInsteadOfSnapTo;
        public bool useParentPositionInsteadOfOriginalPosition = true;
        //public bool turnSelfOffWhenSnapComplete;
        Vector3 originalPos;
        //Vector3 originalPosOfParent;
        public Vector3 originalPosRelative;
        //Quaternion originalRotParent;
        public Quaternion originalRotRelative;
        Quaternion originalRot;
        private Rigidbody body;

        public float waitBeforeSnapping = 0;
        float snapStartTime;
        public float snapTime = 2;

        public float dropTimer;
        private Interactable interactable;
        bool wasKinematic;

        
        private void Start()
        {
            if (snapTo == null)
                snapToOriginalPositionInsteadOfSnapTo = true;
            if (snapToOriginalPositionInsteadOfSnapTo)
            {
                originalPos = transform.position;
                originalRot = transform.rotation;
            }
            if (useParentPositionInsteadOfOriginalPosition)
            {
                //originalPosOfParent = transform.parent !=null ? transform.parent.position : transform.position;
                originalPosRelative = transform.localPosition;
                originalRotRelative = transform.localRotation;
            }
            interactable = GetComponent<Interactable>();
            body = GetComponent<Rigidbody>();
            wasKinematic = body.isKinematic;

            if(disableOnStart){
                this.enabled = false;
            }
        }
        
        private void FixedUpdate()
        {
            bool used = false;
            if (interactable != null){
                //Debug.Log("attached to hand");
                used = interactable.attachedToHand;
            }

            if (used)
            {
                //Debug.Log("if used");
                //body.isKinematic = false;
                wasKinematic = body.isKinematic;
                dropTimer = -1;
                snapStartTime = Time.time;
            }
            else
            {
                
                body.isKinematic = dropTimer > 1 ?  true : wasKinematic;
                if (Time.time - snapStartTime > waitBeforeSnapping)
                {

                    dropTimer += Time.deltaTime / (snapTime / 2);

                    

                    if (dropTimer > 1)
                    {
                        
                        //Debug.Log("else 1");
                        if (snapToOriginalPositionInsteadOfSnapTo)
                        {
                            if (useParentPositionInsteadOfOriginalPosition)
                            {
                                transform.localPosition = originalPosRelative;
                                transform.localRotation = originalRotRelative;
                            }
                            else{
                                //transform.parent = snapTo;
                                transform.position = originalPos;
                                transform.rotation = originalRot;
                            }
                        }
                        else
                        {
                            //transform.parent = snapTo;
                            transform.position = snapTo.position;
                            transform.rotation = snapTo.rotation;
                        }
                    }
                    else
                    {
                        //Debug.Log("else 2");
                        float t = Mathf.Pow(35, dropTimer);

                        body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.fixedDeltaTime * 4);
                        if (body.useGravity)
                            body.AddForce(-Physics.gravity);

                        if (snapToOriginalPositionInsteadOfSnapTo)
                        {
                            if (useParentPositionInsteadOfOriginalPosition)
                            {
                                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosRelative, Time.fixedDeltaTime * t * 3);
                                transform.localRotation = Quaternion.Slerp(transform.rotation, originalRotRelative, Time.fixedDeltaTime * t * 3.5f);
                            }
                            else{
                                transform.position = Vector3.Lerp(transform.position, originalPos, Time.fixedDeltaTime * t * 3);
                                transform.rotation = Quaternion.Slerp(transform.rotation, originalRot, Time.fixedDeltaTime * t * 2);
                            }
                        }
                        else
                        {
                            transform.position = Vector3.Lerp(transform.position, snapTo.position, Time.fixedDeltaTime * t * 3);
                            transform.rotation = Quaternion.Slerp(transform.rotation, snapTo.rotation, Time.fixedDeltaTime * t * 2);
                        }
                    }
                }
            }
        }
    }
}