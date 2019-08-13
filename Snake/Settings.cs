using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public enum Direction
    {
        up,
        down,
        left,
        right,
        none
    }

    class Settings
    {
        public static int speed { set; get; }
        public static int score { set; get; }
        public static int width { set; get; }
        public static int height { set; get; }
        public static Direction direction { set; get; }
        public static int points { set; get; }
        public static bool gameOver { set; get; }
        
        public Settings()
        {
            gameOver = false;
            speed = 10;
            direction = Direction.down;
            score = 0;
            points = 10;
            width = 20;
            height = 20;
        }
    }
}
