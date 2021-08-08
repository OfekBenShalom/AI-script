using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT CREATED BY Ofek Ben Shalom

//Instruction:
//1.put camera on the enemy and use it for raycast 
//2.use vector3. Distance either if the fps is crouching because the ray will not detect it
//3.if the enemy detect the fps with his ray = his eyes the enemy will change his target to fps target instead the waypoints.

public class AiOfekReal : MonoBehaviour {         //THIS SCRIPT IS CONNECTED TO 'MoveTo' SCRIPT!

	public Transform cam;
	public float RangeOfEyes;       //The minimum vision of the enemy
	public float MaxDistanceEyes;  // The vision distance of the enemy
	
	public GameObject sound;       //Sound of the enemy when he chase you
	public Transform fps;
	public float Run;               //Speed enemy run
	public float Walk;              //Speed enemy walk
	public float countDownScan;     //The seconds the enemy will scan the area after he lose the vision with the player
	public bool UsePatrol;
	public Transform[] waypoints; 
	public float MaxDisWaypoints;   //Distance from the waypoint
	public bool WaitAtPoint;        // If the enemy want to wait for X seconds every point
	public float timeWaiting;       // Amount the enemy will wait in seconds
	public bool Idle;
	public float NumCountingScan; // ITS PUBLIC JUST FOR THE SURVIVAL MODE! THE  DIFFICLTY SCRIPT NEED TO DETECT IT
	public int NumPoints = 0;

	private bool m_startCount;
	private bool m_foward;
	private bool m_reverse;
        private float m_walkSave;
	private GameObject m_hitobject;
	private bool m_InTheSight;
	private bool m_lost;
	private UnityEngine.AI.NavMeshAgent m_navComponent;


	// Use this for initialization
	void Start () {
		//float distance = Vector3.Distance(transform.position,GetComponent<MoveTo>().target.transform.position);
		NumCountingScan = countDownScan;
		m_navComponent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		m_walkSave = Walk;
	}
	
	// Update is called once per frame
	void Update () {  cam.LookAt (fps);
		if (m_lost) {
			m_navComponent.speed = Walk;
		}

		if(UsePatrol){
			WayPointsAndPatroling ();
			}
	
		if (m_startCount) {
			countDownScan -= Time.deltaTime;  // THE ENEMY SEARCH AND SCAN FOR THE PLAYER 
		}

		if (countDownScan <= 0f) {
			m_lost = true;              //THE PLAYER RUN AWAY...
			m_startCount = false;
			countDownScan = NumCountingScan;
            sound.GetComponent<AudioSource>().enabled = false;
			GetComponent<MoveTo> ().target = null; //
		}

		Ray playerAim = cam.GetComponent<Camera> ().ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
		RaycastHit hit;

		//float step = speed * Time.deltaTime;

		if (Physics.Raycast (playerAim, out hit, RangeOfEyes)) {
			m_hitobject = hit.collider.gameObject;

			if (m_hitobject.gameObject.tag == "Player") {  
				PlayerOnSight ();                 //THE PLAYER IS ON SIGTH!
				sound.GetComponent<AudioSource>().enabled = true;
			}
		}
			
		if (Vector3.Distance (fps.transform.position, cam.transform.position) > MaxDistanceEyes  && m_InTheSight) {
			m_startCount = true;
			PlayerOutOfSight();
		}
	}

	void PlayerOnSight(){
		m_InTheSight = true; // ITS BETTER FOR YOU TO RUN!
		m_lost = false; //THE PLAYER DOESNT LOST ANYMORE!
		GetComponent<MoveTo> ().target = fps;  //FPS IS THE TARGET NOW INSTEAD WAYPOINT THE PATROL IS NOT RELEVANTE ANYMORE 
		m_navComponent.speed = Run;
	}

		void PlayerOutOfSight(){
		m_InTheSight = false;
	}

	void WayPointsAndPatroling(){
		if (m_lost) { //Lost can be after the target that got lost and before even see the target
			if (m_foward) {
				WayForword ();
				m_reverse = false;
			}
			if (!m_foward) {
				m_reverse = true;
				WayReverse ();
			}
		}

	}
		

	void WayForword()   //FORWORD
	{
		GetComponent<MoveTo> ().target = waypoints [NumPoints];  //Target is the all amount of the waypoints 
		if (Vector3.Distance (transform.position, GetComponent<MoveTo> ().target.transform.position) < MaxDisWaypoints) {
			if (WaitAtPoint) {
				StartCoroutine (speedChange ());
			}
			if (NumPoints == waypoints.Length - 1) {
				m_foward = false;
				NumPoints--;
			}
			  else {
				NumPoints++;
				WayForword ();
			}
		}
	}

	void WayReverse()   //REVERSE
	{
		GetComponent<MoveTo> ().target = waypoints [NumPoints];
		if (Vector3.Distance (transform.position, GetComponent<MoveTo> ().target.transform.position) < MaxDisWaypoints) {
			if (WaitAtPoint) {
				StartCoroutine (speedChange ());
			}
			if (NumPoints == 0) {
				NumPoints++;
				m_foward = true;
				WayForword ();
			}
			 else {
				NumPoints--;
				WayReverse ();
			}
		}
	}

	IEnumerator speedChange(){
		if (m_foward && Idle) {
            Walk = 0;
			yield return new WaitForSeconds (timeWaiting);
            Walk = m_walkSave;
			Idle = false;
			WayForword ();
		}
		if (m_reverse && Idle) {
            Walk = 0;
            yield return new WaitForSeconds (timeWaiting);
            Walk = m_walkSave;
            Idle = false;
			WayReverse ();
		}
	}
	

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "idleW" && !m_InTheSight && WaitAtPoint) {
			Idle = true;
		}
	}
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "idleW")
        {
            Idle = false;
        }
    }

	
}
