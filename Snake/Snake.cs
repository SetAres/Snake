using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    enum BodyPart
    {
        Head,
        Body,
        Tail,
        Food
    }
    public partial class Snake : Form
    {
        private int cellSide = 40;
        private int windowWidth = 800;
        private int windowHeight = 840;
        private LinkedList<SnakeBodyPart> snake = new LinkedList<SnakeBodyPart>();
        private Keys moveDirection;
        private Keys SnakeDirection;
        private PictureBox food;

        public Snake()
        {
            InitializeComponent();
            this.Width = windowWidth + 150;
            this.Height = windowHeight;
            this.timer.Enabled = true;

            moveDirection = Keys.Up;
            GenerateSnake();
            GenerateFood();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            #region Generate grid lines
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));

            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(800, 0);

            for (int i = 0; i < 20; i++)
            {
                startPoint.Y += cellSide;
                endPoint.Y += cellSide;

                e.Graphics.DrawLine(pen, startPoint, endPoint);
            }


            startPoint = new Point(0, 0);
            endPoint = new Point(0, 800);

            for (int i = 0; i < 20; i++)
            {
                startPoint.X += cellSide;
                endPoint.X += cellSide;

                e.Graphics.DrawLine(pen, startPoint, endPoint);
            }
            #endregion
        }

        private SnakeBodyPart GenerateSnakePart(BodyPart partName, Color color, Point point)
        {
            SnakeBodyPart part = new SnakeBodyPart()
            {
                Width = cellSide,
                Height = cellSide,
                Location = point,
                BackColor = color,
                PartName = partName
            };

            part.SizeMode = PictureBoxSizeMode.StretchImage;
            panel1.Controls.Add(part);
            return part;
        }

        private void GenerateSnake()
        {
            SnakeBodyPart head = GenerateSnakePart(BodyPart.Head, Color.DarkGreen, new Point(400, 400));
            head.Image = Properties.Resources.HeadUp;
            SnakeBodyPart tail = GenerateSnakePart(BodyPart.Tail, Color.MediumSeaGreen, new Point(400, 440));
            tail.Image = Properties.Resources.Tail;

            panel1.Controls.Add(head);
            panel1.Controls.Add(tail);

            snake.AddFirst(head);
            snake.AddLast(tail);

            this.Text = $"Snake     *** score {snake.Count} ***";
        }

        private void GenerateFood()
        {
            Random random = new Random();

            var value = random.Next(0, 760);
            int x = value - value % 40;
            value = random.Next(0, 760);
            int y = value - value % 40;

            if (food == null)
                food = GenerateSnakePart(BodyPart.Food, Color.OrangeRed, new Point(x, y));
            else
                food.Location = new Point(x, y);

            panel1.Controls.Add(food);
        }

        /// <summary>
        /// General method
        /// </summary>
        private void SnakeMove()
        {
            Point lastLocation = snake.First().Location;

            foreach (var bodyPart in snake.ToList())
            {
                switch (bodyPart.PartName)
                {
                    case BodyPart.Head:
                        {
                            switch (moveDirection)
                            {
                                case Keys.Up:
                                    bodyPart.Location = new Point(bodyPart.Location.X, bodyPart.Location.Y - cellSide);
                                    bodyPart.Image = Properties.Resources.HeadUp;
                                    break;
                                case Keys.Down:
                                    bodyPart.Location = new Point(bodyPart.Location.X, bodyPart.Location.Y + cellSide);
                                    bodyPart.Image = Properties.Resources.HeadDown;
                                    break;
                                case Keys.Left:
                                    bodyPart.Location = new Point(bodyPart.Location.X - cellSide, bodyPart.Location.Y);
                                    bodyPart.Image = Properties.Resources.HeadLeft;
                                    break;
                                case Keys.Right:
                                    bodyPart.Location = new Point(bodyPart.Location.X + cellSide, bodyPart.Location.Y);
                                    bodyPart.Image = Properties.Resources.HeadRight;
                                    break;
                            }

                            SnakeDirection = moveDirection;

                            if (bodyPart.Location == food.Location)
                            {
                                var head = snake.Find(snake.First());
                                var newBody = GenerateSnakePart(BodyPart.Body, Color.MediumSeaGreen, lastLocation);
                                newBody.Image = Properties.Resources.HeadBody;
                                snake.AddAfter(head, newBody);
                                this.Text = $"Snake     *** score {snake.Count} ***";
                                GenerateFood();
                                return;
                            }
                        }
                        break;
                    case BodyPart.Body:
                        {
                            var currentLocation = bodyPart.Location;

                            bodyPart.Location = lastLocation;
                            lastLocation = currentLocation;
                        }
                        break;
                    case BodyPart.Tail:
                        {
                            bodyPart.Image = Properties.Resources.Tail;

                            var currentLocation = bodyPart.Location;

                            bodyPart.Location = lastLocation;
                            lastLocation = currentLocation;
                        }
                        break;
                }
            }
            CheckGame();
        }

        private void CheckGame()
        {
            var headLocation = snake.First().Location;
            bool bodyAcross = snake.Where(part => (part.PartName == BodyPart.Body || part.PartName == BodyPart.Tail) && part.Location.X == headLocation.X && part.Location.Y == headLocation.Y).ToList().Count > 0 ? true : false;

            if (bodyAcross || headLocation.X == -cellSide || headLocation.X == windowWidth || headLocation.Y == -cellSide || headLocation.Y == windowHeight - cellSide)
            {
                timer.Stop();
                Thread.Sleep(1000);

                foreach (var item in snake.ToList())
                    item.Dispose();

                snake.Clear();
                GenerateSnake();
                GenerateFood();
                moveDirection = Keys.Up;

                timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SnakeMove();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if (SnakeDirection == Keys.Up && e.KeyCode == Keys.Down)
                    return;
                if (SnakeDirection == Keys.Down && e.KeyCode == Keys.Up)
                    return;
                if (SnakeDirection == Keys.Left && e.KeyCode == Keys.Right)
                    return;
                if (SnakeDirection == Keys.Right && e.KeyCode == Keys.Left)
                    return;

                moveDirection = e.KeyCode;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer.Interval = (int)(1000 / numericUpDown1.Value);
        }
    }

    class SnakeBodyPart : PictureBox
    {
        public BodyPart PartName { get; set; }
    }
}
