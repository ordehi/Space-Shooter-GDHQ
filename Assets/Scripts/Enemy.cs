using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _enemyAnim;
    private AudioSource _explosionAudioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _explosionAudioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("Player from enemy is NULL");
        }

        _enemyAnim = GetComponent<Animator>();

        if (_enemyAnim == null)
        {
            Debug.LogError("Enemy Animator is NULL");
        }

        if (_explosionAudioSource == null)
        {
            Debug.LogError("Explosion audio from enemy is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }

    }
    void CalculateMovement()
    {
        //move down at 4m per sec
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if bottom of screen
        //respawn at top with new random x position
        if (transform.position.y < -5.60)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 6.97f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Player player = other.transform.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage();
            }
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _explosionAudioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddToScore(10);
            }
            else if (_player == null)
            {
                Debug.LogError("Player is null");
            }
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _explosionAudioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}
