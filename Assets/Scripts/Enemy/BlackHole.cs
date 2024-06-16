using UnityEngine;

public class BlackHole : Enemy
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

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
