using UnityEngine;
using System.Collections;

public class BasketBallBehaviour: MonoBehaviour
{

    //private Texture2D texture;

    private PXCUPipeline.Mode mode=PXCUPipeline.Mode.GESTURE;

    private PXCUPipeline pp;
	
	PXCMGesture.Gesture gestureObject;
	PXCMGesture.GeoNode PrimaryGeonodeObject;
	
	Transform BallTransform;
	Rigidbody BallRigidbody;
	
	Vector3 BallPosition,GravityVector;
	int LimitXlow,LimitXhigh,LimitYlow,LimitYhigh,LimitZlow,LimitZhigh;
	public float GravityValue;
	public float BallSpeed,MaxBallSpeed,MinBallSpeed;
	public string BallName;
	int PoseVCount;
	
	public GUIStyle MyStyle,MyStyle2;
	
	public static bool BallinMotion;
	bool GestureFound,PrimaryGeonodeObjectFound;
	bool InitFailed;
	bool ShowInstructions;

	
    void Start ()
	{
		
        pp=new PXCUPipeline();
        if(pp.Init(mode)==false)
		{
			Debug.Log ("Failed to initialise.");
			InitFailed = true;
		}
		else 
		{
			Debug.Log ("Init was successful.");
			InitFailed = false;
		}
        if(pp.IsDisconnected() == true)
            Debug.Log("The device is disconnected, connect the camera.");
        //int[] size=new int[2];
        //pp.QueryLabelMapSize(size);
        //texture=new Texture2D(size[0],size[1],TextureFormat.ARGB32,false);
        //renderer.material.mainTexture = texture;
		BallName = "BasketBall";
		
		BallTransform = GameObject.Find(BallName).GetComponent<Transform>();
		BallRigidbody = GameObject.Find(BallName).GetComponent<Rigidbody>();
	
    	
		SetLimits();
		BallRigidbody.freezeRotation = true;
		BallinMotion = false;
		GestureFound = false;
		PrimaryGeonodeObjectFound = false;
		GravityValue = -100;
		GravityVector = new Vector3(0,GravityValue,0);
		
		BallSpeed = 1.0f;
		MaxBallSpeed = 2.5f;
		MinBallSpeed = 1.0f;
		PoseVCount = 0;
		
		BallReset();
		
		MyStyle.fontSize = 16;
		MyStyle2.fontSize = 14;
		ShowInstructions = true;
	}
	
    void OnDisable()
    {
        pp.Dispose();
    }

	
    void FixedUpdate ()
    {
		
        if (!pp.AcquireFrame(false)) return;
		
        //if (pp.QueryLabelMapAsImage(texture)) texture.Apply() ;
		
		if(pp.QueryGeoNode(PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY,out PrimaryGeonodeObject) == true)
		{
			PrimaryGeonodeObjectFound = true;
		}
		else
			Debug.Log( "Primary Geonode object not available.");
		
		if (pp.QueryGesture(PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY,out gestureObject) == true)
		{
			Debug.Log( gestureObject.label.ToString() );
			GestureFound = true;
		}
		else
			Debug.Log( "Secondary Geonode object not available");
			
		if (PrimaryGeonodeObjectFound==true && BallinMotion==false)
		{
			PXCMPoint3DF32 point = PrimaryGeonodeObject.massCenterWorld;
			BallPosition.x = -(point.x);
			BallPosition.y = (point.z);
			BallPosition.z = (300 - point.y);
			if(BallPosition.x > LimitXlow && BallPosition.x < LimitXhigh && BallPosition.y > LimitYlow && BallPosition.y < LimitYhigh && BallPosition.z > LimitZlow && BallPosition.z < LimitZhigh)
				BallTransform.position = BallPosition;
		}
			
		if(GestureFound == true && BallinMotion == false)
		{
			if(gestureObject.label == PXCMGesture.Gesture.Label.LABEL_POSE_THUMB_UP)
			{
				if(BallSpeed + 0.01f <= MaxBallSpeed)
					BallSpeed += 0.01f;
			}
			else if(gestureObject.label == PXCMGesture.Gesture.Label.LABEL_POSE_THUMB_DOWN)
			{
				if(BallSpeed - 0.01f >= MinBallSpeed)
					BallSpeed -= 0.01f;
			}
			else if(gestureObject.label == PXCMGesture.Gesture.Label.LABEL_POSE_PEACE)
			{
				PoseVCount++;
			}
		}
		
		if(PoseVCount >= 20)
		{
			PoseVCount = 0;
			Application.LoadLevel(1);
		}
		
		if(Input.GetKey(KeyCode.Space))
			BallReset();
		
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();
		
		
		if(PrimaryGeonodeObjectFound == true && BallinMotion == false && PrimaryGeonodeObject.openness > 60)
		{
				Vector3 Direction;
				Direction.x = BallPosition.x;
				Direction.y = Mathf.Abs(BallPosition.y);
				Direction.z = Mathf.Abs(BallPosition.z);
				BallRigidbody.WakeUp();
				GravityVector.y = GravityValue;
				Physics.gravity = GravityVector;		
				BallRigidbody.useGravity = true;
				BallRigidbody.velocity = Direction * BallSpeed;
				BallinMotion = true;
		}
	
		if(BallinMotion == true && BallRigidbody.position.y < 0.0f)
			BallReset();
		
		pp.ReleaseFrame();
		
		GestureFound = false;
		PrimaryGeonodeObjectFound = false;
	
    }
	
	
	void OnGUI()
	{
		GUI.Label(new Rect(10,8,200,30), "BASKETBALL MODE!",MyStyle);
		GUI.Label(new Rect(10,35,200,40), "Score: " + BasketBallScorer.PlayerScore,MyStyle);
		GUI.Label(new Rect(10,60,200,30),"Speed Meter",MyStyle);
    	BallSpeed = GUI.VerticalSlider(new Rect(20, 85, 100, 200), BallSpeed, MaxBallSpeed, MinBallSpeed);
		if(BallSpeed > MaxBallSpeed)
			BallSpeed = MaxBallSpeed;
		if(BallSpeed < MinBallSpeed)
			BallSpeed = MinBallSpeed;
   		GUI.Label(new Rect(40,160,200,20), BallSpeed.ToString(),MyStyle) ;
		if(GUI.Button(new Rect(10,290,100,30),"Restart"))
			Application.LoadLevel(2);
		if(GUI.Button(new Rect(10, 330, 50, 30),"Quit"))
			Application.Quit();
		
		if(InitFailed == true)
		{
			GUI.Label(new Rect(280,8,1200,30), "Could not initialise the gesture interface. Please check whether the Gesture Camera is connected and restart the application.",MyStyle);
			GUI.Label(new Rect(280,30,1200,30), "If the Gesture Camera is connected, make sure you have the Intel Perceptual Computing SDK installed.",MyStyle);
		}
		
		int top = 40,leftpoint = 280;
		ShowInstructions =  GUI.Toggle(new Rect(Screen.width - 130,5,200,30),ShowInstructions, "Show Instructions");
		if(ShowInstructions == true)
		{
			GUI.Label(new Rect(Screen.width - leftpoint,top + 20,400,30),"Right Hand Closed: Control the ball",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 50,400,30),"Right Hand Open: Throw the ball",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 80,400,30),"Left Hand Thumbs-up: Increase speed",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 110,400,30),"Left Hand Thumbs-down: Decrease speed",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 140,300,30),"Left Hand V-Pose: Change Mode(PaperBall/BasketBall)",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 180,200,30),"Space Bar: Reset Ball Position",MyStyle2);
			GUI.Label(new Rect(Screen.width - leftpoint,top + 210,200,60),"(You can interchange the Right and Left hand controls). Place your hands directly in front of the Gesture Camera around 10 inches away from it. Use your hands to control the ball and score by throwing it into the buckets and rings! Click Restart to restart the session in case the ball does not respond to hand movements.",MyStyle2);
		}
		
		
  	}
	
	
	void BallReset()
	{
		BallPosition.x = 0;
		BallPosition.y = 50;
		BallPosition.z = -30;
		BallTransform.position = BallPosition;
		BallinMotion = false;
		BallRigidbody.WakeUp();
		BallRigidbody.velocity = Vector3.zero;
		BallRigidbody.useGravity = false;
	}
	
	
	void SetLimits()
	{
		LimitXlow = -100;
		LimitXhigh = 100;
		LimitYlow = 5;
		LimitYhigh = 120;
		LimitZlow = -150;
		LimitZhigh = 0;
	}
	
	
	void OnCollisionEnter(Collision other) 
	{
		if(other.collider.tag == "Player")
		{
			BallReset();
		}
    }
}
