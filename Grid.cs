using System;
using System.Collections.Generic;
using System.Text;

namespace Jewels_Columns_Student
{
    static class Grid
    {
        public const int NCOLUMNS = 7;
        public const int NROWS = 13;
        private static Jewels[,] grid;

        static Grid()
        {
            grid = new Jewels[NROWS, NCOLUMNS];
        }

        public static bool CheckPosition(Position p)
        {
            if (p.row >= 0 && p.row < NROWS &&
                p.col >= 0 && p.col < NCOLUMNS &&
                grid[p.row, p.col] == null)
            {
                return true;
            }
            return false;
        }
        public static bool UpdateGrid(Triplet t)
        {
            Position p = t.GetPosition();
            List<Jewels> ThreeJewels = t.GetThreeJewels();
            if (p.row + 1 >= NROWS || grid[p.row + 1, p.col] != null)
            {
                try
                {
                    grid[p.row, p.col] = ThreeJewels[0];
                    grid[p.row - 1, p.col] = ThreeJewels[1];
                    grid[p.row - 2, p.col] = ThreeJewels[2];
                }
                catch (Exception) { }

                Position pMagic = new Position(0, 0);
                if (t.ContainMagic(ref pMagic))
                    RemoveMagic(pMagic);
                else
                    CheckGrid(p, ThreeJewels);

                return true;
            }

            return false;
        }
        private static void CheckGrid(Position p, List<Jewels> ThreeJewels)
        {
            // posició del jewels respecte p
            List<Position> j_columns = new List<Position>()
            { new Position (0,0), new Position (-1,0), new Position (-2,0)};
            // direccions a comprovar: fila, columna, diagonal cap amunt, diagonal cap avall
            List<Position> directions = new List<Position>()
            { new Position(0, 1), new Position(1, 0), new Position(-1, 1), new Position(1, 1) };

            List<Position> remove = null;

            for (int i = 0; i < ThreeJewels.Count; i++)
            {
                Position pos = p + j_columns[i];
                foreach (Position dir in directions)
                {
                    remove = CountRemove(pos, ThreeJewels[i], dir);
                    if (remove.Count >= 3) break;
                    remove.Clear();
                }
                if (remove.Count >= 3) break;
            }
            while (remove.Count >= 3)
            {
                Hud.Update(remove.Count);
                remove.Sort();
                foreach (Position pos in remove)
                {
                    Remove(pos);
                }
                remove = CheckGrid();
            }
        }
        private static List<Position> CheckGrid()
        {
            // direccions a comprovar: fila, columna, diagonal cap amunt, diagonal cap avall
            List<Position> directions = new List<Position>()
            { new Position(0, 1), new Position(1, 0), new Position(-1, 1), new Position(1, 1) };

            List<Position> remove = new List<Position>();
            for (int i = 0; i < NROWS; i++)
            {
                for (int j = 0; j < NCOLUMNS; j++)
                {
                    if (grid[i, j] != null)
                    {
                        Position pos = new Position(i, j);
                        foreach (Position dir in directions)
                        {
                            remove = CountRemove(pos, grid[i, j], dir);
                            if (remove.Count >= 3) break;
                            remove.Clear();
                        }
                    }
                    if (remove.Count >= 3) break;
                }
                if (remove.Count >= 3) break;
            }
            return remove;
        }
        private static List<Position> CountRemove(Position p, Jewels j, Position dir)
        {
            List<Position> remove = new List<Position>();
            remove.Add(p);
            Position newP = p + dir;
            while (newP.row >= 0 && newP.row < NROWS
                && newP.col >= 0 && newP.col < NCOLUMNS
                && grid[newP.row, newP.col] != null
                && grid[newP.row, newP.col].Equals(j))
            {
                remove.Add(new Position(newP.row, newP.col));
                newP += dir;
            }
            newP = p - dir;
            while (newP.row >= 0 && newP.row < NROWS
                && newP.col >= 0 && newP.col < NCOLUMNS
                && grid[newP.row, newP.col] != null
                && grid[newP.row, newP.col].Equals(j))
            {
                remove.Add(new Position(newP.row, newP.col));
                newP -= dir;
            }
            return remove;
        }

        private static void Remove(Position p)
        {
            int row = p.row;
            while (row > 1)
            {
                grid[row, p.col] = grid[row - 1, p.col];
                row--;
            }
            grid[0, p.col] = null;
        }
        private static void RemoveMagic(Position p)
        {
            List<Position> directions = new List<Position>()
            { new Position(0, 1), new Position(1, 0), new Position(-1, 1), new Position(1, 1) };

            List<Position> remove = new List<Position>();
            List<Position> removedir = new List<Position>();
            foreach (Position dir in directions)
            {
                removedir = CountRemove(p, new Magic(), dir);
                remove.AddRange(removedir);
            }

            while (remove.Count >= 3)
            {
                Hud.Update(remove.Count);
                remove.Sort();
                foreach (Position pos in remove)
                {
                    Remove(pos);
                }
                remove = CheckGrid();
            }


        }


        public static void Draw()
        {
            for (int i = 0; i < NROWS; i++)
            {
                for (int j = 0; j < NCOLUMNS; j++)
                {
                    try
                    {
                        Console.SetCursorPosition(j + 1, i + 1);
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        if (grid[i, j] != null)
                            grid[i, j].Draw();
                        else
                            Console.Write(' ');
                    }
                    catch (Exception) { }
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
        public static bool GameOver()
        {
            for (int i = 0; i < NCOLUMNS; i++)
            {
                if (grid[0, i] != null) return true;
            }
            return false;
        }

    }
}
