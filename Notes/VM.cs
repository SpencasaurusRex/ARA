using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VM
{
    class Program
    {
        enum Heading
        { 
            East,
            North,
            West,
            South,
        }

        enum Instruction
        { 
            Forward   = 0x00,
            Back      = 0x01,
            Up        = 0x02,
            Down      = 0x03,
            TurnLeft  = 0x04,
            TurnRight = 0x05
        }

        struct State
        {
            public long x; // East  / west
            public long y; // Up    / down
            public long z; // North / south
            public Heading h;
        }

        static State state = new State
        {
            x = 0,
            y = 0,
            z = 0,
            h = Heading.East,
        };

        enum VMState
        { 
            Operating,
            StackOverflow = -1,
            StackUnderflow = -2,
        }

        class VM
        {
            const UInt16 MAX_STACK = UInt16.MaxValue;
            UInt16 stackIndex = 0;    
            int[] stack = new int[MAX_STACK];
            VMState state = VMState.Operating;

            public void Push(int val)
            {
                if (state != VMState.Operating)
                {
                    return;
                }
                if (stackIndex + 1 == MAX_STACK)
                {
                    state = VMState.StackOverflow;
                    return;
                }
                stack[stackIndex++] = val;
            }

            public int Pop()
            { 
                if (state != VMState.Operating)
                {
                    return (int)VMState.StackUnderflow;
                }
                if (stackIndex <= 0)
                {
                    state = VMState.StackUnderflow;
                }
                return stack[stackIndex--];
            }
        }

        static void Main(string[] args)
        {
            byte[] code = {0x00, 0x01, 0x02, 0x03, 0x04, 0x05};
            VM vm = new VM();
        }

        static void Interpret(byte[] bytecode)
        {
            for (long i = 0; i < bytecode.LongLength; i++)
            {
                switch ((Instruction)bytecode[i])
                { 
                    case Instruction.Forward:
                        state.x += dx();
                        state.z += dz();
                        break;
                    case Instruction.Back:
                        state.x -= dx();
                        state.z -= dz();
                        break;
                    case Instruction.Up:
                        state.y += 1;
                        break;
                    case Instruction.Down:
                        state.y -= 1;
                        break;
                    case Instruction.TurnLeft:
                        state.h = (Heading)(((int)state.h + 1) % 4);
                        break;
                    case Instruction.TurnRight:
                        state.h = (Heading)(((int)state.h - 1) % 4);
                        break;
                }
            }
        }

        static int dx()
        {
            return Math.Abs((int)state.h % 4 - 2) - 1;
        }

        static int dz()
        {
            return -Math.Abs((int)state.h % 4 - 1) + 1;
        }

        static int EuclideanMod(int a, int b)
        {
            return ((a % b) + b) % b;
        }
    }
}
