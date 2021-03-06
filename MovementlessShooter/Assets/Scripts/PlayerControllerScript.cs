using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum WeaponType
{
    Revolver = 0,   //med damage, med rate, med push
    AutoRifle,      //low-med damage, high rate, low-med push
    Shotgun,        //high damage, low-med rate, high push
    Sniper,         //high damage, slow fire rate, low push
    Melee           //high damage, med-high rate, med push forward (lunge)
}

public class PlayerControllerScript : MonoBehaviour
{
    
    public float baseFOV = 60, currentFOV, jumpForce = 9f;


    float VeryLowPushBack = 0.5f,
          LowPushBack = 0.8f,
          MedPushBack = 1.35f,
          HighPushBack = 1.5f,
          VeryHighPushBack = 2.5f;

    float autoRifleReloadSpeed = 2.2f,  shotgunReloadSpeed = 0.8f,  revolverReloadSpeed = 0.6f, sniperReloadSpeed = 1.5f;
    int   autoRifleReloadAmount = 0,    shotgunReloadAmount = 1,    revolverReloadAmount = 1,   sniperReloadAmount = 0;
    int   autoRifleMaxMag = 50,         shotgunMaxMag = 3,          revolverMaxMag = 6,         sniperMaxMag = 4;
    public 
    bool  autoUnlocked = false,         shotgunUnlocked = false,                                sniperUnlocked = false,     swordUnlocked = false;
    public int unlockedWeaponTypes = 0;

    float shootLockOutTimer = 0f, maxMagSize, currentMag;

    public bool canJump = true, canShoot = false, reloading = false;
    [HideInInspector]
    public GameObject bulletSpawnPoint;
    [HideInInspector]
    public GameObject bulletPreFab;
    [HideInInspector]
    public WeaponType equipWeaponType = WeaponType.Revolver;
    float baseForceMult = 1.0f, forceMult = 1.0f;

    Rigidbody rb;

    public TextMeshProUGUI wepText, ammoText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Camera>().fieldOfView = baseFOV;
        currentFOV = baseFOV;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootLockOutTimer < 0)
            canShoot = true;
        else
            shootLockOutTimer -= Time.deltaTime;


        unlockedWeaponTypes = 0;
        if (autoUnlocked)
            unlockedWeaponTypes = 1;
        if (shotgunUnlocked)
            unlockedWeaponTypes = 2;
        if (sniperUnlocked)
            unlockedWeaponTypes = 3;
        if (swordUnlocked)
            unlockedWeaponTypes = 4;

        switch(equipWeaponType)
        {
            case WeaponType.Revolver:
                {
                    maxMagSize = revolverMaxMag;
                    wepText.text = "Weapon:\nRevolver";
                    ammoText.text = "Ammo: " + currentMag + "/" + maxMagSize;
                    break;
                }
            case WeaponType.AutoRifle:
                {
                    maxMagSize = autoRifleMaxMag;
                    wepText.text = "Weapon:\nAssault Rifle";
                    ammoText.text = "Ammo: " + currentMag + "/" + maxMagSize;
                    break;
                }
            case WeaponType.Shotgun:
                {
                    maxMagSize = shotgunMaxMag;
                    wepText.text = "Weapon:\nShotgun";
                    ammoText.text = "Ammo: " + currentMag + "/" + maxMagSize;
                    break;
                }
            case WeaponType.Sniper:
                {
                    maxMagSize = sniperMaxMag;
                    wepText.text = "Weapon:\nSniper Rifle";
                    ammoText.text = "Ammo: " + currentMag + "/" + maxMagSize;
                    break;
                }
            case WeaponType.Melee:
                {
                    maxMagSize = 1;
                    wepText.text = "Weapon:\nSword";
                    ammoText.text = "Ammo: Infinite";
                    break;
                }
        }
        forceMult = baseForceMult;
        playerInput();
    }

    void playerInput()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StopCoroutine("reloadCoRo");
            equipWeaponType = (WeaponType)loopClamp((int)equipWeaponType + 1, 0, unlockedWeaponTypes);
            
            switch (equipWeaponType)
            {
                case WeaponType.Revolver:
                    {
                        maxMagSize = revolverMaxMag;
                        break;
                    }
                case WeaponType.AutoRifle:
                    {
                        maxMagSize = autoRifleMaxMag;
                        break;
                    }
                case WeaponType.Shotgun:
                    {
                        maxMagSize = shotgunMaxMag;
                        break;
                    }
                case WeaponType.Sniper:
                    {
                        maxMagSize = sniperMaxMag;
                        break;
                    }
                case WeaponType.Melee:
                    {
                        maxMagSize = 1;
                        break;
                    }
            }

            currentMag = 0;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            //interact
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (!reloading)
            {
                reloading = true;
                reload();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //jump
            if (canJump)
            {
                Debug.Log("try jumping");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                canJump = false;
            }
        }

        if (!canJump)
            forceMult += 0.5f;

        //crouch
        if (Input.GetKey(KeyCode.LeftControl))
            forceMult -= 0.25f;

        if(Input.GetMouseButton(1))
        {
            //ADS
            currentFOV = Mathf.Lerp(currentFOV, baseFOV - 20, Time.deltaTime);
            forceMult -= 0.25f;
        }
        else
        {
            //release ADS
            currentFOV = Mathf.Lerp(currentFOV, baseFOV, Time.deltaTime);
        }
        Mathf.Clamp(currentFOV, baseFOV - 20, baseFOV);
        gameObject.transform.GetChild(0).GetComponent<Camera>().fieldOfView = currentFOV;

        if (Input.GetMouseButton(0) && canShoot)
        {
            if (currentMag > 0)
                shoot();
            else
            {
                if (!reloading)
                {
                    reloading = true;
                    reload();
                }
            }
        }
    }

    void reload()
    {
        reloading = true;
        switch (equipWeaponType)
        {
            case WeaponType.Revolver:
                {
                    StartCoroutine(reloadCoRo(revolverReloadSpeed, revolverReloadAmount));
                    break;
                }
            case WeaponType.AutoRifle:
                {
                    StartCoroutine(reloadCoRo(autoRifleReloadSpeed, autoRifleReloadAmount));
                    break;
                }
            case WeaponType.Shotgun:
                {
                    StartCoroutine(reloadCoRo(shotgunReloadSpeed, shotgunReloadAmount));
                    break;
                }
            case WeaponType.Sniper:
                {
                    StartCoroutine(reloadCoRo(sniperReloadSpeed, sniperReloadAmount));
                    break;
                }
        }
        
    }

    IEnumerator reloadCoRo(float reloadTime, int reloadAmount = 0)
    {
        if (reloading && currentMag < maxMagSize)
        {
            yield return new WaitForSeconds(reloadTime);
            if (reloading)
            {
                if (reloadAmount == 0)
                {
                    currentMag = maxMagSize;
                    reloading = false;
                }
                else
                {
                    if (reloading)
                    {
                        currentMag += reloadAmount;
                        currentMag = Mathf.Clamp(currentMag, 0, maxMagSize);

                        if (currentMag < maxMagSize && reloading)
                            reload();

                        if(currentMag >= maxMagSize)
                            reloading = false;
                    }
                }
            }
        }
        currentMag = Mathf.Clamp(currentMag, 0, maxMagSize);

    }

    void shoot()
    {
        reloading = false;
        switch(equipWeaponType)
        {
            case WeaponType.Revolver:
                {
                    Vector3 bulletSpread = new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f));

                    //create bullet
                    GameObject bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);
                    
                    bulletScript scr = bullet.gameObject.GetComponent<bulletScript>();
                    scr.damage = 2.0f;
                    scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.dir = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.lifetime = 4.5f;
                    
                    //apply pushback to player
                    rb.AddForce(MedPushBack * forceMult * transform.GetChild(0).transform.forward * -1, ForceMode.Impulse);

                    //set lockout for shooting again
                    canShoot = false;
                    shootLockOutTimer = 0.7f;
                    currentMag--;

                    break;
                }
            case WeaponType.AutoRifle:
                {
                    Vector3 bulletSpread = new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f));

                    //create bullet
                    GameObject bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);

                    bulletScript scr = bullet.gameObject.GetComponent<bulletScript>();
                    scr.damage = 2.0f;
                    scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.dir = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.lifetime = 3.8f;

                    //apply pushback to player
                    rb.AddForce(VeryLowPushBack * forceMult * transform.GetChild(0).transform.forward * -1, ForceMode.Impulse);

                    //set lockout for shooting again
                    canShoot = false;
                    shootLockOutTimer = 0.1f;
                    currentMag--;

                    break;
                }
            case WeaponType.Shotgun:
                {
                    GameObject bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);

                    bulletScript scr = bullet.gameObject.GetComponent<bulletScript>();
                    scr.damage = 2.0f;
                    scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized) * 3f;
                    scr.dir = (transform.GetChild(0).transform.forward.normalized) * 3f;
                    scr.lifetime = 2.65f;

                    Vector3 bulletSpread;
                    //create bullet
                    for (int i = 0; i < 9; ++i)
                    {
                        bulletSpread = new Vector3(Random.Range(-0.06f, 0.06f), Random.Range(-0.06f, 0.06f), Random.Range(-0.06f, 0.06f));

                        bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);

                        scr = bullet.gameObject.GetComponent<bulletScript>();
                        scr.damage = 2.0f;
                        scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                        scr.dir = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                        scr.lifetime = 2.55f;
                    }
                    //apply pushback to player
                    rb.AddForce(VeryHighPushBack * forceMult * transform.GetChild(0).transform.forward * -1, ForceMode.Impulse);

                    //set lockout for shooting again
                    canShoot = false;
                    shootLockOutTimer = 1.3f;
                    currentMag--;

                    break;
                }
            case WeaponType.Sniper:
                {
                    Vector3 bulletSpread = new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f));

                    //create bullet
                    GameObject bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);

                    bulletScript scr = bullet.gameObject.GetComponent<bulletScript>();
                    scr.damage = 6.0f;
                    scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.dir = (transform.GetChild(0).transform.forward.normalized + bulletSpread) * 3f;
                    scr.lifetime = 4.8f;

                    //apply pushback to player
                    rb.AddForce(LowPushBack * forceMult * transform.GetChild(0).transform.forward * -1, ForceMode.Impulse);

                    //set lockout for shooting again
                    canShoot = false;
                    shootLockOutTimer = 1f;
                    currentMag--;

                    break;
                }
            case WeaponType.Melee:
                {
                    //create bullet
                    GameObject bullet = Instantiate(bulletPreFab, bulletSpawnPoint.transform.position, Quaternion.identity);

                    bulletScript scr = bullet.gameObject.GetComponent<bulletScript>();
                    scr.damage = 6.0f;
                    scr.gameObject.GetComponent<Rigidbody>().velocity = (transform.GetChild(0).transform.forward.normalized) * 3f;
                    scr.dir = (transform.GetChild(0).transform.forward.normalized) * 3f;
                    scr.lifetime = 0.03f;

                    //apply pushback to player
                    rb.AddForce(HighPushBack * forceMult * transform.GetChild(0).transform.forward, ForceMode.Impulse);

                    //set lockout for shooting again
                    canShoot = false;
                    shootLockOutTimer = 0.6f;

                    break;
                }
        }
        

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            canJump = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            canJump = true;
    }

    public float loopClamp(float value, float min, float max)
    {
        if (value < min)
            value = max;

        if (value > max)
            value = min;

        return value;
    }
}