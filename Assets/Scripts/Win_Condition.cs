using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win_Condition : MonoBehaviour {

    public bool pickedUpByPlayer;

    void Start() {
        pickedUpByPlayer = false;
    }

    void OnTriggerEnter(Collision col) {
        if(col.gameObject.CompareTag("Register")) {
            if(pickedUpByPlayer) {
                print("win");
            } else {
                // lose
            }
        }
    }
}
