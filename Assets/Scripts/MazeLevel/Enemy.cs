//TODO: COPYRIGHT 

//#define USETIMEDLERP 
//#define USEOFFSETLERP 
#define USELINEARDELTA 

using UnityEngine;
using System.Collections.Generic;
using MonsterGame;

public class Enemy : MovingCharacter
{
    PathFinder _pathFinder;
    Vector2 enemyPosition;
    Vector2 nextPosition;  
    public float speedFactor = 8f;
    private float speed; 
    float moveTileTime ;
    float lastMoveTime;
    
	Animator animator;
    float tileSizeX;
    float tileSizeY; 
    new void Awake ()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

	// Use this for initialization
	new void Start ()
	{
        base.Start();                         		
	}

    private float CalculateUserPreferredSpeed()
    {
        float  deltaTime = 1/60f; 
        //multipy it by a factor because we want it to move a little slower than player and 
        // distance is calculated by TileHeght/speed 
        return Preferences.Current.PlayerSpeed * speedFactor * deltaTime  ;
    }

    void LevelReset()
    {
        _pathFinder = new PathFinder();
        _pathFinder.Initialize(new Maze(GameState.Instance.MazeConfiguration));

        tileSizeX = GameState.Instance.MazeConfiguration.TileWidth;
        tileSizeY = GameState.Instance.MazeConfiguration.TileHeight;
        
        CharacterReset(); 
    }

    void WinAchieved()
    {
        animator.SetTrigger("enemyStop"); 
    }

    void CharacterReset ()
    {
        speed = CalculateUserPreferredSpeed();
        moveTileTime = GameState.Instance.MazeConfiguration.TileHeight / speed;
        linearDelta = speed * GameState.Instance.MazeConfiguration.TileHeight;
        deltaOffset =  speed ;
 
        if (transform != null)
        {
            if ( animator != null )
			    animator.SetTrigger("enemyStop");

            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            enemyPosition = transform.position;
#if DEBUG
            Debug.Assert ( GameState.Instance.MazeConfiguration.IsValidEnemyPosition(enemyPosition));
#endif 
            enemyTile = _pathFinder.WorldToTile(enemyPosition); 
            nextPosition = enemyPosition;
            //this resets offset so that we inmmediately try to find a path.. 
            currentLerpOffset = nextPositionThreshold; 
        }
    }
     
	// Update is called once per frame
	void Update()
	{
        if (!GameState.Instance.IsRunning)
            return; 

        enemyPosition = transform.position;

 
        if (currentLerpOffset >= nextPositionThreshold)
         {
            _pathFinder.FindPath(enemyPosition, GameState.Instance.PlayerPosition);
            if (_pathFinder.SearchStatus == SearchStatus.PathFound)
            {
                LinkedList<Point> path = _pathFinder.FinalPath();
                if (path.Count == 1)
                    return;

                path.RemoveFirst();

                Point enemyTile = _pathFinder.WorldToTile(enemyPosition);
                Point nextTile = path.First.Value;

                Debug.Assert((  Mathf.Abs(enemyTile.X - nextTile.X) +
                                Mathf.Abs(enemyTile.Y - nextTile.Y) ) <= 1, "Tile has unexpected delta"); 

                if (nextTile.X > (enemyTile.X ))
                    StartMove(Direction.Right);
                else if (nextTile.X < (enemyTile.X ))
                    StartMove(Direction.Left);
                else if (nextTile.Y > (enemyTile.Y ))
                    StartMove(Direction.Down);
                else if (nextTile.Y < (enemyTile.Y ))
                    StartMove(Direction.Up);
                else
                    Debug.Assert(false, "NO MOVE. OFFSET MUST BE WRONG");

            }
#if DEBUG
            else
                Debug.Assert(false, "NO PATH FOUND!!"); 
#endif
        }
		else
        {
            ContinueMove(nextPosition);  
        }
    }

    void ContinueMove ( Vector3 nextPosition )
    {
    //    Debug.Log(string.Format("{0}: {1}", nextPosition.ToString(), currentLerpOffset));  


        currentLerpOffset += deltaOffset;
        transform.position = new Vector3(transform.position.x + nextDirectionOffsetX,
                                          transform.position.y + nextDirectionOffsetY, 
                                          transform.position.z);
 
    }

 
   
    float nextDirectionOffsetX;
    float nextDirectionOffsetY;  
 
    void StartMove (Direction next)
    {
		// only set the trigger if we're currently idle
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("enemy_idle_"))
			animator.SetTrigger("enemyMove");

        nextPosition = enemyPosition; 

        switch (next)
        {
            case Direction.Up:
                nextPosition.y += tileSizeY;
                nextDirectionOffsetX = 0f;
                nextDirectionOffsetY = linearDelta ; 
                break;
            case Direction.Down:
                nextPosition.y -= tileSizeY;
                nextDirectionOffsetX = 0f;
                nextDirectionOffsetY = -linearDelta ; 
                break;
            case Direction.Right:
                nextPosition.x += tileSizeX;
                nextDirectionOffsetX = linearDelta ;
                nextDirectionOffsetY = 0f;
                break;
            case Direction.Left:                
                nextPosition.x += -tileSizeY;
                nextDirectionOffsetX = -linearDelta;
                nextDirectionOffsetY = 0f;
                break;
            
        }

        currentLerpOffset = deltaOffset;
 
        Rotate(next);  

        ContinueMove(nextPosition); 
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
			animator.SetTrigger("enemyStop");
	}

    float deltaOffset = 0.10f;
    float currentLerpOffset = nextPositionThreshold;
    const float nextPositionThreshold = 1.01f; 
    float linearDelta = 0.072f;


    Point enemyTile; 
}

