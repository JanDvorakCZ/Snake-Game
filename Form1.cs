using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> player = new List<Circle>();

        private Circle food;
        private Circle bp;

        private keyState ks;
        private Random rnd;

        private bool paused;
        private bool threadRunning;

        private System.Threading.Thread GameThread { get; set; }

        public Form1()
        {

            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            ks = new keyState();
            rnd = new Random();
            new Settings();

            GameThread = new System.Threading.Thread(RunGame);
            threadRunning = false;
            startGame();
        }

        public void RunGame()
        {
            long last = GetNano();
            long lastOut = GetNano();
            double nsPerTick = 1000000.0 / 3.0;
            double delta = 0;
            int FPS = 0;
            int TPS = 0;

            while (!Settings.gameOver)
            {
                long now = GetNano();
                delta += (now - last) / nsPerTick;
                last = now;

                while (delta >= 1)
                {
                    if (!paused)
                    {
                        update();
                        TPS++;
                    }
                    delta--;

                }
                keyCheck();
                pbCanvas.Invalidate();
                FPS++;


                if ((GetNano() - lastOut) > 1000000.0)
                {
                    Console.WriteLine("FPS: " + FPS + " TPS: " + TPS);
                    FPS = 0;
                    TPS = 0;
                    lastOut = GetNano();
                }
            }
            DialogResult dialogResult = MessageBox.Show("Would you like to start new game? \n Your score was : " + Settings.score, "Game Over", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
               
                System.Environment.Exit(1);
            }
        }

        public void update()
        {

            //set parameters of next bodypart as the one before 
            for (int i = player.Count() - 1; i > 0; i--)
            {
                player[i].X = player[i - 1].X;
                player[i].Y = player[i - 1].Y;
            }


            //Player movement
            if (Settings.direction == Direction.down)
            {
                player[0].Y += 20;
            } else if (Settings.direction == Direction.up)
            {
                player[0].Y -= 20;
            }
            else if (Settings.direction == Direction.right)
            {
                player[0].X += 20;
            } else if (Settings.direction == Direction.left)
            {
                player[0].X -= 20;
            }

            //collisions with map border
            if (player[0].X < 0)
            {
                Settings.gameOver = true;
            }
            else if (player[0].Y < 0)
            {
                Settings.gameOver = true;
            }
            else if (player[0].X + Settings.width > pbCanvas.Width)
            {
                Settings.gameOver = true;
            }
            else if (player[0].Y + Settings.height > pbCanvas.Height)
            {
                Settings.gameOver = true;
            }

            //collision of food and player ( :) FaP :) )
            FaPCollision();

            //collision of head with tail
            HaTCollision();
        }

        private long GetNano() => Stopwatch.GetTimestamp();

        private void startGame()
        {
            MessageBox.Show(new Form() { TopMost = true }, "Press RETURN to start game.");

            paused = false;
            Circle head = new Circle(20, 20);
            player.Clear();
            player.Add(head);

            generateFood();
            if (!threadRunning)
            {
                GameThread.Start();
                threadRunning = true;
            }
        }

        private void generateFood()
        {
            food = new Circle((rnd.Next(1, (pbCanvas.Width - Settings.width) / 20)) * 20, (rnd.Next(1, (pbCanvas.Height - Settings.height) / 20)) * 20);
        }

        private void generateBodyPart()
        {
            if (Settings.direction == Direction.up)
            {
                bp = new Circle(player[player.Count - 1].X, player[player.Count - 1].Y + Settings.height);
                player.Add(bp);
            }
            else if (Settings.direction == Direction.down)
            {
                bp = new Circle(player[player.Count - 1].X, player[player.Count - 1].Y - Settings.height);
                player.Add(bp);
            }
            else if (Settings.direction == Direction.left)
            {
                bp = new Circle(player[player.Count - 1].X + Settings.width, player[player.Count - 1].Y);
                player.Add(bp);
            }
            else if (Settings.direction == Direction.right)
            {
                bp = new Circle(player[player.Count - 1].X - Settings.width, player[player.Count - 1].Y);
                player.Add(bp);
            }
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush;
            scoreLabel.Text = Settings.score.ToString();

            brush = Brushes.Black;
            g.Clear(Color.ForestGreen);

            int pattern = 1;
            for (int i = 0; i < player.Count(); i++)
            {
                g.FillEllipse(brush, player[i].X, player[i].Y, Settings.width, Settings.height);
                if (pattern == 1)
                {
                    brush = Brushes.DarkOrange;
                    pattern = 2;
                }
                else if (pattern == 2)
                {
                    brush = Brushes.OrangeRed;
                    pattern = 1;
           
                }
                
            }

            brush = Brushes.Wheat;
            g.FillEllipse(brush, food.X, food.Y, Settings.width, Settings.height);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            ks.changeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            ks.changeState(e.KeyCode, false);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.gameOver = true;
        }

        private void FaPCollision()
        {
            if (player[0].X == food.X && player[0].Y == food.Y)
            {
                generateFood();
                generateBodyPart();
                Settings.score = Settings.score + 10;
            }
        }

        //checks collision of head with tail
        private void HaTCollision()
        {
            if (player.Count > 4)
            {
                for (int i = 4; i <= player.Count - 1; i++)
                {
                    if (player[0].X == player[i].X && player[0].Y == player[i].Y)
                    {
                        Settings.gameOver = true;
                    }
                }
            }
        }

        private void keyCheck()
        {

            //ESC closing
            if (ks.keyPressed(Keys.Escape))
            {
                Settings.gameOver = true;
            }

            if (ks.keyPressed(Keys.P))
            {
                paused = !paused;
            }

            //Changes directions
            if (ks.keyPressed(Keys.Up) && Direction.down != Settings.direction)
            {
                Settings.direction = Direction.up;
            }
            else if (ks.keyPressed(Keys.Down) && Direction.up != Settings.direction)
            {
                Settings.direction = Direction.down;
            }
            else if (ks.keyPressed(Keys.Right) && Direction.left != Settings.direction)
            {
                Settings.direction = Direction.right;
            }
            else if (ks.keyPressed(Keys.Left) && Direction.right != Settings.direction)
            {
                Settings.direction = Direction.left;
            }
        }
    }
}
