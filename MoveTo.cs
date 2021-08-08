using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {
	public float deathDistance = 0.5f;
	public float distanceAway;
	public Transform thisObject;
	public Transform target;
	//public Transform Target;
	public bool DestroyMe;

	private UnityEngine.AI.NavMeshAgent navComponent;

	void Start() 
	{
		navComponent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	void Update() 
	{

		float dist = Vector3.Distance(target.position, transform.position);

		if(target)
		{
			navComponent.SetDestination(target.position);
		}

		else
		{
			if (target = null) {
				target = this.gameObject.GetComponent<Transform> ();
			}
		}
		if (dist <= deathDistance)
		{
			//KILL PLAYER
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Destroy" && DestroyMe) {
			Destroy (thisObject);
		}
	}
}
