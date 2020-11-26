using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToNext : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.tag =="Player")
        {
            GameManager._instance.ShowWin();
            other.gameObject.GetComponent<SimpleCharacterControl>().LockPlayerControl();
        }
    }
}
