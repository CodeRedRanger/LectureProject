using UnityEngine;
//Lecture 3
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask ignoreLayer; 
    [SerializeField] CharacterController controller;

    [SerializeField] int HP; 
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    private Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig; 

    float shootTimer; 

    bool isSprinting; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;

        //Lecture 3
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow);
        shootTimer += Time.deltaTime;

        movement();

        sprint(); 
    }

    void movement()
    {

        if(controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0; 
        }

        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }
        
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime); 

        if(Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }


    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;

        }

        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;  
        }
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpCountMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++; 
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit; 

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            Debug.Log(hit.collider.name); 

        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        //Lecture 3
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            //player death
            gameManager.instance.youLose();
        }
    }

    //Lecture 3
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }
}
