using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string enemyName;
    public int enemyHP;
    public bool runner;
    public bool faster;

    [Space]
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
    private Transform player; // Referência para o jogador

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
                if(navAgent.enabled){
                    navAgent.SetDestination(player.position);
                }
                
                if (isDead)
                {
                    // Para o NavMeshAgent
                    if(navAgent.enabled){
                        navAgent.isStopped = true;
                    }
                    
                } else {
                    // Verifica se o jogador está dentro da distância de parada
                    if (distanceToPlayer <= stoppingDistance)
                    {
                        // Para o NavMeshAgent
                        if(navAgent.enabled){
                            navAgent.isStopped = true;
                        }
                        enemyAnimator.SetBool("IsWalking", false);
                        enemyAnimator.SetBool("IsRunning", false);
                        enemyAnimator.SetBool("IsFaster", false);
                    }
                    else
                    {
                        // Continua seguindo o jogador
                        if(navAgent.enabled){
                            navAgent.isStopped = false;
                        }
                        
                        if (runner)
                        {
                            enemyAnimator.SetBool("IsRunning", true);
                        } else if (faster) {
                            enemyAnimator.SetBool("IsFaster", true);
                        } else {
                            enemyAnimator.SetBool("IsWalking", true);
                        }
                    }
                }
            }
        }
    }

    public bool TakeHit(int damage)
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
                if(navAgent.enabled){
                    navAgent.enabled = false;
                }
                enemyAnimator.SetBool("IsDead", true);
            } else {
                Destroy(gameObject);
            }

            return true;
        }

        return false;
    }

    void ResetMaterial()
    {
        // Restaura o material original
        rend.material = originalMaterial;
    }
}
