﻿using System;

namespace Movement
{
    public class SoftEnemiesMovement : IMovement
    {
        private int _maxYPosition;
        private int _maxXPosition;
        private readonly Random _r;
        private int _moveValue = 2;

        private bool moveUpDown = true;
        private bool moveLeftRight = true;

        public SoftEnemiesMovement()
        {
            _r = new Random();
        }

        public void SetMaxYPosition(int maxPosition)
        {
            _maxYPosition = maxPosition;
        }

        public void SetMaxXPosition(int maxPosition)
        {
            _maxXPosition = maxPosition;
        }

        public int GetNewXPosition(int oldPosition)
        {
            if (moveLeftRight)
            {
                int value = _r.Next(oldPosition, oldPosition + _moveValue);
                if (value <= _maxXPosition)
                {
                    return _maxXPosition;
                }
            }
            
            return _r.Next(oldPosition - _moveValue, oldPosition);
            
        }
        
        public int GetNewYPosition(int oldPosition)
        {
            if (moveUpDown)
            {
                int value = _r.Next(oldPosition, oldPosition + _moveValue);
                if (value <= _maxYPosition)
                {
                    return _maxYPosition;
                }
            }
            
            return _r.Next(oldPosition - _moveValue, oldPosition);
        }
    }
}