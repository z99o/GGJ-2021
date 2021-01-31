using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//set to kinematic
public class PlayerInteractions : MonoBehaviour {
    [Header("InteractableInfo")]
    public float sphereCastRadius = 0.5f;
    public int interactableLayerIndex;
    private Vector3 raycastPos;
    public GameObject lookObject;
    private GameObject physicsObject;
    private Camera mainCamera;

    [Header("Pickup")]
    [SerializeField] private Transform pickupParent;
    private Rigidbody pickupRB;
    public float throwForce = 100f;
    public bool drivenByAi;
    [HideInInspector]public bool holdingSomething;

    [Header("ObjectFollow")]
    [SerializeField] private float maxDistance = 1f;

    private void Start() {
        mainCamera = Camera.main;
    }

    //Interactable Object detections and distance check
    void Update() {
        //Here we check if we're currently looking at an interactable object
        raycastPos = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.SphereCast(raycastPos, sphereCastRadius, mainCamera.transform.forward, out hit, maxDistance, 1 << interactableLayerIndex)) {
            lookObject = hit.collider.transform.root.gameObject;
        } else {
            lookObject = null;
        }

        if (!drivenByAi) {
            if (Input.GetButtonDown("Fire1")) {
                //and we're not holding anything
                if (physicsObject == null) {
                    //and we are looking an interactable object
                    if (lookObject != null) {
                        PickUpObject();
                    }

                } else {
                    BreakConnection();
                }
            }

            if (Input.GetButtonDown("Fire2")) {
                if (physicsObject != null) {
                    ThrowObject();
                }
            }
        }
    }

    //Release the object
    public void BreakConnection() {
        physicsObject.GetComponent<ThrowObject>().held = false;
        physicsObject.transform.parent = null;
        physicsObject.GetComponent<Rigidbody>().useGravity = true;
        physicsObject.GetComponent<Rigidbody>().freezeRotation = false;
        physicsObject.GetComponent<Rigidbody>().isKinematic = false;

        holdingSomething = false;

        physicsObject = null;
    }

    //player pickup
    public void PickUpObject() {
        physicsObject = lookObject;
        pickupRB = physicsObject.GetComponent<Rigidbody>();
        holdingSomething = true;

        physicsObject.GetComponent<ThrowObject>().held = true;

        physicsObject.GetComponent<Rigidbody>().useGravity = false;
        physicsObject.GetComponent<Rigidbody>().freezeRotation = true;
        physicsObject.GetComponent<Rigidbody>().isKinematic = true;
        physicsObject.transform.position = lookObject.transform.position;
        physicsObject.transform.parent = pickupParent.transform;

        if (physicsObject.TryGetComponent(out Win_Condition cereal)) {
            cereal.pickedUpByPlayer = true;
        }
    }

    //ai pickup
    public void PickUpObject(GameObject pickUp) {
        if (physicsObject == null) {
            physicsObject = pickUp;
            pickupRB = physicsObject.GetComponent<Rigidbody>();
            holdingSomething = true;

            physicsObject.GetComponent<ThrowObject>().held = true;

            physicsObject.GetComponent<Rigidbody>().useGravity = false;
            physicsObject.GetComponent<Rigidbody>().freezeRotation = true;
            physicsObject.GetComponent<Rigidbody>().isKinematic = true;
            physicsObject.transform.position = pickupParent.transform.position;
            physicsObject.transform.parent = pickupParent.transform;

            if (physicsObject.TryGetComponent(out Win_Condition cereal)) {
                cereal.pickedUpByPlayer = false;
            }
        }
    }

    public void ThrowObject() {
        if(holdingSomething == true) {
            BreakConnection();
            pickupRB.AddForce((pickupRB.transform.position - transform.position) * throwForce);
        }
    }

}