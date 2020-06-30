using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    //ID for Powerups
    //0 = TripleShot
    //1 = Speed
    //2 = Shields
    //3 = Ammo
    //4 = Health
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        LeaveMap();

    }

    private void LeaveMap()
    {
        if (transform.position.y < -6)
        {  
            Destroy(this.gameObject);     
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == ("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotBuffDuration();
                        break;
                    case 1:
                        player.SpeedBuffDuration();
                        break;
                    case 2:
                        player.ShieldBuff();
                        break;
                    case 3:
                        player.AmmoBuff();
                        break;
                    case 4:
                        player.HealthBuff();
                        break;
                    default:
                        break;
                }         
            }
            Destroy(this.gameObject);
        }
    }
}
