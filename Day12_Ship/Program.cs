using SantasToolbox;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day12_Ship
{
    class Program
    {
        private static readonly Regex numRegex = new Regex(@"-?\d+");

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<Instruction?>("Input.txt", GetInstruction);
            var inputs = inputProvider.Where(w => w != null).Cast<Instruction>().ToList();

            // Part 1
            var ship = new Agent(0, 0, Direction.East);

            foreach (var instruction in inputs)
            {
                ship.Move(instruction);
            }

            Console.WriteLine($"Part 1: X: {ship.X} Y: {ship.Y} Manhattan: {Math.Abs(ship.X) + Math.Abs(ship.Y)} Heading: {ship.Heading}");

            // Part 2
            ship = new Agent(0, 0, Direction.East);
            var waypoint = new Agent(-10, -1, Direction.East) { RotateAroundPoint = true };

            foreach (var instruction in inputs)
            {
                if (instruction.Dir != Direction.Forward)
                {
                    waypoint.Move(instruction);
                }
                else
                {
                    ship.Move(waypoint, instruction.Step);
                }
            }

            Console.WriteLine($"Part 2: X: {ship.X} Y: {ship.Y} Manhattan: {Math.Abs(ship.X) + Math.Abs(ship.Y)} Heading: {ship.Heading}");
        }

        static bool GetInstruction(string? input, out Instruction? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            try
            {
                int steps = int.Parse(numRegex.Match(input).Captures[0].Value);
                Direction direction = input[0] switch
                {
                    'N' => Direction.North,
                    'S' => Direction.South,
                    'E' => Direction.East,
                    'W' => Direction.West,
                    'L' => Direction.Left,
                    'R' => Direction.Right,
                    'F' => Direction.Forward,
                    _ => throw new Exception()
                };

                value = new Instruction(direction, steps);
                return true;
            }
            catch
            {
                return false;
            }
        }

        enum Direction { North = 1, East = 2, West = 3, South = 4, Left = 5, Right = 6, Forward = 7 }

        class Agent
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public Direction Heading { get; private set; }

            public bool RotateAroundPoint { get; set; } = false;

            public Agent(int x, int y, Direction initialDirection)
            {
                this.X = x;
                this.Y = y;
                this.Heading = initialDirection;
            }

            public void Move(Instruction instruction)
            {
                Direction direction = instruction.Dir;

                if (direction == Direction.Forward)
                {
                    direction = this.Heading;
                }

                if ((int)direction < 5)
                {
                    if (direction == Direction.North)
                    {
                        this.Y -= instruction.Step;
                    }
                    else if (direction == Direction.South)
                    {
                        this.Y += instruction.Step;
                    }
                    else if (direction == Direction.East)
                    {
                        this.X -= instruction.Step;
                    }
                    else if (direction == Direction.West)
                    {
                        this.X += instruction.Step;
                    }
                    else throw new Exception();
                }
                else
                {
                    int steps = instruction.Step / 90;

                    for (int i = 0; i < steps; i++)
                    {
                        if (instruction.Dir == Direction.Left)
                        {
                            this.Heading = this.Heading switch
                            {
                                Direction.North => Direction.West,
                                Direction.South => Direction.East,
                                Direction.East => Direction.North,
                                Direction.West => Direction.South,
                                _ => throw new Exception()
                            };

                            if (this.RotateAroundPoint)
                            {
                                (this.X, this.Y) = RotateCounterClockwise(this.X, this.Y, 90);
                            }
                        }
                        else if (instruction.Dir == Direction.Right)
                        {
                            this.Heading = this.Heading switch
                            {
                                Direction.North => Direction.East,
                                Direction.South => Direction.West,
                                Direction.East => Direction.South,
                                Direction.West => Direction.North,
                                _ => throw new Exception()
                            };

                            if (this.RotateAroundPoint)
                            {
                                (this.X, this.Y) = RotateCounterClockwise(this.X, this.Y, -90);
                            }
                        }
                    }
                }

                (int x, int y) RotateCounterClockwise(int x, int y, double angle)
                {
                    angle = angle * Math.PI / 180;

                    return
                    (
                    (int)Math.Round(x * Math.Cos(angle) - y * Math.Sin(angle)),
                    (int)Math.Round(y * Math.Cos(angle) + x * Math.Sin(angle))
                    );
                }
            }

            public void Move(Agent waypoint, int steps)
            {
                this.X += waypoint.X * steps;
                this.Y += waypoint.Y * steps;
            }
        }

        class Instruction
        {
            public Direction Dir { get; }
            public int Step { get; }

            public Instruction(Direction direction, int steps)
            {
                this.Dir = direction;
                this.Step = steps;
            }
        }
    }
}
