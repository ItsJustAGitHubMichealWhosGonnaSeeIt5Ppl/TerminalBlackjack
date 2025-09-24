public enum Anchor { // TODO Create an X anchor and Y anchor
    TopLeft,
    TopCenter,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    BottomCenter,
    BottomRight,
};
public enum Direction {
    PosX,
    PosY,
    NegX,
    NegY,
}

public class Coordinate
{
    public int x;
    public int y;
    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Coordinate()
    {
        this.x = 0;
        this.y = 0;
    }
//TODO convert all coordinates to this
}