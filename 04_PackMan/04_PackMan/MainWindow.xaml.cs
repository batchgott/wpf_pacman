﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _04_PackMan
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public enum Direction { up , down, left, right, none };
    public partial class MainWindow : Window
    {
        //Timer
        private DispatcherTimer enemyTimer, pacmanTimer;

        //Controller
        private CollisionController collisionController;
        private BorderController borderController;
        private PacmanBorderController pacBorderController;

        //Lists of Entitys
        Dictionary<Ellipse, Direction> enemyList;
        List<Shape> borders = new List<Shape>();
        
        //Attributes
        private Direction currentDirection,newDircetion;
        private int counter;
        private int generateCounter;
        private const double speed =1;

        public object Keys { get; private set; }


        public MainWindow()
        {
            InitializeComponent();
            enemyList = new Dictionary<Ellipse, Direction>();
            enemyTimer = new DispatcherTimer();
            pacmanTimer = new DispatcherTimer();
            generateCounter = 0;
            EventManager.RegisterClassHandler(typeof(MainWindow), UIElement.KeyDownEvent, new KeyEventHandler(KeyDownHandler));

            counter = 0;



            ReadAllBoxes();
            borderController = new BorderController(borders);
            GenerateEnemie();

            enemyTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            enemyTimer.Tick += new EventHandler(MoveEnemies);
            pacmanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            enemyTimer.Start();
            pacmanTimer.Start();
            pacmanTimer.Tick += new EventHandler(Move);
            currentDirection = Direction.none;
            newDircetion = Direction.none;
        }
        private void MoveLeft()
        {
                double left = Canvas.GetLeft(Pacman);
                left -= speed;
                Canvas.SetLeft(Pacman, left);
                currentDirection = Direction.left;
        }
        private void MoveRight()
        {
                double left = Canvas.GetLeft(Pacman);
                left += speed;
                Canvas.SetLeft(Pacman, left);
                currentDirection = Direction.right;
        }
        private void MoveUp()
        {
                double top = Canvas.GetTop(Pacman);
                top -= speed;
                Canvas.SetTop(Pacman, top);
                currentDirection = Direction.up;
        }
        private void MoveDown()
        {
                double top = Canvas.GetTop(Pacman);
                top += speed;
                Canvas.SetTop(Pacman, top);
                currentDirection = Direction.down;
        }

        private void Move(object sender, EventArgs e) {
            collisionController = new CollisionController(enemyList);
            if (collisionController.HasCrashed(Pacman))
                MessageBox.Show("Crashed");
            if (currentDirection != Direction.none || newDircetion!=currentDirection)
            {
                pacBorderController = new PacmanBorderController(borders, currentDirection,newDircetion);
                switch (pacBorderController.GetValidDirections(Pacman))
                {
                    case Direction.left:MoveLeft();currentDirection = Direction.left;  break;
                    case Direction.right: MoveRight();currentDirection = Direction.right; break;
                    case Direction.up: MoveUp();currentDirection = Direction.up; break;
                    case Direction.down: MoveDown();currentDirection = Direction.down; break;
                    case Direction.none:  break;

                }
            }
        }

        void KeyDownHandler(object sender, KeyEventArgs e)
        {
            UIElement element = e.OriginalSource as UIElement;
            if (Pacman != null)
            {
                    switch (e.Key)
                    {
                        case Key.Left: newDircetion = Direction.left; break;
                        case Key.Right: newDircetion = Direction.right; break;
                        case Key.Up: newDircetion = Direction.up; break;
                        case Key.Down: newDircetion = Direction.down; break;
                    }
            }
        }
        private bool CheckIf25() {
            if (Canvas.GetLeft(Pacman) % 25 == 0 && Canvas.GetTop(Pacman) % 25 == 0)
                return true;
            return false;
        }

        public void GenerateEnemie()
        {
            enemyTimer.Stop();
            if (!enemyTimer.IsEnabled)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Name = "Enemie" + enemyList.Count;
                ellipse.Fill = Brushes.Red;
                ellipse.Height = 25;
                ellipse.Width = 25;
                Canvas.SetLeft(ellipse, 225);
                Canvas.SetTop(ellipse, 100);
                boxes.Children.Add(ellipse);

                enemyList.Add(ellipse, Direction.down);
            }
            enemyTimer.Start();
        }

        public void MoveEnemies(object sender, EventArgs e)
        {

            bool checkValidDirection = false;
            if (generateCounter == 400)
            {
                GenerateEnemie();
                generateCounter = 0;
            }
            generateCounter++;
            if (counter == 25)
            {
                counter = 0;
                checkValidDirection = true;
            }
            counter++;

            Dictionary<Ellipse, Direction> currentEnemies = new Dictionary<Ellipse, Direction>();
            List<Direction> validDirections = new List<Direction>();
            Random r = new Random();

            if (checkValidDirection)
            {
                foreach (KeyValuePair<Ellipse, Direction> enemie in enemyList)
                {
                    validDirections.Clear();
                    validDirections = borderController.GetValidDirections(enemie.Key);
                    if (validDirections.Count > 1)
                    {
                        if (enemie.Value == Direction.down && validDirections.Contains(Direction.up))
                        {
                            validDirections.Remove(Direction.up);
                        }
                        else if (enemie.Value == Direction.left && validDirections.Contains(Direction.right))
                        {
                            validDirections.Remove(Direction.right);
                        }
                        else if (enemie.Value == Direction.up && validDirections.Contains(Direction.down))
                        {
                            validDirections.Remove(Direction.down);
                        }
                        else if (enemie.Value == Direction.right && validDirections.Contains(Direction.left))
                        {
                            validDirections.Remove(Direction.left);
                        }
                    }
                   
                   
                    int choose = r.Next(0, validDirections.Count);
                    KeyValuePair<Ellipse, Direction> editEnemie = new KeyValuePair<Ellipse, Direction>(enemie.Key, validDirections[choose]);

                    

                    currentEnemies.Add(editEnemie.Key, editEnemie.Value);

                }

                enemyList.Clear();

                foreach (KeyValuePair<Ellipse, Direction> enemie in currentEnemies)
                {
                    enemyList.Add(enemie.Key, enemie.Value);
                }

                currentEnemies.Clear();
            }

            foreach (KeyValuePair<Ellipse, Direction> enemie in enemyList)
            { 
                DoEnemieMove(enemie);
            }

            



        }

        public void DoEnemieMove(KeyValuePair<Ellipse, Direction> enemie)
        {
            switch (enemie.Value)
            {
                case Direction.down: MoveDown(enemie.Key); break;
                case Direction.up: MoveUp(enemie.Key); break;
                case Direction.right: MoveRight(enemie.Key); break;
                case Direction.left: MoveLeft(enemie.Key); break;
            }
        }

        public void MoveUp(Ellipse e)
        {
            Canvas.SetTop(e, Canvas.GetTop(e) - 1);
        }
        public void MoveDown(Ellipse e)
        {
            Canvas.SetTop(e, Canvas.GetTop(e) + 1);
        }
        public void MoveLeft(Ellipse e)
        {
            Canvas.SetLeft(e, Canvas.GetLeft(e) - 1);
        }

        public void MoveRight(Ellipse e)
        {
            Canvas.SetLeft(e, Canvas.GetLeft(e) + 1);
        }

        public void ReadAllBoxes()
        {
            borders.Clear();
            foreach (Shape item in boxes.Children)
            {

                if (item is Ellipse)
                {

                }
                else if (item is Rectangle)
                {
                    borders.Add(item);
                }

            }
        }

    }

}
