using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHowToPlay : MonoBehaviour
{
    public GameObject helpPanel1;
    public GameObject helpPanel2;
    public GameObject helpPanel3;
    public GameObject helpPanel4;
    // Start is called before the first frame update
    void Start()
    {
        helpPanel1.SetActive(false);
        helpPanel2.SetActive(false);
        helpPanel3.SetActive(false);
        helpPanel4.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
