using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabble : MonoBehaviour
{
    public Rigidbody objectRigidbody;
    public Transform objectGrabPointTransform;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
    }
    public void DropOrThrow()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
    }
    private void FixedUpdate()
    {
        if(objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position,
                Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);

        }
    }
}
