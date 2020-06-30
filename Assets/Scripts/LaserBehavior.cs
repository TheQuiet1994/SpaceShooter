using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;
    private bool _isPlayerLaser = true;
    [SerializeField]
    private GameObject _explosion = null;

    void Update()
    {
        if (_isPlayerLaser == false)
        {
            MoveDown();
        }
        else
        {
            MoveUp();     
        }     
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= 8f)
        {
            if (transform.parent != null)
            {
                Destroy(this.gameObject, 3f);
                Destroy(transform.parent.gameObject, 3f);
            }
            else
            {
                Destroy(this.gameObject, 3f);
            }
        }
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -8f)
        {
            if (transform.parent != null)
            {
                Destroy(this.gameObject, 3f);
                Destroy(transform.parent.gameObject, 3f);
            }
            else
            {
                Destroy(this.gameObject, 3f);
            }
        }
    }
    public void AssignEnemy()
    {
        _isPlayerLaser = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isPlayerLaser == false)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                GameObject laserExplosion = Instantiate(_explosion, transform.position + new Vector3(0f, -2f, 0), Quaternion.identity);
                laserExplosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Destroy(this.gameObject);
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
