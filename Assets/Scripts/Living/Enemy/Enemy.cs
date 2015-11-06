using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking }
    State currentState;
    public ParticleSystem deathEffect;

    NavMeshAgent pathFinder;
    Transform target;
    Material skinMaterial;
    Color originalColor;

    LivingEntity targetEntity;
    bool hasTarget;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float damage = 1;

    float collisionRadius;
    float targetCollisionRadius;

    void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        currentState = State.Chasing;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            collisionRadius = GetComponent<CapsuleCollider>().radius;
            targetEntity = target.GetComponent<LivingEntity>();
        }
    }

    protected override void Start() {
        base.Start();
        if (hasTarget)
        {
            StartCoroutine(UpdatePath());
            targetEntity.OnDeath += OnTargetDeath;
        }
    }

    public override void Takehit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.Takehit(damage, hitPoint, hitDirection);
    }

    public void SetCharacteristics(float moveSpeed, int _damage, float health, Color skinColor){
        pathFinder.speed = moveSpeed;
        damage = _damage;
        startingHealth = health;
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinColor;
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

	void Update () {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + targetCollisionRadius + collisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathFinder.enabled = false;
        skinMaterial.color = Color.red;

        Vector3 originalPosition = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (collisionRadius);


        float attackSpeed = 3;
        float percentOfAttackDone = 0;

        bool hasAppliedDamage = false;
        while(percentOfAttackDone <= 1)
        {
            if(percentOfAttackDone >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percentOfAttackDone += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percentOfAttackDone,2) + percentOfAttackDone) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        currentState = State.Chasing;
        pathFinder.enabled = true;
        skinMaterial.color = originalColor;
    }

    IEnumerator UpdatePath() {
        float refreshRate = 0.25f;
        while (hasTarget) {
            if (currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (collisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (isAlive)
                {
                  //  pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
            
        }
    }
}
