﻿using Game2048.Game.Library;
using System;

namespace Game2048.AI.TD_Learning
{
    public abstract class TupleFeature
    {
        private float[] tuples;
        public virtual void UpdateScore(ulong rawBlocks, float delta)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                tuples[index] += delta;
                if (symmetricIndex != index)
                    tuples[symmetricIndex] += delta;
            }
        }
        public virtual float GetScore(ulong blocks)
        {
            float sum = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                sum += tuples[index];
                if (symmetricIndex != index)
                    sum += tuples[symmetricIndex];
            }
            return sum;
        }
        public abstract int GetIndex(ulong blocks);

        public ulong[] rotateBoards;

        protected TupleFeature()
        { }
        protected TupleFeature(int tupleNumber)
        {
            tuples = new float[(int)Math.Pow(16, tupleNumber)];
        }
        
        public void SetSymmetricBoards(ulong[] rotateSymmetry)
        {
            rotateBoards = rotateSymmetry;
        }
        public ulong GetMirrorSymmetricRawBlocks(ulong rawBlocks)
        {
            ushort[] reversedRowContents = new ushort[4];

            for (int i = 0; i < 4; i++)
            {
                reversedRowContents[i] = BitBoardOperationSet.ReverseRow(BitBoardOperationSet.GetRow(rawBlocks, i));
            }

            return BitBoardOperationSet.SetRows(reversedRowContents); ;
        }
    }
}