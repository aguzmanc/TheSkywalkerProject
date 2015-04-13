using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour 
{
	public GameObject Target;

	void Start () 
	{
	
	}
	
	void Update () 
	{
		Vector3 p = this.Target.transform.position;

		transform.position.Set (p.x, transform.position.y, p.z);
	}
}
