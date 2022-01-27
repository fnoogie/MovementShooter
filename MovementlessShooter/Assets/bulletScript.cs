using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float damage;
    public float lifetime;

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
        switch(col.gameObject.tag)
        {
            case "Enemy":
                {
                    break;
                }
            case "Ground":
                {
                    break;
                }
            case "Switch":
                {
                    break;
                }
        }
        if(!col.gameObject.tag.Equals("Bullet"))
            Destroy(this.gameObject);
    }
}
