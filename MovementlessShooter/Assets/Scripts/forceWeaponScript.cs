using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class forceWeaponScript : MonoBehaviour
{
    public WeaponType forceWeapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch(forceWeapon)
            {
                case WeaponType.AutoRifle:
                    {
                        GameObject.Find("Player").GetComponent<PlayerControllerScript>().autoUnlocked = true;
                        break;
                    }
                case WeaponType.Sniper:
                    {
                        GameObject.Find("Player").GetComponent<PlayerControllerScript>().sniperUnlocked = true;
                        break;
                    }
                case WeaponType.Shotgun:
                    {
                        GameObject.Find("Player").GetComponent<PlayerControllerScript>().shotgunUnlocked = true;
                        break;
                    }
                case WeaponType.Melee:
                    {
                        GameObject.Find("Player").GetComponent<PlayerControllerScript>().swordUnlocked = true;
                        break;
                    }
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameObject.Find("Player").GetComponent<PlayerControllerScript>().equipWeaponType = forceWeapon;
        }
    }
}
