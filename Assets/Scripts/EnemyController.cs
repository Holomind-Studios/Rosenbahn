using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string enemyName;
    public int enemyHP;

    [Space]
    public Transform player; // Referência para o jogador
    public GameObject enemyBaseMesh;
    public float chaseDistance = 10f; // Distância de perseguição
    public float stoppingDistance = 1f; // Distância de parada
    public Material hitMaterial; // Material para quando o inimigo tomar hit
    private Material originalMaterial; // Material original do objeto
    private Renderer rend; // Referência para o Renderer
    private NavMeshAgent navAgent; // Referência para o NavMeshAgent
    private Animator enemyAnimator;
    private bool isDead = false;
    // Referência para o colisor que você deseja desativar
    private Collider enemyCollider;
    private string enemyTag;

    void Start()
    {
        enemyTag = gameObject.tag;
        enemyCollider = gameObject.GetComponent<Collider>();

        if(enemyTag == "Enemy"){
            enemyAnimator = enemyBaseMesh.GetComponent<Animator>();
            navAgent = GetComponent<NavMeshAgent>();
            rend = GetComponent<Renderer>(); // Obtém o componente Renderer
            originalMaterial = rend.material; // Salva o material original do objeto
        }
    }

    void Update()
    {
        if (enemyTag == "Enemy")
        {
            CommonBehaviour();
        } else {
            BoidBehaviour();
        }
    }

    void BoidBehaviour()
    {
        //
    }

    void CommonBehaviour()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Verifica se o jogador está dentro da distância de perseguição
            if (distanceToPlayer <= chaseDistance)
            {
                // Define a posição do jogador como o destino do NavMeshAgent
                navAgent.SetDestination(player.position);

                if (isDead)
                {
                    // Para o NavMeshAgent
                    navAgent.isStopped = true;
                } else {
                    // Verifica se o jogador está dentro da distância de parada
                    if (distanceToPlayer <= stoppingDistance)
                    {
                        // Para o NavMeshAgent
                        navAgent.isStopped = true;
                        enemyAnimator.SetBool("IsRunning", false);
                    }
                    else
                    {
                        // Continua seguindo o jogador
                        navAgent.isStopped = false;
                        enemyAnimator.SetBool("IsRunning", true);
                    }
                }
            }
        }
    }

    public void TakeHit(int damage)
    {
        enemyHP -= damage;

        if (enemyTag == "Enemy")
        {
            rend.material = hitMaterial;
            Invoke("ResetMaterial", 0.1f);
        }
        
        if(enemyHP <= 0)
        {
            enemyCollider.enabled = false;
            isDead = true;

            if(enemyTag == "Enemy")
            {
                enemyAnimator.SetBool("IsDead", true);
            } else {
                Destroy(gameObject);
            }
        }
    }

    void ResetMaterial()
    {
        // Restaura o material original
        rend.material = originalMaterial;
    }
}
