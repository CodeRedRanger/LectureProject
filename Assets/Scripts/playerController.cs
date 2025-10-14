using UnityEngine;
//Lecture 3
using System.Collections;
//Lecture 5
using System.Collections.Generic;


public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] LayerMask ignoreLayer; 
    [SerializeField] CharacterController controller;

    [SerializeField] int HP; 
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;

    //Lecture 5
    [SerializeField] List<gunStats> gunList = new List<gunStats>(); 
    [SerializeField] GameObject gunModel; 
    
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    int gunListPos; 



    private Vector3 moveDir;
    Vector3 playerVel;
    //pushback example, can call on damage and explosion script
    public Vector3 pushBack;
    [SerializeField] int pushBackTime; 

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

        //Lecture 5 if statement
        if(!gameManager.instance.isPaused)
        {
            movement(); 
        }
        
        //must be outside isPaused statement because other wise you could 
        //hold down sprint, then pause, and unpause and sprit boost will remain
        //because button up was not recognized. 
        sprint(); 
    }
    //pushback example
    public void AppliedPushback(Vector3 direction)
    {
        pushBack = direction;
    }
    void movement()
    {
        //pushback example
        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackTime); 
        if(controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0; 
        }

        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }
        
        //pushback example
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move((moveDir + pushBack) * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime); 

        //Lecture 5, added gunListcount, ammocurrent
        if(Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
        {
            shoot();
        }

        //Lecture 5
        selectGun();
        reload();


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

        //Lecture 5
        gunList[gunListPos].ammoCur--; 
        updatePlayerUI();

        RaycastHit hit; 

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //NEW
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            //Debug.Log(hit.collider.name); 

        }
    }

    //Lecture 5
    void reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
            updatePlayerUI(); 
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
        //Lecture 5
        if (gunList.Count > 0)
        {
            gameManager.instance.ammoCur.text = gunList[gunListPos].ammoCur.ToString("F0");
            gameManager.instance.ammoMax.text = gunList[gunListPos].ammoMax.ToString("F0");
        }
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }

    //Lecture 5
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;


        changeGun(); 

        
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count -1)
        {
            gunListPos++;
            changeGun(); 
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0 )
        {
            gunListPos--;
            changeGun();
        }


    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        updatePlayerUI();
    }


}
