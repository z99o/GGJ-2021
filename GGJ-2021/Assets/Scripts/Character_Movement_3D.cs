using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement_3D : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public CharacterController controller;
    [SerializeField] private float m_min_speed = 0f;
    [SerializeField] public float  m_max_speed = 5f;
    [SerializeField] public float  m_cur_speed_x;
    [SerializeField] public float  m_cur_speed_z;
    [SerializeField] public float m_acceleration_rate;
    [SerializeField] public float jump_height;
    [SerializeField] private float m_gravity = -9.8f;
    [SerializeField] Vector3 m_velocity;
    [SerializeField] public LayerMask ground_mask;
    [SerializeField] public bool m_is_grounded;
    [SerializeField] public Transform ground_check;
    [SerializeField] public float grounded_clearance; //How large the grounded check sphere is
    [SerializeField] public float i_x_movement;
    [SerializeField] public float i_z_movement;
    [SerializeField] public bool i_is_jumping;
    [SerializeField] public Transform cam;
    
    void Start()
    {
        
    }

    //Refresh inputs every fixed frame, preventing input lag
    private void FixedUpdate() {
        
    }

    void Update()
    {
        m_is_grounded = Check_If_Grounded();
        Get_Inputs();
        Move();
    }

    public bool Check_If_Grounded(){
        return Physics.CheckSphere(ground_check.position,grounded_clearance,ground_mask);
    }

    void Move(){
        //Get our inputs
        float x = i_x_movement;
        float z = i_z_movement;
        Vector2 speed = Calculate_Speed(x,z);
        //Multiply by speed components
        Vector3 move = transform.right * speed.x + transform.forward * speed.y;
        //m_velocity = (move * Time.deltaTime);
        controller.Move(move * Time.deltaTime);
        //Compute jump
        Jump();
        Apply_Gravity();
        controller.Move(m_velocity*Time.deltaTime);
    }

    private void Apply_Gravity(){
        m_velocity.y += m_gravity * Time.deltaTime;
    }

    private void Jump(){
        if(m_is_grounded && m_velocity.y < 0)
            m_velocity.y = -2;

        if(i_is_jumping && m_is_grounded){
            m_velocity.y = Mathf.Sqrt(jump_height * -2 * m_gravity);
        }
    }

    private void Get_Inputs(){
        i_x_movement = Input.GetAxis("Horizontal");
        i_z_movement = Input.GetAxis("Vertical");
        i_is_jumping = Input.GetButtonDown("Jump");
    }
    public Vector2 Calculate_Speed(float x, float z){
        if(x != 0 || z != 0){
            //look at last velocity and apply acceleration based off our movements
            m_cur_speed_x += x * Time.deltaTime * m_acceleration_rate;
            m_cur_speed_z += z * Time.deltaTime * m_acceleration_rate;
        }
        //Axis inputs will still be non zero even when the key is not depressed so we evaluate between -1 and 1
        //decreasing the value (absolutely) will allow for a spotting distance
        float val = 0.5f;
        if(x < val && x > -val){
            //reset x's acceleration
            m_cur_speed_x = 0;
        }
        if(z < val && z > -val){
            //reset z's acceleration
            m_cur_speed_z = 0;
        }
        //Lastly, clamp everything
        m_cur_speed_x = Mathf.Clamp(m_cur_speed_x,m_min_speed,m_max_speed);
        m_cur_speed_z = Mathf.Clamp(m_cur_speed_z,m_min_speed,m_max_speed);
        return new Vector2(m_cur_speed_x,m_cur_speed_z);
    }

}
