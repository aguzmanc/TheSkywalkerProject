using UnityEngine;
using System.Collections;

public class ApplyForcesTest : MonoBehaviour 
{
	public float speed = 10.0f;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	void FixedUpdate()
	{
//		if (Input.GetKey (KeyCode.A)) {
//			this.ActivateMotorA();
//		}
//
//		if (Input.GetKey (KeyCode.S)) {
//			rigidbody.AddForceAtPosition(Vector3.forward, new Vector3(-1,0,0));
//		}
//
//		if (Input.GetKey (KeyCode.D)) {
//			rigidbody.AddForceAtPosition(Vector3.forward, new Vector3(0,1,0));
//		}
	}


	private void ActivateMotorA()
	{
		rigidbody.AddForceAtPosition(Vector3.forward, new Vector3(1,0,0));
	}
}
