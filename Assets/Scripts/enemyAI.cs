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

    Color colorOrig; 

    //Lecture 3
    float shootTimer;

    float angleToPlayer;

    bool playerInRange;

    //Lecture 3
    //can't do in game manager because specific to each enemy
    Vector3 playerDir;
   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1); //1 more enemy to kill for win condition    
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (playerInRange && canSeePlayer())
        {
            //do nothing, handled in canSeePlayer()
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

    //Lecture 3
    void faceTarget()
    {
     
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
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
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }


                if (shootTimer > shootRate)
                {
                    shoot();
                }

                return true;
            }

        }

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
        if(HP <= 0)
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
