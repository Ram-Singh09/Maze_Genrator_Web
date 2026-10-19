using Microsoft.AspNetCore.Http.Json;

// builder options setup
var Builder = WebApplication.CreateBuilder(args);
Builder.Services.ConfigureHttpJsonOptions((JsonOptions Opts) => { Opts.SerializerOptions.IncludeFields = true; });

// don't change builder settings after this
var App = Builder.Build();
App.UseDefaultFiles(); // for allowing usage of wwwroot and etc folders
App.UseStaticFiles(); // actually sends files like index.html

const byte Maze_Width = 40, Maze_Height = 30;

App.MapPost("/GetMaze", (Maze_Req Req) =>
{
    try
    {
        var Cells = new Maze_Cell[Maze_Height, Maze_Width];
        var State = Maze_Generator.Gen(Cells, Req.Seed);

        if (State != Result_Type.Success)
            return Results.Problem(Get_Error_Msg(State));

        var Ret_cells = new Maze_Cell[Maze_Width * Maze_Height];

        for (int i = 0; i < Maze_Height; i++)
            for (int j = 0; j < Maze_Width; j++)
                Ret_cells[i * Maze_Width + j] = Cells[i, j];

        return Results.Ok(new Generated_Maze(Ret_cells, Maze_Width, Maze_Height));
    }
    catch(Exception Ex)
    {
        Console.WriteLine(Ex.Message);
        return Results.Problem("Unknown error");
    }
});

App.Run();

static string Get_Error_Msg(Result_Type X)
{
    return X switch
    {
        Result_Type.Grid_Too_Small => "Grid is too small",
        Result_Type.Grid_Too_Big => "Grid is too big",
        _ => "Unknown Error"
    };
}
