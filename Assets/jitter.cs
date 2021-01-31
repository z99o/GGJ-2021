using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jitter : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 origin;
    public Vector3 jitter_to;
    void Start()
    {
        origin = transform.position;
        jitter_to = origin + jitter_to;
    }

    // Update is called once per frame
    void Update()
    {
            if(transform.position == origin)
                transform.position = jitter_to;
            else if(transform.position == jitter_to){
                transform.position = origin;
            }


    }
}
