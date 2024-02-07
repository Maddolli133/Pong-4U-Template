/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);

        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown, rightKeyDown, eKeyDown, leftKeyDown, qKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        const int BALL_SPEED = 4;
        int BALL_WIDTH = 20;
        int BALL_HEIGHT = 20;
        Rectangle ball;

        //player values
        const int PADDLE_SPEED = 4;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 40;
        Rectangle player1, player2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
                case Keys.Right:
                    rightKeyDown = true;
                    break;
                case Keys.E:
                    eKeyDown = true;
                    break;
                case Keys.Left:
                    leftKeyDown = true;
                    break;

            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
                case Keys.Right:
                    rightKeyDown = false;
                    break;
                case Keys.E:
                    eKeyDown = false;
                    break;
                case Keys.Left:
                    leftKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            ball = new Rectangle(this.Width / 2 - BALL_WIDTH / 2, this.Height / 2 - BALL_HEIGHT / 2, BALL_WIDTH, BALL_HEIGHT);
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // ball either left or right 
            if (ballMoveRight)
            {
                ball.X += BALL_SPEED;
            }
            else
            {
                ball.X -= BALL_SPEED;
            }
            // ball either down or up 
            if (ballMoveDown)
            {
                ball.Y += BALL_SPEED;
            }
            else
            {
                ball.Y -= BALL_SPEED;
            }
            
            /*if (leftKeyDown)
            {
                ball.Width = 50;
                ball.Height = 50;
            }
            if (eKeyDown)
            {
                ball.Width = 50;
                ball.Height = 50;
            }*/
            #endregion

            #region update paddle positions

            if (wKeyDown == true && player1.Y > 0)
            {
                player1.Y -= PADDLE_SPEED;
            }
            else if (sKeyDown == true && player1.Y < this.Height - PADDLE_HEIGHT)
            {
                player1.Y += PADDLE_SPEED;
            }


            if (upKeyDown == true && player2.Y > 0)
            {
                player2.Y -= PADDLE_SPEED;
            }
            else if (downKeyDown == true && player2.Y < this.Height - PADDLE_HEIGHT)
            {
                player2.Y += PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0)
            {
                ballMoveDown = true;
                collisionSound.Play();
            }
            else if (ball.Y > this.Height - BALL_HEIGHT)
            {
                ballMoveDown = false;
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles
            ball.Height = BALL_HEIGHT; 
            ball.Width = BALL_WIDTH;
            if (player1.IntersectsWith(ball) || player2.IntersectsWith(ball))
            {
                collisionSound.Play();
                ballMoveRight = !ballMoveRight;
                BALL_HEIGHT --;
                BALL_WIDTH --;
                if ( BALL_HEIGHT < 1|| BALL_WIDTH < 1)
                {
                    BALL_HEIGHT = 1;
                    BALL_WIDTH = 1;
                }
                    
            }
            

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                scoreSound.Play();
                player2Score++;
                plaery2ScoreLabel.Text = $"{player2Score}";
                BALL_HEIGHT = 20;
                BALL_WIDTH = 20;

                if (player2Score > gameWinScore)
                {
                    GameOver("Player2");
                }
                else
                {
                    ballMoveRight = true;
                    SetParameters();
                }
            }

            if (ball.X > this.Width - BALL_WIDTH)
            {
                scoreSound.Play();
                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";
               BALL_HEIGHT = 20;
               BALL_WIDTH = 20;

                if (player1Score > gameWinScore)
                {
                    GameOver("Player1");
                }
                else
                {
                    ballMoveRight = false;
                    SetParameters();
                }
            }


            #endregion

           


            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
     
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            gameUpdateLoop.Stop();

            startLabel.Visible = true;
            startLabel.Text = winner;
            startLabel.Text += " Wins!";

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, player1);
            e.Graphics.FillRectangle(whiteBrush, player2);
            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, ball);
            if (eKeyDown)
            {
                e.Graphics.FillRectangle(blackBrush, ball);
            }
            if (rightKeyDown)
            {
                e.Graphics.FillRectangle(blackBrush, ball);
            }
        }

    }
}
