using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private bool _enabled = false;

    public bool GetEnabled() { return _enabled; }
    public void SetEnabled(bool value) { _enabled = value; }

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_enabled) return;
        
        if (collision.gameObject.GetComponent<PlayerDamage>())
        {
            _sprite.color = Color.green;
        }
    }
}
