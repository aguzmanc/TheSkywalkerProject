using UnityEngine;
using System.Collections;

public class ROBOT_PARAMETRIZADO : MonoBehaviour 
{
	private static int TOTAL_LAST_MAGNITUDES = 20;
	private static float MIN_TORQUE_MAGNITUDE = 0.1f;

	// parameters
	public float StableThreshold = 0.01f;
	public float MotorForce = 1.0f;
	public float MaxTimeBeforeMotorChange = 3.0f;
	public float MinTimeBeforeMotorSwitchDirection = 0.5f;
	public bool StartWithDisplacementForce = false;
	
	private GameObject _motorX, _motorX2;
	private GameObject _motorY;
	private GameObject _motorZ;
	private GameObject _body;
	
	private float[] _lastMagnitudes;
	private int _lastMagnitudeIndex = 0;
	
	private float _timeToMotorChange;
	
	private float _timeToMotorSwitch;
	
	void Start () 
	{
		_body = transform.FindChild ("MainBody").gameObject;

		_motorX = _body.transform.FindChild ("MotorX").gameObject;
		_motorX2 = _body.transform.FindChild ("MotorX_2").gameObject;
		_motorY = _body.transform.FindChild ("MotorY").gameObject;
		_motorZ = _body.transform.FindChild ("MotorZ").gameObject;
		
		_currentStabilizerMotor = _motorX;
		
		_lastMagnitudes = new float[TOTAL_LAST_MAGNITUDES];
		for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
			_lastMagnitudes [i] = 9999;

		Invoke ("ApplyStartForce", 1.0f);
	}
	
	void Update () 
	{
	}

	void ApplyStartForce() 
	{
		Vector3 torque = new Vector3 (Random.Range(-300,300),Random.Range(-300,300),Random.Range(-300,300));
		_body.rigidbody.AddRelativeTorque(torque, ForceMode.Force);

		if (StartWithDisplacementForce)
			_body.rigidbody.AddForce(new Vector3 (Random.Range(-300,300),Random.Range(-300,300),Random.Range(-300,300)));

	}
	
	bool _stabilizing = false;
	
	bool _isForward = true;
	float _lastMagnitude = 999;
	GameObject _currentStabilizerMotor;
	
	void FixedUpdate()
	{
		if (_body.rigidbody.angularVelocity.magnitude < MIN_TORQUE_MAGNITUDE) {
			_body.renderer.material.color = Color.green;
			return;
		}
		else 
			_body.renderer.material.color = Color.red;


		if (_lastMagnitude == 999) {
			_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;
			return;
		}
		
		bool isIncreasing = (_body.rigidbody.angularVelocity.magnitude > _lastMagnitude);
		
		_lastMagnitudes[_lastMagnitudeIndex++] = Mathf.Abs(_lastMagnitude - _body.rigidbody.angularVelocity.magnitude);
		_lastMagnitudeIndex %= TOTAL_LAST_MAGNITUDES;
		
		_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;
		
		//		Debug.Log (_lastMagnitude);
		_timeToMotorSwitch -= Time.fixedDeltaTime;
		
		setMotorActive (_motorX, false, true);
		setMotorActive (_motorX2, false, true);
		setMotorActive (_motorY, false, true);
		setMotorActive (_motorZ, false, true);
		
		if (false == _stabilizing)
			return;
		
		if (isIncreasing) {
			if(_timeToMotorSwitch <=  0.0f) {
				_isForward = !_isForward;
				_timeToMotorSwitch = MinTimeBeforeMotorSwitchDirection;
			}
		}
		
		setMotorActive (_currentStabilizerMotor, true, _isForward);
		
		_timeToMotorChange -= Time.fixedDeltaTime;
		
		float sum = 0.0f;
		for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
			sum+=_lastMagnitudes[i];
		sum /= TOTAL_LAST_MAGNITUDES;
		
		if (sum < StableThreshold || _timeToMotorChange <= 0.0f) {
			if(_currentStabilizerMotor.Equals(_motorX))
				_currentStabilizerMotor = _motorY;
			else if(_currentStabilizerMotor.Equals(_motorY))
				_currentStabilizerMotor = _motorZ;
			else if(_currentStabilizerMotor.Equals(_motorZ))
				_currentStabilizerMotor = _motorX;
			
			for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
				_lastMagnitudes [i] = 9999;
			
			_timeToMotorChange = MaxTimeBeforeMotorChange;
		}
	}
	
	private void setMotorActive(GameObject motor, bool active, bool forward)
	{
		motor.renderer.material.color = active ? Color.green:Color.white;
		
		if (active) {
			Vector3 torque = Vector3.forward;
			float mult = forward?MotorForce:-MotorForce;
			
			if(motor.Equals(_motorX))
				torque = Vector3.right * mult;
			
			if(motor.Equals(_motorX2))
				torque = Vector3.left * mult;
			
			if(motor.Equals(_motorY))
				torque = Vector3.up * mult;
			
			if(motor.Equals(_motorZ))
				torque = Vector3.forward * mult;
			
			_body.rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);
		}
	}
	
	
	void OnMouseDown()
	{
		Debug.Log ("abc");

		GameObject [] robots = GameObject.FindGameObjectsWithTag("parametrized_robot");

		foreach(GameObject robot in robots) {
			robot.GetComponent<ROBOT_PARAMETRIZADO>().StartStabilizing();
		}
	}


	public void StartStabilizing()
	{
		_stabilizing = true;
	}
}
