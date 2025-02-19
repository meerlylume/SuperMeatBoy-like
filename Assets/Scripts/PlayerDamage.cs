using System;
using System.Collections;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private Transform _lastCheckpoint;
    [SerializeField] private ParticleSystem _deathParticles;
    private PlayerMovement _playerMovement;
    private Coroutine _respawnRoutine;
    private bool _deathParticlesPlayedOnce;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checkpoint handling
        Checkpoint thisCheckpoint = collision.gameObject.GetComponent<Checkpoint>();

        if (!thisCheckpoint) return;

        if (thisCheckpoint.GetEnabled()) return;

        _lastCheckpoint = collision.transform;
        thisCheckpoint.SetEnabled(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DamagingHazard>())
        {
            Die(collision.transform);
        }
    }

    private void Die(Transform deathPos)
    {
        _playerMovement.SetCanMove(false);
        _playerMovement.VelocityReset();
        if (!_deathParticlesPlayedOnce)
        {
            Instantiate(_deathParticles, deathPos);
            _deathParticlesPlayedOnce = true;
        }

        _respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        //Transition in

        //Respawn
        if (!_lastCheckpoint)
        {
            Debug.Log("No checkpoint found");
            _playerMovement.SetCanMove(true);
            yield break;
        }

        this.transform.position = _lastCheckpoint.position;

        //Transition out

        //Let the player move
        _playerMovement.SetCanMove(true);
        _deathParticlesPlayedOnce = false;

        yield break;
    }
}
