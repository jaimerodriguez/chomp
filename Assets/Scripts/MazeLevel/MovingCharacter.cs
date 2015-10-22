using UnityEngine;
using System.Collections;
using MonsterGame;

public class MovingCharacter : MonoBehaviour {

    protected new Transform transform;
    // Use this for initialization

    protected void Awake()
    {
        transform = GetComponent<Transform>();
    }

    protected void Start () {
        
    }
	
    
	// Update is called once per frame
	void Update () {
	
	}

    protected Direction GetNormalizedDirection ( int horizontal, int vertical )
    {
           if ( horizontal != 0 )
            return (horizontal == -1) ? Direction.Left : Direction.Right;
           else if (vertical != -0)
            return (vertical == -1) ? Direction.Down : Direction.Up;

            Debug.Assert(false, "invalid direction");
            return Direction.Right; 
    }

    protected void Rotate( Direction currentDirection )
    {
        float lerpDelta = 0.34f;
#if LERPROTATION && DEBUG
        if ( lerpTarget != Quaternion.identity )
        {
            Debug.Log("Mid rotation: " + rotationTime); 
        }
#endif
#if LERPROTATION
        rotationTime = 0.0f + lerpDelta;
#endif
        switch ( currentDirection)
            {
                case Direction.Right:
#if !LERPROTATION
                transform.rotation = ROTATE_RIGHT;
#else
                        lerpTarget = ROTATE_RIGHT;
                        transform.rotation = Quaternion.Lerp(transform.rotation, lerpTarget, rotationTime);
                        Debug.Log("lerp started right ");
#endif
                break;
                case Direction.Left:
#if !LERPROTATION
                transform.rotation = ROTATE_LEFT;
#else
                        lerpTarget = ROTATE_LEFT;
                        transform.rotation = Quaternion.Lerp(transform.rotation, lerpTarget, rotationTime);
                        Debug.Log("lerp started left ");
#endif
                break; 
            case Direction.Down:
#if !LERPROTATION
                transform.rotation = ROTATE_DOWN;
#else
                        lerpTarget = ROTATE_DOWN;
                        transform.rotation = Quaternion.Lerp(transform.rotation, lerpTarget, rotationTime);
                        Debug.Log("lerp started down");
#endif         
                 break;
            case Direction.Up:
#if !LERPROTATION
                transform.rotation = ROTATE_UP;
#else
                        lerpTarget = ROTATE_UP;
                        transform.rotation = Quaternion.Lerp(transform.rotation, lerpTarget, rotationTime);
                        Debug.Log("lerp started up");
#endif
                break; 

        }                           
#if DEBUG
                //used for DebugHud 
                GameState.Instance.PlayerDirection = currentDirection;
#endif
    }

#if LERPROTATION
     protected float rotationTime; 
#endif


    static Quaternion ROTATE_RIGHT = Quaternion.Euler(Vector3.zero);
    static Quaternion ROTATE_LEFT = Quaternion.Euler(new Vector3(0, 180, 0));
    static Quaternion ROTATE_UP = Quaternion.Euler(new Vector3(0, 0, 90));
    static Quaternion ROTATE_DOWN = Quaternion.Euler(new Vector3(0, 180, 270));
}
