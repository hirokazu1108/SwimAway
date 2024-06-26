using UnityEngine;

public class PinnedEnemy : Enemy
{
    private Vector3 _originalPosition = Vector3.zero;

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void Update()
    {
        Move();
    }

    public override void Hit()
    {

    }

    public override void Move()
    {
        if ((_originalPosition - transform.position).magnitude > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, speed * Time.deltaTime);
        }
    }
}
