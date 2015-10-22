#define USELINEARDELTA 

using UnityEngine;
using System.Collections;
using System;

namespace MonsterGame {

    enum TileType
    {
        PathWithDot = 0 ,
        PathWithoutDot = 1 ,
        BlueWall = 2,
        GrayWall =3 ,
        PlayerStart =4 ,
        PathWithIcon =5 ,
        EnemyStart=6 ,
    }


    public class DefaultMazeConfig : MazeConfig 
    {
        public DefaultMazeConfig( Vector3 world ) : base ( world,  13, 21, 0f, 0f )
        {
            this.height = DefaultMazeConfig.DefaultTileHeight;
            this.width = DefaultMazeConfig.DefaultTileWidth; 
        }

         
    
        public const int NumRows = 13;
        public const int NumColumns = 21;
        public const float DefaultTileHeight = .72f;  
        public const float DefaultTileWidth = .72f; 
        public const int BlueWallCount = 5;
        public const int GrayWallCount = 5;

       

    }

    public interface IMazeConfig
    {
        float TileWidth { get; }
        float TileHeight { get; }
        int NumRows { get; } 
        int NumColumns { get; }  

     
        //private const int BlueWallCount = 5;
        //private const int GrayWallCount = 5;
    }


    public class MazeConfig : IMazeConfig
    {
        protected int columns, rows;
        protected float width, height;
        protected Vector3 worldPosition; 
        //public MazeConfig () : this ( new Vector3(0f,0f,0f), DefaultMazeConfig.NumRows, DefaultMazeConfig.NumColumns, DefaultMazeConfig.DefaultTileWidth, DefaultMazeConfig.DefaultTileHeight)
        //{

        //}
        public MazeConfig ( Vector3 worldPosition,  int paramRows, int paramColumns, float paramWidth, float paramHeight )
        {
            this.worldPosition = worldPosition; 
            this.rows = paramRows;
            this.columns = paramColumns;
            this.width = paramWidth;
            this.height = paramHeight; 
        }
        public int NumColumns
        {
            get
            {
                return columns ; 
            }
        }

        public int NumRows
        {
            get
            {
                return rows; 
            }
        }

        public float TileHeight
        {
            get
            {
                return height; 
            }
        }

        public float  TileWidth
        {
            get
            {
                return width; 
            }
        }

        public float VerticalDirection
        {
            get { return -1f;  }
        }


        public Vector3 WorldPosition 
        {
            get { return worldPosition ;  }
            set { worldPosition = value;  }
        }
       

        public float Height { get { return this.NumRows * this.TileHeight; } } 
        public float Width { get { return this.NumColumns * this.TileWidth; } }

        public Vector2 GetRowColumn(Vector2 worldPosition )
        {            
            Vector2 retVal = WorldToTile(worldPosition);
            //   Debug.Log(string.Format("GRC: {0}, {1}", transformposition.position, retVal));
            return retVal;
        }

        public Vector2 GetRowColumn ( Transform transformposition )
        {
            
            Vector2 vector = transformposition.position;
            Vector2 retVal = WorldToTile(vector);
          //   Debug.Log(string.Format("GRC: {0}, {1}", transformposition.position, retVal));
            return retVal; 
        }

        public Vector2 TileToWorld(Vector2 p)
        {
            Vector2 vector;
#if USELINEARDELTA 
            vector.x = worldPosition.x + (p.x * TileWidth) /*- (TileWidth / 2)*/;
            vector.y = worldPosition.y + (p.y * TileHeight * VerticalDirection) /*- (TileHeight / 2)*/ ;
#else 
   vector.x = worldPosition.x + (p.x * TileWidth) ;
            vector.y = worldPosition.y + (p.y * TileHeight * VerticalDirection );
#endif 
            return vector;
        }

        public Vector2 TileToWorld (Point p)
        {
            Vector2 vector;
#if USELINEARDELTA 
            vector.x = worldPosition.x + (p.X * TileWidth) - (TileWidth/2) ;
            vector.y = worldPosition.y + (p.Y * TileHeight * VerticalDirection ) - (TileHeight / 2);
#else 
   vector.x = worldPosition.x + (p.X * TileWidth) ;
            vector.y = worldPosition.y + (p.Y * TileHeight * VerticalDirection );
#endif 
            return vector;
        }

        public Vector2 WorldToTile (Vector2  vector )
        {
#if USELINEARDELTA
            float roundingoffset = 0.0f;
#else
            float roundingoffset = 0.1f;
#endif
            vector.x = Mathf.Abs(worldPosition.x - (vector.x + roundingoffset));
            vector.y = Mathf.Abs(worldPosition.y - (vector.y - roundingoffset));
            Vector2 ret = new Vector2(Mathf.RoundToInt(vector.x / TileWidth),
                             // Use Abs for y because we don't know if board direction goes up or down ... 
                             Mathf.RoundToInt(Mathf.Abs(vector.y) / TileHeight));
            return ret ;
        }


        public bool IsValidEnemyPosition(Vector2 vector)
        {
            float threshold = TileWidth/4f;
            Vector2 position = WorldToTile(vector);
            Vector2 vectorCentered = new Vector2(position.x * TileWidth, position.y * TileHeight);
            // rounding for math multiplication issues 
            vectorCentered.x += .01f; vectorCentered.y += 0.01f; 
            bool ret = (((vectorCentered.x % TileWidth) < threshold) && ((vectorCentered.y % TileHeight) < threshold)
                       );
            if ( !ret )
                Debug.Assert(ret, string.Format("INVALID Center {0},{1}", vectorCentered.x, vectorCentered.y));

            if (ret)
            {
                threshold = 0.1f; 
                float thresholdX = Mathf.Abs ( worldPosition.x % TileWidth) + threshold;
                float thresholdY = Mathf.Abs ( worldPosition.y % TileHeight) + threshold ; 
                float x = Mathf.Abs((worldPosition.x + (vector.x) + TileWidth/2f ) % TileWidth);
                float y = Mathf.Abs((worldPosition.y + (vector.y) + TileHeight/2f) % TileHeight);
                ret = (x < thresholdX && y < thresholdY);    
                if ( !ret )         
                    Debug.Assert(ret, string.Format("INVALID OFFSET {0},{1}", x, y));
            }
            return ret; 
        }
         
        public Vector2 GetTileWithOffset( Vector2 tile, int horizontal, int vertical) 
        {
            Vector2 target;
            vertical *= (int) VerticalDirection; 
            target.x = tile.x + horizontal;
            target.y = tile.y + vertical;
            Debug.Assert(target.x >= 0 && target.x < NumColumns  &&
                target.y >= 0 && target.y < NumRows 
                );
            return target; 
             
        }
        public int[,] Data
        {
            get; set;
        }

        public bool IsObstacle(Vector2 tile)
        {
            Debug.Assert(Data != null);
            if (Data != null)
            {
                int tileflag = Data[(int)tile.y, (int)tile.x];
                return (tileflag == (int)TileType.BlueWall || tileflag == (int)TileType.GrayWall);
            }
            return true;
        }
    }


    //TODO: this is a quick hacky way to reuse the Monogames' path finding source 

#region PathFinder hacks 
    public class MazeTile
    {
        public enum MazeTileType { Path, Wall }
        public enum DotTileType { None, Dot, Icon }

        public MazeTileType TileType { get; set; }
        public DotTileType DotType { get; set; }
        //public Texture2D Tile { get; set; }
        //public Sprite Icon { get; set; }
    }

    public class Maze  
    {
        private MazeTile[,] _mazeTiles;
        MazeConfig config; 

        public Maze (  MazeConfig mazeConfiguration  )
        {
            config = mazeConfiguration;
            CreateMaze(Levels.AllLevels[GameState.Instance.CurrentLevel % Levels.AllLevels.Length].Data); 
        }

        private void CreateMaze(int[,] data)
        {
            //Random r = new Random();
            _mazeTiles = new MazeTile[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    MazeTile mt = new MazeTile();
                    TileType tt = (TileType)data[row, col];
                    switch (tt)
                    {
                        case TileType.PathWithDot:
                            mt.DotType = MazeTile.DotTileType.Dot;
                            //mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            break;
                        case TileType.PlayerStart:
                        case TileType.EnemyStart:
                        case TileType.PathWithoutDot:
                            mt.DotType = MazeTile.DotTileType.None;
                            //mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            //if (tt == TileType.PlayerStart)
                            //    PlayerStart = new Vector2(col, row);
                            //if (tt == TileType.EnemyStart)
                            //    EnemyStart = new Vector2(col, row);
                            break;
                        case TileType.BlueWall:
                            mt.DotType = MazeTile.DotTileType.None;
                            //mt.Tile = _blueWalls[r.Next(_blueWalls.Length)];
                            mt.TileType = MazeTile.MazeTileType.Wall;
                            break;
                        case TileType.GrayWall:
                            mt.DotType = MazeTile.DotTileType.None;
                            //mt.Tile = _grayWalls[r.Next(_grayWalls.Length)];
                            mt.TileType = MazeTile.MazeTileType.Wall;
                            break;
                        case TileType.PathWithIcon:
                            mt.DotType = MazeTile.DotTileType.Icon;
                            //mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            //mt.Icon = new Icon();
                            break;
                    }
                    _mazeTiles[row, col] = mt;
                }
            }
        }


        public MazeTile GetTile(int row, int col)
        {
            if (row < 0 || row >= _mazeTiles.GetLength(0))
                return null;
            if (col < 0 || col >= _mazeTiles.GetLength(1))
                return null;

            return _mazeTiles[row, col];
        }

        public Vector2 TileToWorld ( Point p )
        {
            return config.TileToWorld(p); 
        }

        public Point WordlToTile(Vector2 v)
        {
            Vector2 vTile =  config.WorldToTile(v);
            return new Point( (int) vTile.x, (int) vTile.y); 
        }

        

         

    }

#endregion

}



/* 
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
    public class Maze : Sprite
    {
        
        public static readonly Vector2 MazeOffset = new Vector2(
                (MonsterGame.ScreenWidth - (NumColumns * TileWidth)) / 2.0f,
                (MonsterGame.ScreenHeight - (NumRows * TileHeight)) / 2.0f);

        public Vector2 PlayerStart { get; private set; }
        public Vector2 EnemyStart { get; private set; }

        private readonly Texture2D[] _blueWalls = new Texture2D[BlueWallCount];
        private readonly Texture2D[] _grayWalls = new Texture2D[GrayWallCount];
        private readonly Texture2D _pathTile;
        private readonly Dot _dot = new Dot();

        private MazeTile[,] _mazeTiles;

        private static readonly Vector2 CenterTile = new Vector2(TileWidth / 2.0f, TileHeight / 2.0f);

        public Maze(Game game, int[,] data)
        {
            for (int i = 1; i <= BlueWallCount; i++)
                _blueWalls[i - 1] = game.Content.Load<Texture2D>("gfx\\maze\\WallBlue" + i);
            for (int i = 1; i <= GrayWallCount; i++)
                _grayWalls[i - 1] = game.Content.Load<Texture2D>("gfx\\maze\\WallGray" + i);

            _pathTile = game.Content.Load<Texture2D>("gfx\\maze\\Wallway");

            CreateMaze(data);
        }

        private void CreateMaze(int[,] data)
        {
            Random r = new Random();
            _mazeTiles = new MazeTile[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    MazeTile mt = new MazeTile();
                    TileType tt = (TileType)data[row, col];
                    switch (tt)
                    {
                        case TileType.PathWithDot:
                            mt.DotType = MazeTile.DotTileType.Dot;
                            mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            break;
                        case TileType.PlayerStart:
                        case TileType.EnemyStart:
                        case TileType.PathWithoutDot:
                            mt.DotType = MazeTile.DotTileType.None;
                            mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            if (tt == TileType.PlayerStart)
                                PlayerStart = new Vector2(col, row);
                            if (tt == TileType.EnemyStart)
                                EnemyStart = new Vector2(col, row);
                            break;
                        case TileType.BlueWall:
                            mt.DotType = MazeTile.DotTileType.None;
                            mt.Tile = _blueWalls[r.Next(_blueWalls.Length)];
                            mt.TileType = MazeTile.MazeTileType.Wall;
                            break;
                        case TileType.GrayWall:
                            mt.DotType = MazeTile.DotTileType.None;
                            mt.Tile = _grayWalls[r.Next(_grayWalls.Length)];
                            mt.TileType = MazeTile.MazeTileType.Wall;
                            break;
                        case TileType.PathWithIcon:
                            mt.DotType = MazeTile.DotTileType.Icon;
                            mt.Tile = _pathTile;
                            mt.TileType = MazeTile.MazeTileType.Path;
                            mt.Icon = new Icon();
                            break;
                    }
                    _mazeTiles[row, col] = mt;
                }
            }
        }

        

        public bool IsComplete
        {
            get
            {
                foreach (MazeTile mt in _mazeTiles)
                {
                    if (mt.DotType != MazeTile.DotTileType.None)
                        return false;
                }
                return true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int row = 0; row < _mazeTiles.GetLength(0); row++)
            {
                for (int col = 0; col < _mazeTiles.GetLength(1); col++)
                {
                    MazeTile mt = _mazeTiles[row, col];
                    if (mt.DotType == MazeTile.DotTileType.Icon)
                        mt.Icon.Update(gameTime);
                }
            }

            _dot.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int row = 0; row < _mazeTiles.GetLength(0); row++)
            {
                for (int col = 0; col < _mazeTiles.GetLength(1); col++)
                {
                    MazeTile mt = _mazeTiles[row, col];
                    Vector2 pos = new Vector2(col * TileWidth, row * TileHeight) + MazeOffset;

                    spriteBatch.Draw(_mazeTiles[row, col].Tile, pos, Color.White);

                    switch (mt.DotType)
                    {
                        case MazeTile.DotTileType.None:
                            break;
                        case MazeTile.DotTileType.Dot:
                            _dot.Position = pos + CenterTile;
                            _dot.Draw(gameTime, spriteBatch);
                            break;
                        case MazeTile.DotTileType.Icon:
                            mt.Icon.Position = pos + CenterTile;
                            mt.Icon.Draw(gameTime, spriteBatch);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}

  */
