using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public enum MovementAction
    {
        GoUp,
        GoDown,
        TurnLeft,
        TurnRight,
        GoForward,
        GoBack,
        Idle,
        Up,
    }

    public enum Direction
    {
        Up,
        Down,
        East,
        North,
        West,
        South
    }

    public enum Heading
    {
        East,
        North,
        West,
        South
    }
}
