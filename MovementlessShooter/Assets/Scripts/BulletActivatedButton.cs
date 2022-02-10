using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WhatToDo
{
    EndLevel,
    MoveObj

}

public class BulletActivatedButton : MonoBehaviour
{
    public float timeToStayActive = -1f;
    public float activeTime;
    public bool activated = false, lockout = false;
    const float LockoutTime = 1.5f;
    public float tempLockout;
    public WhatToDo whatDo;

    [Header("End Level")]
    public string levelToLoad;

    [Header("MoveObj")]
    public GameObject objToMove;
    public Vector3 whereToMove;
    Vector3 objStartPos;

    // Start is called before the first frame update
    void Start()
    {
        if (whatDo == WhatToDo.MoveObj)
            objStartPos = objToMove.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(lockout)
            tempLockout -= Time.deltaTime;
        if (tempLockout <= 0)
            lockout = false;

        if (activated)
        {
            if (timeToStayActive >= 0)
            {
                activeTime -= Time.deltaTime;
            }
            else if(timeToStayActive != -1)
                activated = false;
            switch (whatDo)
            {
                case WhatToDo.EndLevel:
                    {
                        SceneManager.LoadScene(levelToLoad);
                        break;
                    }
                case WhatToDo.MoveObj:
                    {
                        objToMove.transform.position = new Vector3(Mathf.Lerp(objToMove.transform.position.x, whereToMove.x, Time.deltaTime), Mathf.Lerp(objToMove.transform.position.y, whereToMove.y, Time.deltaTime), Mathf.Lerp(objToMove.transform.position.z, whereToMove.z, Time.deltaTime));
                        break;
                    }
            }
        }

        if(!activated && whatDo == WhatToDo.MoveObj)
            objToMove.transform.position = new Vector3(Mathf.Lerp(objToMove.transform.position.x, objStartPos.x, Time.deltaTime), Mathf.Lerp(objToMove.transform.position.y, objStartPos.y, Time.deltaTime), Mathf.Lerp(objToMove.transform.position.z, objStartPos.z, Time.deltaTime));

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet" && !lockout)
        {
            lockout = true;
            activated = !activated;
            tempLockout = LockoutTime;
            activeTime = timeToStayActive;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Bullet" && !lockout)
        {
            lockout = true;
            activated = !activated;
            tempLockout = LockoutTime;
            activeTime = timeToStayActive;
        }
    }

}
