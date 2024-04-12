public enum Direction
{
    None,
    Forward,
    Back,
    Up,
    Down,
}

public class Directions2Old
{
    private Direction _x;
    private Direction _y;

    public Direction X { get => _x; }
    public Direction Y { get => _y; }


    public Directions2Old(int x, int y)
    {
        setHorizontal(x);
        setVertical(y);
    }

    /*
     * 水平方向成分をセット
     * 
     * 引数
     *     ・正数 => Direction.Forward
     *     ・負数 => Direction.Back
     *     ・  0  => Direction.None
    */
    public void setHorizontal(int x)
    {
        _x = x > 0 ? Direction.Forward : (x < 0 ? Direction.Back : Direction.None);
    }

    /*
     * 垂直方向成分をセット
     * 
     * 引数
     *     ・正数 => Direction.Up
     *     ・負数 => Direction.Down
     *     ・  0  => Direction.None
    */
    public void setVertical(int y)
    {
        _y = y > 0 ? Direction.Up : (y < 0 ? Direction.Down : Direction.None);
    }

    //水平方向成分を逆にする
    public void inverseX()
    {
        if (_x == Direction.None) return;

        _x = _x == Direction.Forward ? Direction.Back : Direction.Forward;
    }

    //垂直方向成分を逆にする
    public void inverseY()
    {
        if (_y == Direction.None) return;

        _y = _y == Direction.Up ? Direction.Down : Direction.Up;
    }
}
