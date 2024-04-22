using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string enemyName;
    public int enemyHP;

    [Space]
    public Transform player; // Referência para o jogador
    public float chaseDistance = 10f; // Distância de perseguição
    public float stoppingDistance = 1f; // Distância de parada
    public Material hitMaterial; // Material para quando o inimigo tomar hit
    private Material originalMaterial; // Material original do objeto
    private Renderer rend; // Referência para o Renderer
    private NavMeshAgent navAgent; // Referência para o NavMeshAgent

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>(); // Obtém o componente Renderer
        originalMaterial = rend.material; // Salva o material original do objeto
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Verifica se o jogador está dentro da distância de perseguição
            if (distanceToPlayer <= chaseDistance)
            {
                // Define a posição do jogador como o destino do NavMeshAgent
                navAgent.SetDestination(player.position);

                // Verifica se o jogador está dentro da distância de parada
                if (distanceToPlayer <= stoppingDistance)
                {
                    // Para o NavMeshAgent
                    navAgent.isStopped = true;
                }
                else
                {
                    // Continua seguindo o jogador
                    navAgent.isStopped = false;
                }
            }
        }
    }

    public void TakeHit(int damage)
    {
        enemyHP -= damage;
        // Aplica o material de hit
        rend.material = hitMaterial;

        // Chama a função para retornar ao material original após 1 segundo
        Invoke("ResetMaterial", 0.1f);

        if(enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ResetMaterial()
    {
        // Restaura o material original
        rend.material = originalMaterial;
    }
}
