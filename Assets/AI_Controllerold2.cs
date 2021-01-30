using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controllerold2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask lm_ground, lm_player;
    public Vector3 target_pos;
    bool is_target_set;
    public float walk_point_range, sight_range, attack_range;

    //Attacking
    public float time_between_attacks;
    bool already_attacked;
    public bool player_in_sight_range, player_in_attack_range;

    public Rigidbody[] BodyParts;

    public Animator animator;
    
    // Start is called before the first frame update


    void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = this.GetComponentInChildren<Animator>();
        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player in sight
        player_in_attack_range = Physics.CheckSphere(transform.position, attack_range,lm_player);
        player_in_sight_range = Physics.CheckSphere(transform.position, sight_range,lm_player);
        if(!player_in_sight_range && !player_in_attack_range){
            Patrolling();
        }
        if(player_in_sight_range && !player_in_attack_range){
            ChasePlayer();
        }
        if(player_in_sight_range && player_in_attack_range){
            AttackPlayer();
        }
    }

    private void Patrolling(){
        if(!is_target_set){
            animator.SetBool("a_is_idle",true);
            animator.SetBool("a_running",false);
            SearchWalkPoint();
        }
        if(is_target_set){
            agent.SetDestination(target_pos);
            animator.SetBool("a_is_idle",false);
            animator.SetBool("a_running",true);
        }
        Vector3 target_d = transform.position - target_pos;
        if(target_d.magnitude < 1f){
            is_target_set = false;
        }
    }
    private void SearchWalkPoint(){
        float randomZ = Random.Range(-walk_point_range,walk_point_range);
        float randomX = Random.Range(-walk_point_range,walk_point_range);
        target_pos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if(Physics.Raycast(target_pos,-transform.up, 2f, lm_ground)){
            is_target_set = true;
        }
    }

    private void ChasePlayer(){
        agent.SetDestination(player.position);
    }
    private void AttackPlayer(){
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if(!already_attacked){
            //attack
            already_attacked = true;
            animator.SetTrigger("a_attack");
            animator.SetBool("a_is_idle",false);
            animator.SetBool("a_running",true);
            Invoke(nameof(ResetAttack),time_between_attacks);
        }
    }
    private void ResetAttack(){
        already_attacked = false;
    }
    
    private void DisableRagdoll(){
        int count = 0;
        foreach (var item in BodyParts)
        {
            item.isKinematic = true;
            item.detectCollisions = false;
            count ++;
        }
        Debug.Log(count + " ragdoll joints disabled");
        
    }
    private void EnableRagdoll(){
        foreach (var item in BodyParts)
        {
            item.isKinematic = false;
            item.detectCollisions = true;
        }
    }
}
