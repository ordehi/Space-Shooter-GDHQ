using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotation = 20f;

    [SerializeField]
    private GameObject _explosion;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //rotate object on the z axis
        //3m/s
        transform.Rotate(Vector3.forward * _rotation * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(GetComponent<Collider2D>());
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
        }

        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            player.Damage();
            player.Damage();
            player.Damage();
            Destroy(this.gameObject, 0.25f);
        }
    }
}
