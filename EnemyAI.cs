using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAIScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [Header("Enemy Stats")]
    public int attackDamage = 20;  // Changed to int
    public float attackCooldown = 2f;
    private bool canAttack = true;

    [Header("Detection Settings")]
    public float followRange = 15f;
    public float attackRange = 2f;

    // Gizmo Colors
    [Header("Debug Visualization")]
    public Color followRangeColor = new Color(0, 1, 0, 0.2f); // Green 
    public Color attackRangeColor = new Color(1, 0, 0, 0.2f); // Red 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Simple state machine
        if (distanceToPlayer <= attackRange)
        {
            // Attack
            if (canAttack)
            {
                Attack();
            }
        }
        else if (distanceToPlayer <= followRange)
        {
            // Chase player
            agent.SetDestination(player.position);
        }
    }

    void Attack()
    {
        // Start attack cooldown
        canAttack = false;
        StartCoroutine(AttackCooldown());

        // Deal damage to player
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);  // Fixed the split line
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = followRangeColor;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}