using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Locator
{
    private static GridiaGame _game;
    private static ConnectionToGridiaServerHandler _conn;

    public static GridiaGame GetGame()
    {
        return _game;
    }

    public static GridiaGame GetConnection()
    {
        return _conn;
    }

    public static void Provide(GridiaGame game) 
    {
        _game = game;
    }

    public static void Provide(ConnectionToGridiaServerHandler conn)
    {
        _conn = conn;
    }
}
