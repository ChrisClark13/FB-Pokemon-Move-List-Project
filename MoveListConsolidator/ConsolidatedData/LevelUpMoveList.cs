using System;
using System.Collections.Generic;

namespace MoveListConsolidator.ConsolidatedData
{
    public class LevelUpMoveList
    {
        public string Form;
        public List<LevelUpMove> LevelUpMoves = new List<LevelUpMove>();

        public void AddMove(string name, uint level)
        {
            var existingEntry = LevelUpMoves.Find(move => move.Name == name);
            if (existingEntry == null)
            {
                LevelUpMoves.Add(new LevelUpMove {
                    Name = name,
                    Level = level
                });
            }
            else
            {
                existingEntry.Level = Math.Min(existingEntry.Level, level);
            }
        }

        public void SortMoves()
        {
            LevelUpMoves.Sort((a,b) => a.Level.CompareTo(b.Level));
        }
    }
}