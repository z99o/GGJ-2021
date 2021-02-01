using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinBox : MonoBehaviour
{
    void OnTriggerEnter(Collider col) {
        if(col.gameObject.GetComponent<PlayerInteractions>() != null) {
            if(col.gameObject.GetComponent<PlayerInteractions>().holdingWin) {
                SceneManager.LoadScene(2);
            }
        }
    }
}
