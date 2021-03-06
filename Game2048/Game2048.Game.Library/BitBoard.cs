﻿using Game2048.Game.Library.TableSets;
using System;

namespace Game2048.Game.Library
{
    public unsafe class BitBoard
    {
        private static Random newTileRandomGenerator = new Random((int)DateTime.Now.ToBinary());
        public static int RawEmptyCountTest(ulong rawBlocks)
        {
            int result = 0;
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                if (((rawBlocks >> shiftBitCount) & 0xf) == 0)
                {
                    result++;
                }
            }
            return result;
        }
        public static int MaxTileTest(ulong rawBlocks)
        {
            int maxTile = 1;
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                int tileNumber = (int)((rawBlocks >> shiftBitCount) & 0xf);
                if (BitBoardScoreTableSet.tileScoreTable[tileNumber] > maxTile)
                {
                    maxTile = BitBoardScoreTableSet.tileScoreTable[tileNumber];
                }
            }
            return maxTile;
        }
        public static int RawMaxTileTest(ulong rawBlocks)
        {
            int maxTile = 1;
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                int tileNumber = (int)((rawBlocks >> shiftBitCount) & 0xf);
                if (tileNumber > maxTile)
                {
                    maxTile = tileNumber;
                }
            }
            return maxTile;
        }
        public static ulong InversBlocks(ulong rawBlocks)
        {
            int maxTile = RawMaxTileTest(rawBlocks);
            ulong result = 0;
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                int tileNumber = (int)((rawBlocks >> shiftBitCount) & 0xf);
                int inverseNumber = (tileNumber != 0) ? maxTile - tileNumber : 15;
                result |= ((ulong)inverseNumber) << shiftBitCount;
            }
            return result;
        }
        public unsafe static ulong[] GetSymmetricBoards(ulong rawBlocks)
        {
            ulong[] symmetricBoards = new ulong[8];

            symmetricBoards[0] = rawBlocks;
            symmetricBoards[1] = BitBoardOperationSet.Transpose(symmetricBoards[0]);

            symmetricBoards[2] = BitBoardOperationSet.Flip(symmetricBoards[1]);
            symmetricBoards[3] = BitBoardOperationSet.Transpose(symmetricBoards[2]);

            symmetricBoards[4] = BitBoardOperationSet.Flip(symmetricBoards[3]);
            symmetricBoards[5] = BitBoardOperationSet.Transpose(symmetricBoards[4]);

            symmetricBoards[6] = BitBoardOperationSet.Flip(symmetricBoards[5]);
            symmetricBoards[7] = BitBoardOperationSet.Transpose(symmetricBoards[6]);
            return symmetricBoards;
        }

        private ulong rawBlocks;
        public ulong RawBlocks { get { return rawBlocks; } }
        public int EmptyCount
        {
            get
            {
                int result = 0;
                for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
                {
                    if (((rawBlocks >> shiftBitCount) & 0xf) == 0)
                    {
                        result++;
                    }
                }
                return result;
            }
        }
        public bool CanMove
        {
            get
            {
                for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
                {
                    if (MoveCheck(direction))
                        return true;
                }
                return false;
            }
        }
        public bool IsFull
        {
            get
            {
                for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
                {
                    if (((rawBlocks >> shiftBitCount) & 0xf) == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public int MaxTile
        {
            get
            {
                int maxTile = 0;
                for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
                {
                    int tileNumber = (int)((rawBlocks >> shiftBitCount) & 0xf);
                    if (BitBoardScoreTableSet.tileScoreTable[tileNumber] > maxTile)
                    {
                        maxTile = BitBoardScoreTableSet.tileScoreTable[tileNumber];
                    }
                }
                return maxTile;
            }
        }
        public int MaxTileRaw
        {
            get
            {
                int maxTile = 0;
                for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
                {
                    int tileNumber = (int)((rawBlocks >> shiftBitCount) & 0xf);
                    if (tileNumber > maxTile)
                    {
                        maxTile = tileNumber;
                    }
                }
                return maxTile;
            }
        }

        public BitBoard(ulong rawBlocks)
        {
            this.rawBlocks = rawBlocks;
        }
        public void InsertNewTile()
        {
            int emptyCountCache = EmptyCount;
            if (emptyCountCache > 0)
            {
                ulong tile = (newTileRandomGenerator.Next(0, 10) == 0) ? 2UL : 1UL;

                int targetIndex = newTileRandomGenerator.Next(0, emptyCountCache);
                for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
                {
                    if (((rawBlocks >> shiftBitCount) & 0xf) == 0)
                    {
                        if (targetIndex == 0)
                        {
                            rawBlocks |= tile << shiftBitCount;
                            break;
                        }
                        else
                        {
                            targetIndex--;
                        }
                    }
                }
            }
        }
        public void Initial()
        {
            InsertNewTile();
            InsertNewTile();
        }
        public BitBoard Move(Direction direction, out int reward)
        {
            ulong rawBlocksAfterMove, transposedRawBlocks;
            reward = 0;
            rawBlocksAfterMove = rawBlocks;
            switch (direction)
            {
                case Direction.Up:
                    transposedRawBlocks = BitBoardOperationSet.Transpose(rawBlocks);
                    ColumnShiftInfo[] shiftUpInfos = new ColumnShiftInfo[]
                    {
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 48) & 0xFFFF]
                    };
                    for(int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= shiftUpInfos[i].column << (4 * i);
                        reward += shiftUpInfos[i].reward;
                    }
                    return new BitBoard(rawBlocksAfterMove);
                case Direction.Down:
                    transposedRawBlocks = BitBoardOperationSet.Transpose(rawBlocks);
                    ColumnShiftInfo[] shiftDownInfos = new ColumnShiftInfo[]
                    {
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= shiftDownInfos[i].column << (4 * i);
                        reward += shiftDownInfos[i].reward;
                    }
                    return new BitBoard(rawBlocksAfterMove);
                case Direction.Left:
                    RowShiftInfo[] shiftLeftInfos = new RowShiftInfo[] 
                    {
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= ((ulong)shiftLeftInfos[i].row) << (16 * i);
                        reward += shiftLeftInfos[i].reward;
                    }
                    return new BitBoard(rawBlocksAfterMove);
                case Direction.Right:
                    RowShiftInfo[] shiftRightInfos = new RowShiftInfo[]
                    {
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= ((ulong)shiftRightInfos[i].row) << (16 * i);
                        reward += shiftRightInfos[i].reward;
                    }
                    return new BitBoard(rawBlocksAfterMove);
                default:
                    return new BitBoard(rawBlocks);
            }
        }
        public ulong MoveRaw(Direction direction, out int reward)
        {
            ulong rawBlocksAfterMove, transposedRawBlocks;
            reward = 0;
            rawBlocksAfterMove = rawBlocks;
            switch (direction)
            {
                case Direction.Up:
                    transposedRawBlocks = BitBoardOperationSet.Transpose(rawBlocks);
                    ColumnShiftInfo[] shiftUpInfos = new ColumnShiftInfo[]
                    {
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftUpTable[(transposedRawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= shiftUpInfos[i].column << (4 * i);
                        reward += shiftUpInfos[i].reward;
                    }
                    return rawBlocksAfterMove;
                case Direction.Down:
                    transposedRawBlocks = BitBoardOperationSet.Transpose(rawBlocks);
                    ColumnShiftInfo[] shiftDownInfos = new ColumnShiftInfo[]
                    {
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.columnShiftDownTable[(transposedRawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= shiftDownInfos[i].column << (4 * i);
                        reward += shiftDownInfos[i].reward;
                    }
                    return rawBlocksAfterMove;
                case Direction.Left:
                    RowShiftInfo[] shiftLeftInfos = new RowShiftInfo[]
                    {
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftLeftTable[(rawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= ((ulong)shiftLeftInfos[i].row) << (16 * i);
                        reward += shiftLeftInfos[i].reward;
                    }
                    return rawBlocksAfterMove;
                case Direction.Right:
                    RowShiftInfo[] shiftRightInfos = new RowShiftInfo[]
                    {
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 0) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 16) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 32) & 0xFFFF],
                        BitBoardShiftTableSet.rowShiftRightTable[(rawBlocks >> 48) & 0xFFFF]
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        rawBlocksAfterMove ^= ((ulong)shiftRightInfos[i].row) << (16 * i);
                        reward += shiftRightInfos[i].reward;
                    }
                    return rawBlocksAfterMove;
                default:
                    return rawBlocks;
            }
        }
        public bool MoveCheck(Direction direction)
        {
            int reward;
            return rawBlocks != MoveRaw(direction, out reward);
        }
        public int TileCount(int tile)
        {
            int result = 0;
            for (int i = 0; i < 64; i += 4)
            {
                if (BitBoardScoreTableSet.tileScoreTable[((rawBlocks >> i) & 0xf)] == tile)
                {
                    result++;
                }
            }
            return result;
        }
    }
}
