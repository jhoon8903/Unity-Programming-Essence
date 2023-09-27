using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    private NavMeshAgent navMeshAgent;
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator zombieAnimator;
    private AudioSource zombieAudioPlayer;
    private Renderer zombieRenderer;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;
    private bool hasTarget => targetEntity != null && !targetEntity.dead;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();
        zombieRenderer = GetComponentInChildren<Renderer>();
    }

    // private void Setup(ZombieData zombieData)
    // {
    //     startingHealth = zombieData.health;
    //     health = zombieData.health;
    //     damage = zombieData.damage;
    //     navMeshAgent.speed = zombieData.speed;
    //     zombieRenderer.material.color = zombieData.skinColor;
    // }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    private IEnumerator UpdatePath()
    {
        while (!dead)
        {
            if (hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true;
                var colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                foreach (var t in colliders)
                {
                    var livingEntity = t.GetComponent<LivingEntity>();
                    if (livingEntity == null || livingEntity.dead) continue;
                    targetEntity = livingEntity;
                    break;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            zombieAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        base.Die();
        var zombieColliders = GetComponents<Collider>();
        foreach (var zombie in zombieColliders)
        {
            zombie.enabled = false;
        }
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        if (dead || !(Time.time >= lastAttackTime + timeBetAttack)) return;
        var attackTarget = other.GetComponent<LivingEntity>();
        if (attackTarget == null || attackTarget != targetEntity) return;
        lastAttackTime = Time.time;
        var position = transform.position;
        var hitPoint = other.ClosestPoint(position);
        var hitNormal = position - other.transform.position;
        attackTarget.OnDamage(damage, hitPoint, hitNormal);
    }
}
