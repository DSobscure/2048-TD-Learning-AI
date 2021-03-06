﻿using Game2048.Game.Library;
using System;

namespace Game2048.AI.TD_Learning
{
    public class TD_LearningAI : LearningAI
    {
        private TupleNetwork tupleNetwork;

        public TD_LearningAI(float learningRate, TupleNetwork tupleNetwork) : base(learningRate)
        {
            this.tupleNetwork = tupleNetwork;
        }

        protected override float Evaluate(BitBoard board, Direction direction)
        {
            int result;
            BitBoard boardAfter = board.Move(direction, out result);
            ulong rawBoard = boardAfter.RawBlocks;
            boardAfter.InsertNewTile();
            return (result + tupleNetwork.GetValue(BitBoard.GetSymmetricBoards(rawBoard)));
        }
        
        public void UpdateEvaluation()
        {
            BestMoveNode[] bestMoveNodes = new BestMoveNode[td_StateChain.Count];
            for (int i = 0; i < td_StateChain.Count; i++)
            {
                BitBoard board = new BitBoard(td_StateChain[i].insertedRawBlocks);
                Direction nextDirection = GetBestMove(board);
                int nextReward = 0;

                bestMoveNodes[i].bestMove = nextDirection;
                bestMoveNodes[i].rawBlocks = board.RawBlocks;
                bestMoveNodes[i].movedRawBlocks = board.MoveRaw(nextDirection, out nextReward);
                bestMoveNodes[i].reward = nextReward;
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                ulong[] symmetricBoards = BitBoard.GetSymmetricBoards(bestMoveNodes[i].movedRawBlocks);
                ulong[] symmetricBoards_td = BitBoard.GetSymmetricBoards(td_StateChain[i].movedRawBlocks);
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(symmetricBoards);
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(symmetricBoards_td, learningRate * (score - tupleNetwork.GetValue(symmetricBoards_td)));
            }
        }

        public override void Save()
        {
            tupleNetwork.Save();
        }

        public override Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            UpdateEvaluation();
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }

        public override Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage = 0, float previousDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            throw new NotImplementedException();
        }
    }
}
