using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day8_InstructionLoop
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<Instruction?>("Input.txt", GetInstruction);

            var inputs = inputProvider.Where(w => w != null).Cast<Instruction>().ToList();

            Part1(inputs);

            Part2(inputs);
        }

        private static void Part1(IEnumerable<Instruction> inputs)
        {
            var computer = new Computer(inputs);

            Console.WriteLine($"Part 1: Value before first loop: {computer.Run()}");
        }

        private static void Part2(IEnumerable<Instruction> inputs)
        {
            int noOfTries = inputs.Count();
            for (int i = 0; i < noOfTries; i++)
            {
                var instructions = inputs.ToList();
                var current = instructions[i];

                if (current.Type == Instruction.InstructionType.Accumulator) continue;

                instructions[i] = new Instruction(current, current.Type == Instruction.InstructionType.Jump ? Instruction.InstructionType.NoOp : Instruction.InstructionType.Jump);

                var computer = new Computer(instructions);

                (bool terminated, int accumulator) = computer.Run();

                if (terminated)
                {
                    Console.WriteLine($"Part 2: Value after terminates: {accumulator}");
                    break;
                }
            }
        }

        static bool GetInstruction(string? input, out Instruction? value)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                value = null;
                return false;
            }

            try
            {
                value = new Instruction(input);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        class Computer
        {
            private readonly IList<Instruction> instructions;
            private readonly HashSet<Instruction> executionHistory = new HashSet<Instruction>();

            public Computer(IEnumerable<Instruction> instructions)
            {
                this.instructions = instructions.ToList();
            }

            public (bool terminated, int accumulator) Run()
            {
                int accumulator = 0;
                int position = 0;

                while (!executionHistory.Contains(instructions[position]))
                {
                    var current = instructions[position];
                    if (current.Type == Instruction.InstructionType.Jump)
                    {
                        position += current.Value;
                    }
                    else
                    {
                        position++;

                        if (current.Type == Instruction.InstructionType.Accumulator)
                        {
                            accumulator += current.Value;
                        }
                    }

                    executionHistory.Add(current);

                    if (position == instructions.Count)
                        return (true, accumulator);
                }

                return (false, accumulator);
            }
        }

        class Instruction
        {
            private static readonly Regex numRegex = new Regex(@"-?\d+");

            public enum InstructionType { Accumulator, Jump, NoOp }
            public InstructionType Type { get; }

            public int Value { get; }

            public Instruction(string input)
            {
                if (input.Contains("nop"))
                {
                    this.Type = InstructionType.NoOp;
                    return;
                }
                else if (input.Contains("acc"))
                {
                    this.Type = InstructionType.Accumulator;
                }
                else if (input.Contains("jmp"))
                {
                    this.Type = InstructionType.Jump;
                }

                this.Value = int.Parse(numRegex.Match(input).Captures[0].Value);
            }

            public Instruction(Instruction instruction, InstructionType newType)
            {
                this.Type = newType;
                this.Value = instruction.Value;
            }
        }
    }
}
