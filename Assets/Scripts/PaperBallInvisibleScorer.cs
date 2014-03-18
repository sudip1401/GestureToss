using UnityEngine;
using System.Collections;

public class PaperBallInvisibleScorer: MonoBehaviour {
	
	Transform BallTransform;
	Rigidbody BallRigidbody;
	Vector3 BallPosition;
	string BallName;
	
	void Start()
	{
		BallName = "PaperBall";
		BallTransform = GameObject.Find(BallName).GetComponent<Transform>();	
		BallRigidbody = GameObject.Find(BallName).GetComponent<Rigidbody>();
	}
	
	void OnTriggerExit(Collider obj)
	{
		PaperBallScorer.PlayerScore++;
		if(obj.tag == "Player")
		{
			Debug.Log ("SCORE!" + " Score = " + PaperBallScorer.PlayerScore);
			//BallReset();
		}
	}
	
	void BallReset()
	{
		PaperBallBehaviour.BallinMotion = false;
		BallRigidbody.WakeUp();
		BallRigidbody.velocity = Vector3.zero;
		BallRigidbody.useGravity = false;
		BallPosition.x = 0;
		BallPosition.y = 50;
		BallPosition.z = -30;
		BallTransform.position = BallPosition;
	}
}