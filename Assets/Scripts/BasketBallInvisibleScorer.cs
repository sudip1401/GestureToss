using UnityEngine;
using System.Collections;

public class BasketBallInvisibleScorer : MonoBehaviour {

	Transform BallTransform;
	Rigidbody BallRigidbody;
	Vector3 BallPosition;
	string BallName;
	
	void Start()
	{
		BallName = "BasketBall";
		BallTransform = GameObject.Find(BallName).GetComponent<Transform>();	
		BallRigidbody = GameObject.Find(BallName).GetComponent<Rigidbody>();
	}
	
	void OnTriggerExit(Collider obj)
	{
		BasketBallScorer.PlayerScore++;
		if(obj.tag == "Player")
		{
			Debug.Log ("SCORE!" + " Score = " + BasketBallScorer.PlayerScore);
			//BallReset();
		}
	}
	
	
	void BallReset()
	{
		BasketBallBehaviour.BallinMotion = false;
		BallRigidbody.WakeUp();
		BallRigidbody.velocity = Vector3.zero;
		BallRigidbody.useGravity = false;
		BallPosition.x = 0;
		BallPosition.y = 50;
		BallPosition.z = -30;
		BallTransform.position = BallPosition;
	}
}
