using UnityEngine;

public class InkPanel : MonoBehaviour
{
    private Animator _animator;

    private bool _visibleInk = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        if (_visibleInk) return;

        _visibleInk = true;
        gameObject.SetActive(_visibleInk);
        _animator.SetBool("inkAnim", _visibleInk);

        Invoke("StopAnim", 1 * (1/0.2f));    //Layer�g�킸�b�ł��Ă܂��A�߂�ǂ�����
    }

    private void StopAnim()
    {
        if (!_visibleInk) return;

        _visibleInk = false;
        gameObject.SetActive(_visibleInk);
        _animator.SetBool("inkAnim", _visibleInk);
    }
}
