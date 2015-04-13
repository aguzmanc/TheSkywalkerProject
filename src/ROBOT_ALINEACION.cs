using UnityEngine;
using System.Collections;

public class ROBOT_ALINEACION : MonoBehaviour 
{
	private static int RECORD_LENGTH = 20;
	private static float MIN_TORQUE_MAGNITUDE = 0.1f;
	
	// parameters
	public float StableThreshold = 0.01f;
	public float MotorForce = 1.0f;
	public float MaxTimeBeforeMotorChange = 3.0f;
	public float MinTimeBeforeMotorSwitchDirection = 0.5f;
	public bool StartWithDisplacementForce = false;

	public GameObject Target;
	
	private GameObject _motorX, _motorX2;
	private GameObject _motorY;
	private GameObject _motorZ;
	private GameObject _body;
	
	private float[] _lastMagnitudes;
	private float[] _lastAngles;
	private int _lastMagnitudeIndex = 0;
	private int _lastAngleIndex = 0;
	
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
		
		_lastMagnitudes = new float[RECORD_LENGTH];
		_lastAngles = new float[RECORD_LENGTH];

		for (int i=0; i<RECORD_LENGTH; i++) {
			_lastMagnitudes [i] = 9999;
			_lastAngles[i] = 9999;
		}
		
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

		_stabilizing = false;
		_aligning = true;
		
	}

	// status
	bool _stabilizing = false;
	bool _aligning = false;

	// action of the motors
	bool _isForward = true;
	float _lastMagnitude = 999; // for stabilizing
	GameObject _currentStabilizerMotor;

	float _lastAngle = 360.0f; // for aligning with target
	
	void FixedUpdate()
	{
		setMotorActive (_motorX, false, true);
		setMotorActive (_motorX2, false, true);
		setMotorActive (_motorY, false, true);
		setMotorActive (_motorZ, false, true);

		if (_body.rigidbody.angularVelocity.magnitude < MIN_TORQUE_MAGNITUDE && _stabilizing) {
			_body.renderer.material.color = Color.blue;
			_stabilizing = false;
			_aligning = true;
		} 
		else if (Quaternion.Angle (_body.transform.rotation, Quaternion.Euler (Target.transform.position - _body.transform.position)) < 1.0f && _aligning) {
			_body.renderer.material.color = Color.green;
			_stabilizing = false;
			_aligning = false;
		}
//		else{
//			_body.renderer.material.color = Color.red;
//			_stabilizing = true;
//			_aligning = false;
//		}
		
		if (_lastMagnitude == 999) {
			_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;
			return;
		}

		if (_lastAngle == 360.0f) {
			if(Target != null){
				_lastAngle = Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(Target.transform.position - _body.transform.position));
			}

			return;
		}

		bool isIncreasing = false;

		if (_stabilizing) {
			isIncreasing = (_body.rigidbody.angularVelocity.magnitude > _lastMagnitude);
			
			_lastMagnitudes[_lastMagnitudeIndex++] = Mathf.Abs(_lastMagnitude - _body.rigidbody.angularVelocity.magnitude);
			_lastMagnitudeIndex %= RECORD_LENGTH;
			
			_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;
			
			_timeToMotorSwitch -= Time.fixedDeltaTime;

			if (isIncreasing) {
				if (_timeToMotorSwitch <= 0.0f) {
					_isForward = !_isForward;
					_timeToMotorSwitch = MinTimeBeforeMotorSwitchDirection;
				}
			}
			
			setMotorActive (_currentStabilizerMotor, true, _isForward);
			
			_timeToMotorChange -= Time.fixedDeltaTime;
			
			float sum = 0.0f;
			for (int i=0; i<RECORD_LENGTH; i++)
				sum += _lastMagnitudes [i];
			sum /= RECORD_LENGTH;
			
			if (sum < StableThreshold || _timeToMotorChange <= 0.0f) {
				if (_currentStabilizerMotor.Equals (_motorX))
					_currentStabilizerMotor = _motorY;
				else if (_currentStabilizerMotor.Equals (_motorY))
					_currentStabilizerMotor = _motorZ;
				else if (_currentStabilizerMotor.Equals (_motorZ))
					_currentStabilizerMotor = _motorX;
				
				for (int i=0; i<RECORD_LENGTH; i++)
					_lastMagnitudes [i] = 9999;
				
				_timeToMotorChange = MaxTimeBeforeMotorChange;
			}
		} else if (_aligning) {
			isIncreasing = (Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(Target.transform.position - _body.transform.position)) > _lastAngle);

			_lastAngles[_lastAngleIndex++] = Mathf.Abs(_lastAngle - Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(Target.transform.position - _body.transform.position)));
			_lastAngleIndex %= RECORD_LENGTH;
			
			_lastAngle = Quaternion.Angle(_body.transform.rotation, Quaternion.Euler(Target.transform.position - _body.transform.position));
			
			_timeToMotorSwitch -= Time.fixedDeltaTime;
			
			if (isIncreasing) {
				if (_timeToMotorSwitch <= 0.0f) {
					_isForward = !_isForward;
					_timeToMotorSwitch = MinTimeBeforeMotorSwitchDirection;
				}
			}
			
			setMotorActive (_currentStabilizerMotor, true, _isForward);
			
			_timeToMotorChange -= Time.fixedDeltaTime;
			
			float sum = 0.0f;
			for (int i=0; i<RECORD_LENGTH; i++)
				sum += _lastAngles [i];
			sum /= RECORD_LENGTH;
			
			if (sum < StableThreshold || _timeToMotorChange <= 0.0f) {
				if (_currentStabilizerMotor.Equals (_motorX))
					_currentStabilizerMotor = _motorY;
				else if (_currentStabilizerMotor.Equals (_motorY))
					_currentStabilizerMotor = _motorZ;
				else if (_currentStabilizerMotor.Equals (_motorZ))
					_currentStabilizerMotor = _motorX;
				
				for (int i=0; i<RECORD_LENGTH; i++)
					_lastAngles [i] = 9999;
				
				_timeToMotorChange = MaxTimeBeforeMotorChange;
			}
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
//		GameObject [] robots = GameObject.FindGameObjectsWithTag("parametrized_robot");
//		
//		foreach(GameObject robot in robots) {
//			robot.GetComponent<ROBOT_PARAMETRIZADO>().StartStabilizing();
//		}
	}
	
	
	public void StartStabilizing()
	{
		_stabilizing = true;
	}
}
