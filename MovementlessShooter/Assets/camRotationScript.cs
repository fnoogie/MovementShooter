using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camRotationScript : MonoBehaviour
{
    Vector2 screenSize;

    // Start is called before the first frame update
    void Start()
    {
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate towards the camera's direction
        gameObject.transform.rotation = Quaternion.Euler(Mathf.Clamp(Input.mousePosition.y - screenSize.y / 2, -70f, 80f) * -1, Input.mousePosition.x - screenSize.x / 2, 0);
    }
}
