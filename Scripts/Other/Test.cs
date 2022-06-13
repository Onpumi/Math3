using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{

	public event UnityAction<string> DownMouse;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void OnMouseDown()
   {
	DownMouse?.Invoke(this.name);
   }

    // Update is called once per frame
    void Update()
    {

	if(Input.GetMouseButtonDown(0)) {
	//   if(this.name == "Test") DownMouse?.Invoke(this.name);
	}
        
    }
}
