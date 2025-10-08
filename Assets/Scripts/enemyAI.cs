using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    //Lecture 3
    [SerializeField] int faceTargetSpeed;


    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate; //seconds between shots

    //Lecture 3
    [SerializeField] int FOV;
    [SerializeField] Transform headPos; //where raycast comes from

    //NEW
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime; 

    Color colorOrig; 

    //Lecture 3
    float shootTimer;

    //NEW roam
    float roamTimer; 

    float angleToPlayer;
    //new for roam
    float stoppingDistOrig; 

    bool playerInRange;

    //Lecture 3
    //can't do in game manager because specific to each enemy
    Vector3 playerDir;

    //NEW for roam
    Vector3 startingPos; 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1); //1 more enemy to kill for win condition    
        //new
        stoppingDistOrig = agent.stoppingDistance;
        //new
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        //new for roam
        if(agent.remainingDistance <= 0.01f) //if close to destination, start timer to roam again
        {
            roamTimer += Time.deltaTime; 
        }
        //two roam conditions: player not in range, or player in range but can't see player
        if (playerInRange && canSeePlayer())
        {
            //below here is new for roam
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }

        /*
        //Lecture 3
        playerDir = gameManager.instance.player.transform.position - transform.position;
        //end Lecture 3

        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

            //Lecture 3
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
            //end Lecture 3


            if (shootTimer > shootRate)
            {
                shoot();

            }
        }*/

    }

    //new for roam
    void checkRoam()
    {
        if (roamTimer > roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    //new for roam
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0; //so enemy goes right to point

        Vector3 ranPos = startingPos + Random.insideUnitSphere * roamDist; //random point in sphere around starting pos

        ranPos += startingPos; //attaches to starting pos

        NavMeshHit hit; //guarantees point is on navmesh

        if (NavMesh.SamplePosition(ranPos, out hit, roamDist, 1)) //1 is area mask, 1 is default walkable area (a layer thing)
        {
            agent.SetDestination(hit.position);
        }
    }

    //Lecture 3
    void faceTarget() //could put in gameObject target if you want to change target later
    {
     
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z)); //could also use 0 instead of transform.position.y if you want to snap to y axis
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

bool canSeePlayer()
    {
        //Lecture 3
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            //to see what enemy raycast hits
            Debug.Log("Enemy is hitting " + hit.collider.name);

            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player")) 
            {

                agent.SetDestination(gameManager.instance.player.transform.position);

                //Lecture 3
                //new for roam
                if (agent.remainingDistance <= agent.stoppingDistance) //stoppingDistOrig can be ued if you want to change stopping distance later
                {
                    faceTarget();
                }


                if (shootTimer > shootRate)
                {
                    shoot();
                }
                //new for roam
                agent.stoppingDistance = stoppingDistOrig; //reset stopping distance in case it was changed during roaming
                return true;
            }

        }
        //new for roam
        agent.stoppingDistance = 0; 

        return false;

    }


    //end Lecture 3

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        playerInRange = true;
    }
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        playerInRange = false;
        //new
        agent.stoppingDistance = 0; 
    }
}

void shoot ()
    {
        shootTimer = 0;
        //create object at runtime/in real time
        Instantiate(bullet, shootPos.position, transform.rotation);

    }

    public void takeDamage(int amount)
    {
        HP -= amount; 
        //new
        agent.SetDestination(gameManager.instance.player.transform.position); //chases player if hit
        if (HP <= 0)
        {
            Destroy(gameObject); 
            gameManager.instance.updateGameGoal(-1); //1 less enemy to kill for win condition
        }

        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red; 
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
