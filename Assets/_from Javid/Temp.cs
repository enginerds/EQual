using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public Behaviour Halo;
    // Start is called before the first frame update
    void Start()
    {
        Halo.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnMouseEnter()
    {
        Halo.enabled = true;

       


    }


    //OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.
    public void OnMouseExit()
    {
        //mousing off a student
        Halo.enabled = false;

    }

}
