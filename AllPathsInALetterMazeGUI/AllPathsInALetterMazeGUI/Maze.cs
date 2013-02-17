using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllPathsInALetterMazeGUI
{
    public class Maze
    {
        internal Cell[,] maze;
        private Cell startingPos;
        private List<Path> paths;

        public Maze(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(String.Format("File \"{0}\" was not found.", filePath));
            }
            StreamReader reader = new StreamReader(filePath);
            startingPos = null;
            int size = int.Parse(reader.ReadLine()); // it's always in the first line by definition
            this.maze = new Cell[size, size];
            for (int i = 0; i < size; i++)
            {
                string line = reader.ReadLine();
                for (int p = 0; p < line.Length; p += 2)
                {
                    maze[i, p / 2] = new Cell(i, p / 2, line[p], null);
                    if (line[p] == '*')
                    {
                        startingPos = maze[i, p / 2];
                    }
                }
            }
            if (startingPos == null)
            {
                throw new Exception("No staring posiiton in the maze.");
            }
            reader.Close();
        }

        public List<Path> FindAllPaths()
        {
            paths = new List<Path>();
            Queue<Cell> queue = new Queue<Cell>();
            queue.Enqueue(startingPos);
            while (queue.Count > 0)
            {
                Cell current = queue.Dequeue();

                Cell leftNeighbour = GetLeftNeighbour(current);
                if (leftNeighbour == null)
                {
                    Path newPath = new Path(current);
                    if (!paths.Contains(newPath))
                        paths.Add(newPath);
                }
                else if (leftNeighbour.Value != '#')
                {
                    Cell leftNeighbourUpdated = new Cell(leftNeighbour.Row, leftNeighbour.Column,
                        leftNeighbour.Value, current);
                    if (!current.HasPredecessor(leftNeighbourUpdated)) // if not already has predecessor
                    {
                        queue.Enqueue(leftNeighbourUpdated);
                    }
                }

                Cell topNeighbour = GetTopNeighbour(current);
                if (topNeighbour == null)
                {
                    Path newPath = new Path(current);
                    if (!paths.Contains(newPath))
                        paths.Add(newPath);
                }
                else if (topNeighbour.Value != '#')
                {
                    Cell topNeighbourUpdated = new Cell(topNeighbour.Row, topNeighbour.Column,
                        topNeighbour.Value, current);
                    if (!current.HasPredecessor(topNeighbourUpdated))
                    {
                        queue.Enqueue(topNeighbourUpdated);
                    }
                }

                Cell rightNeighbour = GetRightNeighbour(current);
                if (rightNeighbour == null)
                {
                    Path newPath = new Path(current);
                    if (!paths.Contains(newPath))
                        paths.Add(newPath);
                }
                else if (rightNeighbour.Value != '#')
                {
                    Cell rightNeighbourUpdated = new Cell(rightNeighbour.Row, rightNeighbour.Column,
                        rightNeighbour.Value, current);
                    if (!current.HasPredecessor(rightNeighbourUpdated))
                    {
                        queue.Enqueue(rightNeighbourUpdated);
                    }
                }

                Cell bottomNeighbour = GetBottomNeighbour(current);
                if (bottomNeighbour == null)
                {
                    Path newPath = new Path(current);
                    if (!paths.Contains(newPath))
                        paths.Add(newPath);
                }
                else if (bottomNeighbour.Value != '#')
                {
                    Cell bottomNeighbourUpdated = new Cell(bottomNeighbour.Row, bottomNeighbour.Column,
                        bottomNeighbour.Value, current);
                    if (!current.HasPredecessor(bottomNeighbourUpdated)) 
                    {
                        queue.Enqueue(bottomNeighbourUpdated);
                    }
                }
            }
            return paths;
        }


        public Cell GetLeftNeighbour(Cell current)
        {
            if (current.Column - 1 < 0)
            {
                return null; // indicates reaching an exit
            }
            return maze[current.Row, current.Column - 1];
        }

        public Cell GetTopNeighbour(Cell current)
        {
            if (current.Row - 1 < 0)
            {
                return null;
            }
            return maze[current.Row - 1, current.Column];
        }

        public Cell GetRightNeighbour(Cell current)
        {
            if (current.Column + 1 >= maze.GetLength(1))
            {
                return null;
            }
            return maze[current.Row, current.Column + 1];
        }

        public Cell GetBottomNeighbour(Cell current)
        {
            if (current.Row + 1 == maze.GetLength(0))
            {
                return null;
            }
            return maze[current.Row + 1, current.Column];
        }

    }

    public class Cell
    {
        private int row;
        private int col;
        private char value;
        private Cell predecessor;

        public Cell(int row, int col, char value, Cell predecessor)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.predecessor = predecessor;
        }

        public int Row
        {
            get
            {
                return this.row;
            }
            set
            {
                this.row = value;
            }
        }

        public int Column
        {
            get
            {
                return this.col;
            }
            set
            {
                this.col = value;
            }
        }

        public char Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public Cell Predecessor
        {
            get
            {
                return this.predecessor;
            }
            set
            {
                this.predecessor = value;
            }
        }

        public bool HasPredecessor(Cell other)
        {
            Path pathSoFar = new Path(this);
            if (pathSoFar.ContainsCell(other))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override bool Equals(object obj)
        {
            Cell other = (Cell)obj;
            if (this.row == other.Row && this.Column == other.Column)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return row.GetHashCode() * col.GetHashCode() * value.GetHashCode();
        }



    }

    public class Path
    {
        private List<Cell> cells;

        public Path(Cell exitCell)
        {
            cells = new List<Cell>();
            Cell current = exitCell;
            while (current != null)
            {
                cells.Add(current);
                current = current.Predecessor;
            }
            cells.Reverse();
        }
        public Path(List<Cell> cells)
        {
            this.cells = cells;
        }


        public List<Cell> Cells
        {
            get
            {
                return this.cells;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < cells.Count; i++)
            {
                sb.Append(cells[i].Value + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            return sb.ToString();
        }

        public bool ContainsCell(Cell cell)
        {
            if (cells.Contains(cell))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public override bool Equals(object obj)
        {
            Path other = (Path)obj;
            if (this.cells.Count != other.cells.Count)
                return false;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] != other.cells[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return cells.GetHashCode();
        }
    }
            

        
}
