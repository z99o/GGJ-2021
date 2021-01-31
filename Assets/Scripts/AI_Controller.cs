using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask lm_ground, lm_player, lm_interactable;
    public Vector3 target_pos;
    bool is_target_set, is_idle;
    bool isHoldingSomething = false;
    GameObject holdObject;
    public float walk_point_range, sight_range, attack_range, interact_range;
    float timer = 2f;

    public bool objectThrown;

    //Attacking
    public float time_between_attacks;
    bool already_attacked;
    public bool is_ragdolled;
    public float force_to_ragdoll;
    public bool player_in_sight_range, player_in_attack_range, interactable_in_range;

    public Rigidbody[] body_parts;
    public List<Transform> saved_transforms;

    public Animator animator;


    void Start() {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        objectThrown = false;
        //EnableRagdoll();
        DisableRagdoll();
    }

    void Update() {
        //Check if player in sight
        player_in_attack_range = Physics.CheckSphere(transform.position, attack_range, lm_player);
        player_in_sight_range = Physics.CheckSphere(transform.position, sight_range, lm_player);
        interactable_in_range = Physics.CheckSphere(transform.position, interact_range, lm_interactable);
        if(!is_ragdolled){
            if (!player_in_sight_range && !player_in_attack_range) {
                Patrolling();
            }
            if (player_in_sight_range && !player_in_attack_range) {
                ChasePlayer();
            }
            if (player_in_sight_range && player_in_attack_range) {
                AttackPlayer();
            }

            if (isHoldingSomething) {
                GetComponent<PlayerInteractions>().PickUpObject(holdObject);
            }
        }

        if(objectThrown) {
            if (timer > 0) {
                timer -= Time.deltaTime;
                print(timer);
            } else {
                objectThrown = false;
                timer = 2f;
                print("Done");
            }
        }

        float chance = Random.value * 100;
        if (chance >= 0.01 && isHoldingSomething) {
            throwObject();
            print("thrown");
        }

    }

    private void Patrolling() {
        if(is_idle){
            return;
        }
        //sets target
        if (!is_target_set) {
            animator.SetBool("a_is_idle",true);
            animator.SetBool("a_running",false);
            SearchWalkPoint();
        }
        if(is_target_set){
            animator.SetBool("a_is_idle",false);
            animator.SetBool("a_running",true);
            agent.SetDestination(target_pos);
        }

        //updates position
        Vector3 target_d = transform.position - target_pos;
        if (target_d.magnitude < 1f) {
            is_target_set = false;
        }
        //chance to idle
        int chance = (int)(Random.value * 100);
            if (chance > 100) {
                Idle(); //not really wanting to idle for debug purposes
            }

        //interacts with objects randomly
        if(interactable_in_range) {
            chance = (int)(Random.value * 100);
            if (chance > 33) {
                GetComponent<PlayerInteractions>().PickUpObject(GameObject.FindWithTag("Interactable"));
                if (holdObject != null) {
                    isHoldingSomething = true;
                }
            }
        }
    }

    private void Idle(){
        //do nothing for 10 seconds
        is_idle = true;
        animator.SetBool("a_is_idle",true);
        animator.SetBool("a_running",false);
        agent.SetDestination(transform.position);
        Invoke("EndIdle",10f);

    }
    private void EndIdle(){
        is_idle = false;
    }

    private void SearchWalkPoint() {
        float randomZ = Random.Range(-walk_point_range,walk_point_range);
        float randomX = Random.Range(-walk_point_range,walk_point_range);
        target_pos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if(Physics.Raycast(target_pos,-transform.up, 2f, lm_ground)){
            is_target_set = true;
        }
    }

    private void ChasePlayer() {
        agent.SetDestination(player.position);
        is_idle = false;
        animator.SetBool("a_is_idle",false);
        animator.SetBool("a_running",true);
        if (interactable_in_range) {
            int chance = 100;//(int)(Random.value * 100);
            if (chance > 33 && !isHoldingSomething && !objectThrown) {
                holdObject = GameObject.FindWithTag("Interactable");
                if (holdObject != null) {
                    isHoldingSomething = true;
                }
            }
        }
    }

    private void AttackPlayer() {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if(!already_attacked){
            already_attacked = true;
            animator.SetTrigger("a_attack");
            Invoke(nameof(ResetAttack),time_between_attacks);
        }
    }

    private void ResetAttack() {
        already_attacked = false;
    }

       private void DisableRagdoll(){
        int count = 0;
        saved_transforms.Clear();
        foreach (var item in body_parts)
        {
            saved_transforms.Add(item.transform);
            item.isKinematic = true;
            item.detectCollisions = false;
            animator.enabled = true;
            GetComponent<BoxCollider>().enabled = true;
            count ++;
        }
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        is_ragdolled = false;
        Debug.Log(count + " ragdoll joints disabled");
        
    }
    private void EnableRagdoll(){
        int count = 0;
        foreach (var item in body_parts)
        { 
            //restore positions from ragdoll positions
            //item.transform.position = saved_transforms[count].position;
            item.isKinematic = false;
            item.detectCollisions = true;
            animator.enabled = false;
            count++;
        }
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Debug.Log(count + " ragdoll joints enabled");
        is_ragdolled = true;
    }
    private void OnCollisionEnter (Collision collision) {
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        
        if (collisionForce > force_to_ragdoll) {
            EnableRagdoll();
            Invoke("DisableRagdoll",5f);
        }
    }

    private void throwObject() {
        GetComponent<PlayerInteractions>().ThrowObject();
        objectThrown = true;
        isHoldingSomething = false;
    }

}
