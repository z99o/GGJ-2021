using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask lm_ground, lm_player, lm_interactable;
    public Vector3 target_pos;
    bool is_target_set;
    bool isHoldingSomething = false;
    GameObject holdObject;
    public float walk_point_range, sight_range, attack_range, interact_range;

    //Attacking
    public float time_between_attacks;
    bool already_attacked;
    public bool player_in_sight_range, player_in_attack_range, interactable_in_range;


    void Start() {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        //Check if player in sight
        player_in_attack_range = Physics.CheckSphere(transform.position, attack_range, lm_player);
        player_in_sight_range = Physics.CheckSphere(transform.position, sight_range, lm_player);
        interactable_in_range = Physics.CheckSphere(transform.position, interact_range, lm_interactable);
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

        //Throwing looks weird
        /*float chance = Random.value * 100;
        if (chance <= 0.1 && isHoldingSomething) {
            GetComponent<PlayerInteractions>().ThrowObject();
        }*/

    }

    private void Patrolling() {
        //sets target
        if (!is_target_set) {
            SearchWalkPoint();
        }
        if(is_target_set){
            agent.SetDestination(target_pos);
        }

        //updates position
        Vector3 target_d = transform.position - target_pos;
        if (target_d.magnitude < 1f) {
            is_target_set = false;
        }

        //interacts with objects randomly
        if(interactable_in_range) {
            int chance = (int)(Random.value * 100);
            if (chance > 33) {
                GetComponent<PlayerInteractions>().PickUpObject(GameObject.FindWithTag("Interactable"));
                isHoldingSomething = true;
            }
        }
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
        if (interactable_in_range) {
            int chance = 100;//(int)(Random.value * 100);
            if (chance > 33 && !isHoldingSomething) {
                holdObject = GameObject.FindWithTag("Interactable");
                isHoldingSomething = true;
            }
        }
    }

    private void AttackPlayer() {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if(!already_attacked){
            already_attacked = true;
            Invoke(nameof(ResetAttack),time_between_attacks);
        }
    }

    private void ResetAttack() {
        already_attacked = false;
    }
}
