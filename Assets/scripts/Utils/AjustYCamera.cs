using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustYCamera : MonoBehaviour
{
#if UNITY_EDITOR
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            float y = 0;
            if(transform.position.y == 0)
            {
                y = 1;
            }

            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
#endif
}
