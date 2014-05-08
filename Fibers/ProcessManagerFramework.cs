using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

using Fibers;

namespace ProcessManager
{
    public static class ProcessManager
    {
        private static List<uint> fibers = new List<uint>();
        private static List<uint> finished = new List<uint>();
        private static int fiberIndex = 0;

        public static void Switch(bool fiberFinished)
        {
            if (fiberFinished)
            {
                uint fiberId = fibers[fiberIndex];
                Console.WriteLine("Fiber [{0}] finished working", fiberId);
                if (fiberId != Fiber.PrimaryId)
                {
                    finished.Add(fiberId);
                    fibers.RemoveAt(fiberIndex);
                }
            }
            else
            {
                fiberIndex++;
            }
            if (fibers.Count > 0)
            {
                fiberIndex %= fibers.Count;
                uint fiberId = fibers[fiberIndex];
                Fiber.Switch(fiberId);
            }
            else
            {
                Console.WriteLine("All fibers finished working");
                Fiber.Switch(Fiber.PrimaryId);
                foreach (uint fiberId in finished)
                {
                    Fiber.Delete(fiberId);
                }
                Fiber.Delete(Fiber.PrimaryId);
            }
        }


        public static void Main()
        {
            for (int i = 0; i < 7; i++)
            {
                Process process = new Process();
                Fiber fiber = new Fiber(new Action(process.Run));
                fibers.Add(fiber.Id);
            }
            Switch(false);
        }
    }

    public class Process
    {
        private static readonly Random Rng = new Random();

        private const int LongPauseBoundary = 10000;

        private const int ShortPauseBoundary = 100;

        private const int WorkBoundary = 1000;

        private const int IntervalsAmountBoundary = 10;

        private readonly List<int> _workIntervals = new List<int>();
        private readonly List<int> _pauseIntervals = new List<int>();

        public Process()
        {
            int amount = Rng.Next(IntervalsAmountBoundary);

            for (int i = 0; i < amount; i++)
            {
                _workIntervals.Add(Rng.Next(WorkBoundary));
                _pauseIntervals.Add(Rng.Next(
                        Rng.NextDouble() > 0.9
                            ? LongPauseBoundary
                            : ShortPauseBoundary));
            }
        }

        public void Run()
        {
            for (int i = 0; i < _workIntervals.Count; i++)
            {
                Thread.Sleep(_workIntervals[i]); // work emulation
                DateTime pauseBeginTime = DateTime.Now;
                do
                {
                    ProcessManager.Switch(false);
                } while ((DateTime.Now - pauseBeginTime).TotalMilliseconds < _pauseIntervals[i]); // I/O emulation
            }
            ProcessManager.Switch(true);
        }

        public int TotalDuration
        {
            get
            {
                return ActiveDuration + _pauseIntervals.Sum();
            }
        }

        public int ActiveDuration
        {
            get
            {
                return _workIntervals.Sum();
            }
        }
    }
}
