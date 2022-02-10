using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float damage;
    public float lifetime;
    public Vector3 dir;

    public Vector3 colStartPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        colStartPos = gameObject.transform.position;

        switch (col.gameObject.tag)
        {
            case "Enemy":
                {
                    //damage the enemy
                    break;
                }
            case "Ground":
                {

                    break;
                }
            case "Switch":
                {
                    //activate the switch
                    break;
                }
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Player"))
            checkPierce();
    }

    void checkPierce()
    {
        float dist = distance(transform.position, colStartPos);
        Debug.Log(dist);
        if (dist < (damage - 1f))
            damage -= 1f;
        else
            Destroy(this.gameObject);
        
        if (damage < 1f)
            Destroy(this.gameObject);
        
    }

    float distance(Vector3 a, Vector3 b)
    {
        float dist = Mathf.Sqrt(Mathf.Pow(a.x - b.x ,2f) + Mathf.Pow(a.y - b.y, 2f) + Mathf.Pow(a.z - b.z, 2f));
        return dist;
    }
}
