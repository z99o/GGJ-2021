using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Look : MonoBehaviour
{
    // Start is called before the first frame update
    public float mouse_sensitivity = 100f;
    public float x_rotation = 0;
    public Transform player;
    public Character_Movement_3D controller;

    public float bob_a;
    public float bob_h;
    public float bob_f;
    public float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouse_x = Input.GetAxis("Mouse X") *mouse_sensitivity *Time.deltaTime;
        float mouse_y = Input.GetAxis("Mouse Y") *mouse_sensitivity *Time.deltaTime;
        x_rotation -= mouse_y;
        x_rotation = Mathf.Clamp(x_rotation,-90, 90);
        player.Rotate(Vector3.up * mouse_x);
        transform.localRotation = Quaternion.Euler(x_rotation,0f,0f);
        Bob_Camera();
    }

    private void Bob_Camera(){
        Vector3 pos = transform.localPosition;
        //bob the camera at height h an amplitude a and rate speed magnitude
        float power = (Mathf.Abs(new Vector2(controller.m_cur_speed_x,controller.m_cur_speed_z).magnitude)*bob_f);
        if(power > 0)
            power = 10;
        if(controller.i_is_sprinting && !controller.m_sprint_is_exhausted)
            power = 20;
        if(controller.m_sprint_is_exhausted)
            power = 5;
        if(!controller.m_is_grounded)
            power = 0;
        pos.y = bob_h + (bob_a * Mathf.Sin(timer*power));
        //Debug.Log(power);
        timer += Time.deltaTime;
        transform.localPosition = pos;
    }

}
