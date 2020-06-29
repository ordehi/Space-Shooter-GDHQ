using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private int _score;
    [SerializeField]
    private GameObject _explosion;

    private UIManager _uiManager;
    private PostProcessVolume _postProcessVolume;
    [SerializeField]
    private AudioClip _laserAudio;
    private AudioSource _audioSource;
    private AudioSource _explosionAudioSource;
    //variable to store audio clip

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _postProcessVolume = GameObject.Find("Post Process Volume").GetComponent<PostProcessVolume>();
        _audioSource = GetComponent<AudioSource>();

        if (_postProcessVolume == null)
        {
            Debug.LogError("Post Process on player is NULL");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager on player is null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UIManager on player is null");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudiSource on Player is null");
        }
        else
        {
            _audioSource.clip = _laserAudio;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    private void TurnOnChromaticAberration()
    {
        if (_postProcessVolume != null)
        {
            ChromaticAberration chromaticAberration;
            if (_postProcessVolume.profile.TryGetSettings(out chromaticAberration))
            {
                chromaticAberration.intensity.value = 1;
            }
        }
    }

    private void TurnOffChromaticAberration()
    {
        if (_postProcessVolume != null)
        {
            ChromaticAberration chromaticAberration;
            if (_postProcessVolume.profile.TryGetSettings(out chromaticAberration))
            {
                chromaticAberration.intensity.value = 0;
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
        

        //wrapping player from one edge to another
        //by changing the transform position to the value of the opposite edge in the given axis
        if (transform.position.x >= 10)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.10)
        {
            transform.position = new Vector3(9.90f, transform.position.y, 0);
        }

        if (transform.position.y >= 7)
        {
            transform.position = new Vector3(transform.position.x, -5.56f, 0);
        }
        else if (transform.position.y <= -5.60)
        {
            transform.position = new Vector3(transform.position.x, 6.97f, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
        
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _playerShield.SetActive(false);
            return;
        }
        else
        {
            _lives--;

            if (_lives == 2)
            {
                _rightEngine.SetActive(true);
            }
            else if (_lives == 1)
            {
                _leftEngine.SetActive(true);
            }
            _uiManager.UpdateLives(_lives);
        }

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            //_uiManager.GameOver();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        TurnOnChromaticAberration();
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
        TurnOffChromaticAberration();
    }

    public void ShieldBoostActive()
    {
        _playerShield.SetActive(true);
        _isShieldActive = true;
    }

    //method to add 10 to score
    //communicate with UI manager and update score

    public void AddToScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
