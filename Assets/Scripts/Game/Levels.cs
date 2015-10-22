using System.Collections.Generic;

namespace MonsterGame
{
    public static class Levels
    {
        public static ILevel[] AllLevels { get; private set; }

        static Levels()
        {
            AllLevels = new ILevel[] {    /* new LevelDebug(), new LevelDebug(), new LevelDebug(),   new LevelDebug(), new LevelDebug(), new LevelDebug(), */ 
                        new Level1(), new Level2(), new Level3(), new Level4(), new Level5(), new Level6() };
        }
    }
}
