using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour {

    public bool thrown = false;

    public void wasThrown() {
        StartCoroutine(waiter());
    }

    IEnumerator waiter() {
        thrown = true;
        yield return new WaitForSeconds(1);
        thrown = false;
    }

}
