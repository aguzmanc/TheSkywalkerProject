using UnityEngine;
using System.Collections;

public class FreeObstacle : MonoBehaviour 
{	
	void Start () 
	{
		Vector3 initialTorque = new Vector3 (Random.Range(0,10),Random.Range(0,10),Random.Range(0,10));
		rigidbody.AddTorque (initialTorque, ForceMode.VelocityChange);

		Vector3 initialForce = new Vector3 (Random.Range(0,100),Random.Range(0,100),Random.Range(0,100));
		rigidbody.AddForce (initialForce);
	}
	
	void Update () 
	{
		
	}


}
