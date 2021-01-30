using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {
    [Header("InteractableInfo")]
    public float sphereCastRadius = 0.5f;
    public int interactableLayerIndex;
    private Vector3 raycastPos;
    public GameObject lookObject;
    private PhysicsObject physicsObject;
    private Camera mainCamera;

    [Header("Pickup")]
    [SerializeField] private Transform pickupParent;
    public GameObject currentlyPickedUpObject;
    private Rigidbody pickupRB;

    [Header("ObjectFollow")]
    [SerializeField] private float minSpeed = 0;
    [SerializeField] private float maxSpeed = 10000f;
    [SerializeField] private float maxDistance = 10f;
    private float currentSpeed = 0f;
    private float currentDist = 0f;

    [Header("Rotation")]
    public float rotationSpeed = 10000f;
    Quaternion lookRot;

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



        //if we press the button of choice
        if (Input.GetButton("Fire1")) {
            //and we're not holding anything
            if (currentlyPickedUpObject == null) {
                //and we are looking an interactable object
                if (lookObject != null) {
                    PickUpObject();
                }

            }
        } else if (currentlyPickedUpObject != null) {
            BreakConnection();
        }


    }

    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate() {
        if (currentlyPickedUpObject != null) {
            currentDist = Vector3.Distance(pickupParent.position, pickupRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = pickupParent.position - pickupRB.position;
            pickupRB.velocity = direction.normalized * currentSpeed;

            //Rotation
            lookRot = Quaternion.LookRotation(mainCamera.transform.position - pickupRB.position);
            lookRot = Quaternion.Slerp(mainCamera.transform.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime);
            pickupRB.MoveRotation(lookRot);
        }

    }

    //Release the object
    public void BreakConnection() {
        pickupRB.constraints = RigidbodyConstraints.None;
        currentlyPickedUpObject = null;
        physicsObject.pickedUp = false;
        currentDist = 0;
    }

    public void PickUpObject() {
        physicsObject = lookObject.GetComponentInChildren<PhysicsObject>();
        currentlyPickedUpObject = lookObject;
        pickupRB = currentlyPickedUpObject.GetComponent<Rigidbody>();
        //pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
        physicsObject.playerInteractions = this;
        StartCoroutine(physicsObject.PickUp());
    }


}