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
     * ���������������Z�b�g
     * 
     * ����
     *     �E���� => Direction.Forward
     *     �E���� => Direction.Back
     *     �E  0  => Direction.None
    */
    public void setHorizontal(int x)
    {
        _x = x > 0 ? Direction.Forward : (x < 0 ? Direction.Back : Direction.None);
    }

    /*
     * ���������������Z�b�g
     * 
     * ����
     *     �E���� => Direction.Up
     *     �E���� => Direction.Down
     *     �E  0  => Direction.None
    */
    public void setVertical(int y)
    {
        _y = y > 0 ? Direction.Up : (y < 0 ? Direction.Down : Direction.None);
    }

    //���������������t�ɂ���
    public void inverseX()
    {
        if (_x == Direction.None) return;

        _x = _x == Direction.Forward ? Direction.Back : Direction.Forward;
    }

    //���������������t�ɂ���
    public void inverseY()
    {
        if (_y == Direction.None) return;

        _y = _y == Direction.Up ? Direction.Down : Direction.Up;
    }
}
