﻿using Game2048.AI.TD_Learning;
using System;

namespace Game2048.Game.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            TD_LearningAgent agent = new TD_LearningAgent(0.0025f);
            agent.Training(10000, 1000, ConsoleGameEnvironment.PrintBoard);
            //Game.Library.Game game = new Library.Game();
            //ConsoleGameEnvironment.PrintBoard(game.Board);
            //while (!game.IsEnd)
            //{
            //    ulong movedRawBlocks = game.Move(ConsoleGameEnvironment.GetDirection());
            //    ulong blocksAfterAdded = game.Board.RawBlocks;
            //    ConsoleGameEnvironment.PrintBoard(game.Board);
            //}

            Console.Read();
        }
    }
}