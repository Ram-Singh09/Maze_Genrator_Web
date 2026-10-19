
public struct Vec2i
{
    public int X = 0, Y = 0;

    public Vec2i(int a_x, int a_y)
    {
        X = a_x;
        Y = a_y;
    }
    public Vec2i(Vec2i Val)
    {
        X = Val.X;
        Y = Val.Y;
    }

    public static bool operator ==(Vec2i A, Vec2i B) => A.X == B.X && A.Y == B.Y;
    public static bool operator !=(Vec2i A, Vec2i B) => A.X != B.X || A.Y != B.Y;
}

enum Result_Type
{
    Success, Grid_Too_Small, Grid_Too_Big
}

sealed record Generated_Maze(Maze_Cell[] Cells, int Len_x, int Len_y);
sealed record Maze_Req(int Seed);