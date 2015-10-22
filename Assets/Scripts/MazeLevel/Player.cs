//TODO: Copy right 
//TODO: Polish LERP rotation.. Right now it is too simplistic and does not take into account incoming direction.. 
//#define LERPROTATION 

//PLAYERHASRAILS keeps the Player centered when directions change, that way user does not have to be 100% accurate 
#define PLAYERHASRAILS 
#define USECNINPUT 


using UnityEngine;
using System.Collections;
using MonsterGame;

public class Player : MovingCharacter 
{

    #region private members 
    private Animator animator;
    private Rigidbody2D rb2d;     
    new BoxCollider2D collider; 
    Axis currentAxis; 
    int lastDirection = 0;
    float speed = 2.0f;
    private float UserPreferredMaxSpeed;  

#endregion

#region public /designer configurable 
    public bool continueWalkingOnLastKey;
    public int pointsPerFood;
    public float MaxSpeed;
    public float MinSpeed;
       

#endregion

    new void Awake ()
    {
        base.Awake(); 
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
       

    }
    // Use this for initialization
    new void Start()
    {
        base.Start();
        forceMobileInput = PlatformHelper.UseJoystick;              
    }

    void LevelReset()
    {
        CharacterReset();
        tileSizeX = GameState.Instance.MazeConfiguration.TileWidth;
        tileSizeY = GameState.Instance.MazeConfiguration.TileHeight;
        VerticalThreshold = tileSizeX * .10f;
        HorizontalThreshold = tileSizeY * .10f;
    
        RailsLowVerticalThreshold = tileSizeY * 0.25f;
        RailsHighVerticalThreshold = tileSizeY * 0.75f;
        RailsLowHorizontalThreshold = tileSizeX * 0.25f;        
        RailsHighHorizontalThreshold = tileSizeX * 0.75f;
    }


    float lastXPHorizontal = 0f;
    float lastXPVertical = 0f;
    float tileSizeX;
    float tileSizeY;
    bool isMultiDirectional = false ;
    bool forceMobileInput = true;  

    void GetInput ( out int horizontal , out int vertical )
    {
        horizontal = 0;
        vertical = 0;
        if (!forceMobileInput && !isMultiDirectional && Application.platform != RuntimePlatform.WSAPlayerARM)
        {
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");
        }
        else
        {
#if USECNINPUT
            float h = CnControls.CnInputManager.GetAxis("Horizontal") ;
            float v = CnControls.CnInputManager.GetAxis("Vertical");
            const float VerticalThreshold = 0.05f;
            const float HorizontalThreshold = 0.05f;
            float DeltaOverrideThreshold = 0.10f;
#else
            float h = UnitySampleAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = UnitySampleAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxisRaw("Vertical");
            const float VerticalThreshold = 0.05f;
            const float HorizontalThreshold = 0.05f;
            float DeltaOverrideThreshold = 0.10f; 
#endif

            float hDelta = Mathf.Abs(h - lastXPHorizontal);
            float vDelta = Mathf.Abs(v - lastXPVertical);
            if (hDelta < HorizontalThreshold && vDelta < VerticalThreshold)
            {
                Debug.Log(string.Format("Skipping delta {0},{1}", hDelta, vDelta));
                return;
            }

            if (h != 0f || v != 0f)
            {
                Debug.Log(string.Format("Raw: {0}, {1}", h, v));
                float absH = Mathf.Abs(h);
                float absV = Mathf.Abs(v);
               
                if ( Mathf.Abs(hDelta - vDelta) >= DeltaOverrideThreshold )
                {
                    if (hDelta > vDelta)
                        vDelta = 0.0f;
                    else    
                        hDelta = 0.0f;
                    Debug.Log("DeltaOverride Threshold"); 
                }
                if ( 
                    (!isMultiDirectional &&( hDelta > vDelta))  || 
                    (isMultiDirectional && ( hDelta > HorizontalThreshold )))  

                {
                    horizontal = (absH > HorizontalThreshold) ?
                                    ((h > 0f) ? 1 : -1) : 0;
                    //lastXPHorizontal = h;
                    //lastXPVertical = v;
                }
                else if ( (!isMultiDirectional && (hDelta < vDelta)) ||
                   (isMultiDirectional && (vDelta> VerticalThreshold)))
                {
                    vertical = (absV > VerticalThreshold) ?
                                    ((v > 0f) ? 1 : -1) : 0;
                    //lastXPVertical = v;
                    //lastXPHorizontal = h;
                }
                Debug.Log(string.Format("XP: {0}, {1}", horizontal, vertical));
            }
        }
    }

    /// <summary>
    /// GetEndPosition takes the current coordinates for the player, and the vertical and horizontal direction and gives out the next frame position. It also gives out data detecting that we are at the end of the tile (there fore at a collision on the frame)
    /// </summary>
    /// <param name="start">Current Player coordinates</param>
    /// <param name="tile"> unused //TODO:Remove </param>
    /// <param name="horizontal"> </param>
    /// <param name="vertical"></param>
    /// <param name="useBounds"></param>
    /// <param name="bounds"></param>
    /// <param name="ret"> next Position </param>
    /// <param name="isEnd"> did we reach the end of bounds </param>
    void GetEndPosition(Vector2 start, Vector2 unused, int horizontal, int vertical, bool useBounds, Vector2 bounds, out Vector2 ret, out bool isEnd  )
    {
        // move towards end of square... 
        ret = start;
        isEnd = false;
       
        float deltaTime = Time.deltaTime;

        //force it in case debugger has a large delta time 
        deltaTime = 1 / 60f;

        float endX = start.x + (horizontal * tileSizeX * deltaTime * speed);
        float endY = start.y + (vertical * tileSizeY * deltaTime * speed);
       
        if (!useBounds)
        {
            ret = new Vector2(endX, endY);
        } 
        else
        {
            if (horizontal == Left)
            {
               // bounds.x += (tileSizeX * .1f); 
                endX = Mathf.Max(endX, bounds.x);
                ret = new Vector2(endX, endY);
                //  isEnd = Mathf.Abs(endX - bounds.x) <= HorizontalThreshold;
                isEnd = (endX == bounds.x);
                
            }
            else if (horizontal == Right)
            {
                // bounds.x -= (tileSizeX * .1f);
                endX = Mathf.Min(endX, bounds.x);
                ret = new Vector2(endX, endY);                 
                isEnd = (endX == bounds.x);
                  
            }
            else if (vertical == Down)
            {
                endY = Mathf.Max(endY, bounds.y);
                ret = new Vector2(endX, endY);                
                isEnd = (endY == bounds.y); 
            }
            else if (vertical == Up)
            {                 
                endY = Mathf.Min(endY, (bounds.y));
                ret = new Vector2(endX, endY);                 
                isEnd = (endY == bounds.y);
            }
        }        
    }

    int GetNewDirection ( int horizontal, int vertical )
    {
        return (horizontal == 0) ? (vertical * VerticalAxis) : (horizontal * HorizontalAxis );
    }

    const int VerticalAxis = 2;
    const int HorizontalAxis = 3;
    const int Left = -1;
    const int Right = 1;
    const int Up = 1;
    const int Down = -1; 

    float VerticalThreshold;
    float HorizontalThreshold;
    float RailsLowVerticalThreshold;
    float RailsLowHorizontalThreshold;
    float RailsHighVerticalThreshold;
    float RailsHighHorizontalThreshold;


    bool TryMove ( int horizontal, int vertical)
    {
        bool sameDirection = false;
#if PLAYERHASRAILS        
         Vector2 start = transform.position;
        Vector2 railed;
        Vector2 startLocal = transform.localPosition;

         float offsetTileX = startLocal.x % tileSizeX;
         float offsetTileY = startLocal.y % tileSizeY;

         float absOffsetX = Mathf.Abs(offsetTileX);
         float absOffsetY = Mathf.Abs(offsetTileY);

         if (vertical != 0)
        {
            if (absOffsetX < RailsLowHorizontalThreshold)
            {
                startLocal.x -= absOffsetX;
            }
            else if (absOffsetX > RailsHighHorizontalThreshold)
            {
                startLocal.x += (tileSizeX - absOffsetX);
            }
            else
            {  
                vertical = 0;
                Debug.Log("Cancelling vertical move"); 
            }
        }

        if (horizontal != 0)
        {
            if (absOffsetY < RailsLowVerticalThreshold)
            {
                startLocal.y += absOffsetY;
            }
            else if (absOffsetY > RailsHighVerticalThreshold)
            {
                startLocal.y -= (tileSizeY - absOffsetY);
            }
            else
            {
                horizontal = 0;
                Debug.Log("cancel horizontal");
            }
        }


        railed = transform.parent.TransformPoint(startLocal);
#endif

        int newDirection = GetNewDirection(horizontal, vertical);

        if (horizontal == 0 && vertical == 0 
            && continueWalkingOnLastKey && lastDirection != 0)

        {
            sameDirection = true;
            speed = Mathf.Min(UserPreferredMaxSpeed, speed + 1.0f);
            GetNormalizedAxis(lastDirection, out horizontal, out vertical);
        }
        else
        {
            if (newDirection == lastDirection)
            {
                sameDirection = true;
            }
        }

        if (horizontal != 0 || vertical != 0)
        {

            float deltaTime = Time.deltaTime;
            Debug.Assert(horizontal == -1 || horizontal == 1 || vertical == 1 || vertical == -1);

            Vector2 tile = GameState.Instance.MazeConfiguration.GetRowColumn(railed);
            Vector2 targetTile; 
            targetTile = GameState.Instance.MazeConfiguration.GetTileWithOffset(tile, horizontal, vertical);
            bool isObstacle = GameState.Instance.MazeConfiguration.IsObstacle(targetTile);
            Vector2 end = railed + Vector2.zero;
            Vector2 bounds = Vector2.zero;
            bool isEnd = false;

            
           
            if (isObstacle)
            {
                if (sameDirection)
                {
                    bounds = GameState.Instance.MazeConfiguration.TileToWorld(tile);
                    GetEndPosition(railed, tile, horizontal, vertical, true, bounds, out end, out isEnd);
                }
                else
                {
                    if (lastDirection != 0 && continueWalkingOnLastKey )
                    {
                        GetNormalizedAxis(lastDirection, out horizontal, out vertical);
                        tile = GameState.Instance.MazeConfiguration.GetRowColumn(transform);
                        targetTile = GameState.Instance.MazeConfiguration.GetTileWithOffset(tile, horizontal, vertical);
                        Debug.Assert(targetTile.x != tile.x || targetTile.y != tile.y, "SAME TILE after offset");  
                        isObstacle = GameState.Instance.MazeConfiguration.IsObstacle(targetTile);
                        if (isObstacle)
                        {
                            bounds = GameState.Instance.MazeConfiguration.TileToWorld(tile);
                            GetEndPosition(transform.position, tile, horizontal, vertical, true, bounds, out end, out isEnd);
                        }
                        else
                        {
                             
                            GetEndPosition(transform.position, targetTile, horizontal, vertical, false, bounds, out end, out isEnd);
                            
                            newDirection = lastDirection;
                            sameDirection = true;
                        }
                    }
                    else
                    {
                        Debug.Assert(speed == MinSpeed);
                        return false ;
                    }
                }
            }
            else
            {
                GetEndPosition(railed, targetTile, horizontal, vertical, false, bounds, out end, out isEnd);
            }

            if (isEnd)
            {
                animator.SetTrigger("playerStop");
                lastDirection = 0;
                speed = MinSpeed;
                return false  ;
            }

            GameState.Instance.SetPlayerCoords(end);
            
            rb2d.position = end;
             
            animator.SetTrigger("playerMove");
            SoundManager.Instance.PlayMove();

            

            if (!sameDirection)
            {
                Rotate(GetNormalizedDirection (horizontal, vertical) ); 
            }
#if LERPROTATION
            else if ( rotationTime != -1.0f  )
            {
                rotationTime += lerpDelta;
                transform.rotation = Quaternion.Lerp(transform.rotation, lerpTarget, rotationTime);
                if (rotationTime >= 1.0f)
                {
                    Debug.Log("lerp ended"); 
                    lerpTarget =  Quaternion.identity;
                    rotationTime = -1.0f; 
                }  
                
            }
#endif
            if (newDirection != 0)
                lastDirection = newDirection;

            return true; 
        }

#if DEBUG
        else
        {
         //    Debug.Log("No move"); 
        }
#endif 
        return false; 
    }

    
    // Update is called once per frame
    // void FixedUpdate()

    void Update () 
    {
		if(GameState.Instance == null || GameState.Instance.MazeConfiguration == null)
			return;

        if (!GameState.Instance.IsRunning && 
            !GameState.Instance.IsGameOver )
        {   
            //TODO: is this needed?           
            animator.SetTrigger("playerStop");
            return; 
        }

#if REMOVEME
        if ( doOnce )
        {
            for (float y = 0f; y < GameState.Instance.MazeConfiguration.NumRows; y++)
            {
                for (float x = 0f; x < GameState.Instance.MazeConfiguration.NumColumns; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    Vector2 v = GameState.Instance.MazeConfiguration.TileToWorld(pos);

                    Debug.Log(string.Format("{0}: {1},{2}", pos, v.x, v.y));
                }
            }
            doOnce = false; 
        }
#endif

        int horizontal, vertical;
        GetInput(out horizontal, out vertical );

        bool done = !(isMultiDirectional && (horizontal != 0) && (vertical != 0));

        if ( done)
        {
            if (horizontal != 0)
                vertical = 0;
             
            
            TryMove(horizontal, vertical);
        }
        else
        {
            Debug.Log("multiple values!!"); 
            int localhorizontal = horizontal;
            int localvertical = vertical;

            if (localhorizontal != 0 && localvertical != 0)
            {
                if (lastDirection % VerticalAxis == 0)
                {
                    Debug.Log("multi: try horizontal first"); 
                    localvertical = 0;
                }
                else if (lastDirection % HorizontalAxis == 0)
                {
                    Debug.Log("multi: try horizontal first");
                    localhorizontal = 0;
                }
            }                                 

            Vector2 tile = GameState.Instance.MazeConfiguration.GetRowColumn(transform);
            Vector2 targetTile = GameState.Instance.MazeConfiguration.GetTileWithOffset(tile, localhorizontal, localvertical);
            bool isObstacle = GameState.Instance.MazeConfiguration.IsObstacle(targetTile);
            if (!isObstacle && !done)  // tries new direction first, but only if we know it will work 
            {
                Debug.Log("multi: switching direction ");
                TryMove(localhorizontal, localvertical);                
            }
            else
            {
                Debug.Log("multi: going in w/ default delta");
                TryMove(horizontal, vertical); 
            }                            
        }                    
    }
 

    void GetNormalizedAxis ( int lastDirection, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0; 
        if (lastDirection == -3)
        {
            horizontal = -1;            
        }
        else if (lastDirection == 3)
        {
            horizontal = 1;             
        }
        else if (lastDirection == -2)
        {
            vertical = -1;             
        }
        else if (lastDirection == 2)
        {
            vertical = 1;             
        } 
    }

    bool isFlipping = false;
  
    Quaternion lerpTarget = Quaternion.identity;
   

    IEnumerator FlipOnDie()
    {
        
        float factor = 3f; 
        for (float f = 1f; f < 60/factor; f += 1)
        {
            transform.Rotate(new Vector3(0, 0, 1), 6*factor);
        }

        for (float f = 1f; f < 120/factor; f += 1)
        {
            transform.Rotate(new Vector3(0, 0, 1), 3*factor);                     
            float diff =  f / (120f/factor);
                transform.localScale = new Vector3(1f - diff, 1 - diff, 1);                                       
            yield return null; ;
        }

    }


    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameState.Instance.IsRunning )
        {
            return;
        }
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Wall")
        {
            animator.SetTrigger("playerStop");
            lastDirection = 0;			 
        }

        else if (other.tag.StartsWith("Pill")) 
        {
            other.gameObject.SetActive(false); 
            GameState.Instance.EatPill( );
#if DEBUG 
            GameState.Instance.RemovePill(other.gameObject.tag);
#endif 
        }
        else if (other.tag == "Feature")
        {
            other.gameObject.SetActive(false);
            GameState.Instance.EatFeature( );               
        }
        else if (other.tag == "Enemy")
        {
            animator.SetTrigger("playerStop"); 
            GameState.Instance.Die();           
            StartCoroutine(FlipOnDie());           
        }
    }

    private float CalculateUserPreferredSpeed()
    {
      UserPreferredMaxSpeed = MinSpeed + (Preferences.Current.PlayerSpeed * (MaxSpeed - MinSpeed));
      return UserPreferredMaxSpeed;  
    }

    public void CharacterReset (  )
    {
        StopCoroutine(FlipOnDie());
        GameState.Instance.SetPlayerCoords(transform.position); 
        lastDirection = 0;

        currentAxis = Axis.Horizontal;
        continueWalkingOnLastKey = true;
        speed = CalculateUserPreferredSpeed();
        if (transform != null)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler (0, 0, 0);
        }
      
        
        if (animator != null)
        {
            animator.SetTrigger("playerStop");
        }
    }

}

public enum Axis
{
    Horizontal,
    Vertical,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}