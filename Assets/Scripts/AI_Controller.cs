using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : MonoBehaviour {

    [Header("Navigation")]
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
    [Header("Attack")]
    public float time_between_attacks;
    bool already_attacked;
    public bool is_ragdolled;
    public float force_to_ragdoll;
    public bool player_in_sight_range, player_in_attack_range, interactable_in_range;
    public Transform attackOrigin;
    public float attackRadius;
    public float attackForce;
    public AudioClip attackSound;

    [Header("Misc")]
    public Rigidbody[] body_parts;
    public List<Transform> saved_transforms;

    private GameObject cereal;

    public Animator animator;

    private bool playerHasCereal;

    private NavMeshPath path;

    private float got_stuck_timer;


    void Start() {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        objectThrown = false;
        cereal = GameObject.Find("Cereal");
        //EnableRagdoll();
        DisableRagdoll();
    }

    void Update() {
        playerHasCereal = cereal.GetComponent<Win_Condition>().pickedUpByPlayer;

        //Check if player in sight
        player_in_attack_range = Physics.CheckSphere(transform.position, attack_range, lm_player);
        player_in_sight_range = Physics.CheckSphere(transform.position, sight_range, lm_player);
        interactable_in_range = Physics.CheckSphere(transform.position, interact_range, lm_interactable);
        if(!is_ragdolled){
            if (playerHasCereal) {
                if (player_in_sight_range && !player_in_attack_range) {
                    ChasePlayer();
                } else if (player_in_sight_range && player_in_attack_range) {
                    AttackPlayer();
                }
            } else {
                if (!player_in_sight_range && !player_in_attack_range) {
                    Patrolling();
                }
            }

            if (isHoldingSomething) {
                GetComponent<PlayerInteractions>().PickUpObject(holdObject);
            }
        }

        if(objectThrown) {
            if (timer > 0) {
                timer -= Time.deltaTime;
                //print(timer);
            } else {
                objectThrown = false;
                timer = 2f;
                //print("Done");
            }
        }

        float chance = Random.value * 100;
        if (chance >= 0.01 && isHoldingSomething) {
            throwObject();
            //print("thrown");
        }

    }

    private Vector3 last_pos;
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

        if(is_target_set) {
            animator.SetBool("a_is_idle",false);
            animator.SetBool("a_running",true);
            got_stuck_timer += Time.deltaTime;
            if(got_stuck_timer > 2f){
                if(((int)last_pos.x == (int)transform.position.x && (int)last_pos.z == (int)transform.position.z)){
                    SearchWalkPoint();
                }
                last_pos = transform.position;
                got_stuck_timer = 0;
        }
            agent.SetDestination(target_pos);
            
        }

        //updates position
        Vector3 target_d = transform.position - target_pos;
        if (target_d.magnitude < 1f) {
            is_target_set = false;
        }
        //chance to idle
        int chance = (int)(Random.value * 100);
            if (chance > 99) {
                Idle(); //not really wanting to idle for debug purposes
            }

        //interacts with objects randomly
        if(interactable_in_range) {
            chance = (int)(Random.value * 100);
            if (chance > 33) {
                GetComponent<PlayerInteractions>().PickUpObject(GameObject.FindWithTag("Interactable"));
                if (holdObject != null) {
                    isHoldingSomething = GetComponent<PlayerInteractions>().holdingSomething;
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
        Invoke("EndIdle",5f);

    }
    private void EndIdle(){
        is_idle = false;
    }

    private void SearchWalkPoint() {
        float randomZ = Random.Range(-walk_point_range,walk_point_range);
        float randomX = Random.Range(-walk_point_range,walk_point_range);
        target_pos = GetRandomLocation();
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
                    isHoldingSomething = GetComponent<PlayerInteractions>().holdingSomething;
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

            Collider[] colliders = Physics.OverlapSphere(attackOrigin.position, attackRadius);

            foreach (Collider nearbyObject in colliders) {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.AddForce(Vector3.forward * attackForce);
                    GetComponent<AudioSource>().PlayOneShot(attackSound, 1f);
                }

                if (nearbyObject.GetComponent<Character_Movement_3D>() != null) {
                    nearbyObject.GetComponent<Character_Movement_3D>().Take_Damage(10f);
                }
            }


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
        //Debug.Log(count + " ragdoll joints disabled");
        
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
        agent.SetDestination(transform.position);
        //Debug.Log(count + " ragdoll joints enabled");
        is_ragdolled = true;
    }
    private void OnCollisionEnter (Collision collision) {
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        
        if (collisionForce > force_to_ragdoll && collision.transform.tag == "Interactable") {
            EnableRagdoll();
            Invoke("DisableRagdoll",5f);
        }
        //if(collisionForce > force_to_ragdoll)
        //    print(collision.transform.name + " collided with " + collisionForce + ", possibly causing a ragdoll");
    }

    private void throwObject() {
        if (isHoldingSomething) {
            GetComponent<PlayerInteractions>().ThrowObject();
            objectThrown = true;
            isHoldingSomething = GetComponent<PlayerInteractions>().holdingSomething;
        }
    }

    public float elapsed;
    public bool show_path;
    private void ShowPath(){
            if(show_path){
                path = agent.path;
                // Update the way to the goal every second.
                elapsed += Time.deltaTime;
                if (elapsed > 1.0f)
                {
                    elapsed -= 1.0f;
                    NavMesh.CalculatePath(transform.position, target_pos, NavMesh.AllAreas, path);
                }
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;
        // Pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = Random.Range(0, maxIndices);
        int secondVertexSelected = Random.Range(0, maxIndices);
        //Spawn on Verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];
        //Eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x ||
            (int)firstVertexPosition.z == (int)secondVertexPosition.z
            )
        {
            point = GetRandomLocation(); //Re-Roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // Select a random point on it
            point = Vector3.Lerp(
                                            firstVertexPosition,
                                            secondVertexPosition, //[t + 1]],
                                            Random.Range(0.05f, 0.95f) // Not using Random.value as clumps form around Verticies 
                                        );
        }
        //Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value); //Made Obsolete

        return point;
    }



}
