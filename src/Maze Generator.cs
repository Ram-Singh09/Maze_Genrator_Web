
struct Maze_Cell
{
    public Maze_Generator.Cell_Type Cell_type;
    public int Step_count;
    public bool Visited;

    public Maze_Cell(Maze_Generator.Cell_Type Cell_type, int Step_count, bool Visited)
    {
        this.Cell_type = Cell_type;
        this.Step_count = Step_count;
        this.Visited = Visited;
    }
}

static class Maze_Generator
{
    public static Result_Type Gen(Maze_Cell[,] Cells, int Seed = 0)
    {
        int Len_x = Cells.GetLength(1);
        int Len_y = Cells.GetLength(0);
        const int Default_Seed = 0;

        if (Len_x < 4 || Len_y < 4) return Result_Type.Grid_Too_Small;
        if (Len_x > 255 || Len_y > 255) return Result_Type.Grid_Too_Big;

        // to prevent prev state data leaking to current
        for (int i = 0; i < Len_y; i++)
            for (int j = 0; j < Len_x; j++)
                Cells[i, j] = new(Cell_Type.Wall, 0, false);

        // for backtraking only
        using var Backtrack_cells = new Ref_List<Vec2i>((int)(Cells.Length / 1.5));
        var Current_cell = new Vec2i(1, 1);
        Backtrack_cells.Add(Current_cell);

        // dirs for valid dir finding
        using var Directions = new Ref_List<Cell_Dir>(4);
        Cells[Current_cell.Y, Current_cell.X] = new(Cell_Type.Path, 0, true);

        // randomizer
        Random Rand = Seed == Default_Seed ? new() : new(Seed);

        while (Backtrack_cells.Length > 0)
        {
            Directions.Clear();

            // left
            if (Evaluate_Neighbor(new(Current_cell.X - 1, Current_cell.Y), Cells))
                Directions.Add(Cell_Dir.Left);

            // right
            if (Evaluate_Neighbor(new(Current_cell.X + 1, Current_cell.Y), Cells))
                Directions.Add(Cell_Dir.Right);

            // up
            if (Evaluate_Neighbor(new(Current_cell.X, Current_cell.Y - 1), Cells))
                Directions.Add(Cell_Dir.Up);

            // down
            if (Evaluate_Neighbor(new(Current_cell.X, Current_cell.Y + 1), Cells))
                Directions.Add(Cell_Dir.Down);

            // move to prev cell to find dirs to move
            if (Directions.Length < 1)
            {
                Current_cell = Backtrack_cells[Backtrack_cells.Length - 1];
                Backtrack_cells.Remove_Last();
                continue;
            }

            // get dir from ones in list
            var Rand_dir = Directions[Rand.Next(0, Directions.Length)];
            int Prev_steps = Cells[Current_cell.Y, Current_cell.X].Step_count;

            Current_cell = Rand_dir switch
            {
                Cell_Dir.Up => new(Current_cell.X, Current_cell.Y - 1),
                Cell_Dir.Down => new(Current_cell.X, Current_cell.Y + 1),
                Cell_Dir.Left => new(Current_cell.X - 1, Current_cell.Y),
                Cell_Dir.Right => new(Current_cell.X + 1, Current_cell.Y),
            };

            // add new explored cell to backtracking stack
            Cells[Current_cell.Y, Current_cell.X] = new(Cell_Type.Path, Prev_steps + 1, true);
            Backtrack_cells.Add(Current_cell);
        }

        return Result_Type.Success;
    }

    private static bool Evaluate_Neighbor(Vec2i Cell, Maze_Cell[,] Cells)
    {
        byte Dirs_available = 0;

        if (Cell.X - 1 >= 0 && !Cells[Cell.Y, Cell.X -1].Visited) Dirs_available++;
        if (Cell.X + 1 < Cells.GetLength(1) && !Cells[Cell.Y, Cell.X +1].Visited) Dirs_available++;
        if (Cell.Y - 1 >= 0 && !Cells[Cell.Y -1, Cell.X].Visited) Dirs_available++;
        if (Cell.Y + 1 < Cells.GetLength(0) && !Cells[Cell.Y +1, Cell.X].Visited) Dirs_available++;

        return Dirs_available == 3;
    }

    public enum Cell_Type : byte
    {
        Wall, Path
    }
    public enum Cell_Dir : byte
    {
        Up, Down, Left, Right
    }
}
