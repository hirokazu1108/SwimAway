using UnityEngine;

public class BlackHole : Enemy
{
    private void OnCollisionEnter(Collision collision)
    {
        Hit();
    }

    public override void Move()
    {

    }

    public override void Hit()
    {
        GameManager.GameOver();
    }

}
