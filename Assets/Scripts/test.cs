using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class test : MonoBehaviour
{
    public Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) )
        {Test();}
    }
        public void Test()
    {flowchart.ExecuteBlock("New Game");}
}
