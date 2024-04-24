using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int killCounter = 0;
    public int collectedBloodCounter = 150;

    public GameObject fountainPiece;
    public GameObject UIbloodCounter;
    public GameObject UIinteractionGO;
    public GameObject UICrosshair;


    [Space]
    public GameObject[] demonPrefabs; // O GameObject que você quer instanciar
    public Transform demonSpawnPoint1; // O ponto onde o objeto será instanciado
    public Transform demonSpawnPoint2;
    public Transform demonSpawnPoint3;    

    [Space]
    public int wave0DemonLimit = 50;
    public int wave0DemonCounter = 0;
    public int wave1DemonLimit = 100;
    public int wave1DemonCounter = 0;

    private Text UIbloodCounterText;
    private Text UIinteractionText;
    private float spawnInterval = 1f; // Intervalo de tempo em segundos
    private float timer; // Temporizador para controlar o intervalo de instância
    private int wave = 0;

    [HideInInspector]
    public int bloodCounter = 0;
    public List<GameObject> bloodList;
    public List<GameObject> deadEnemies;

    void Start()
    {
        UIinteractionText = UIinteractionGO.GetComponent<Text>();
        UIbloodCounterText = UIbloodCounter.GetComponent<Text>();
        UIbloodCounterText.text = "150";
        UIinteractionText.text = "";
    }

    void Update()
    {
        Interactor();
        UIbloodCounterText.text = collectedBloodCounter.ToString();
        // Atualize o temporizador
        timer += Time.deltaTime;

        
        if (wave == 0 && wave0DemonCounter <= wave0DemonLimit)
        {
            // Verifique se o temporizador atingiu o intervalo desejado
            if (timer >= spawnInterval)
            {
                // Chame a função para instanciar o GameObject
                SpawnEnemiesWave0();

                // Reinicie o temporizador
                timer = 0f;
            }
        } else if (wave == 1 && wave1DemonCounter <= wave1DemonLimit)
        {
            // Verifique se o temporizador atingiu o intervalo desejado
            if (timer >= spawnInterval)
            {
                // Chame a função para instanciar o GameObject
                SpawnEnemiesWave1();

                // Reinicie o temporizador
                timer = 0f;
            }
        }
    }

    void Interactor()
    {
        // Dispara um raio para frente a partir da posição da câmera do jogador
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Define o comprimento máximo do raio
        float maxDistance = 2f;

        // Executa o Raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Verifica se o objeto atingido possui a tag "WishFountain"
            if (hit.collider.CompareTag("WishFountain"))
            {
                UIinteractionText.text = "make a wish";
                UICrosshair.SetActive(false);
            } else if (hit.collider.CompareTag("Refuel"))
            {
                UIinteractionText.text = "refuel Rose";
                UICrosshair.SetActive(false);
            } else {
                UICrosshair.SetActive(true);
                UIinteractionText.text = "";
            }
        } else {
            UICrosshair.SetActive(true);
            UIinteractionText.text = "";
        }
    }

    void SpawnEnemiesWave0()
    {
        // Instancie o GameObject na posição do spawnPoint
        Instantiate(demonPrefabs[0], demonSpawnPoint1.position, demonSpawnPoint1.rotation);
        wave0DemonCounter++;
        Instantiate(demonPrefabs[0], demonSpawnPoint2.position, demonSpawnPoint2.rotation);
        wave0DemonCounter++;
        Instantiate(demonPrefabs[0], demonSpawnPoint3.position, demonSpawnPoint3.rotation);
        wave0DemonCounter++;
    }

    void SpawnEnemiesWave1()
    {
        // Instancie o GameObject na posição do spawnPoint
        Instantiate(demonPrefabs[0], demonSpawnPoint1.position, demonSpawnPoint1.rotation);
        wave1DemonCounter++;
        Instantiate(demonPrefabs[1], demonSpawnPoint2.position, demonSpawnPoint2.rotation);
        wave1DemonCounter++;
        Instantiate(demonPrefabs[0], demonSpawnPoint3.position, demonSpawnPoint3.rotation);
        wave1DemonCounter++;
    }

    public void CountKill()
    {
        killCounter += 1;

        if (bloodCounter >= 100)
        {
            int objectsToDestroy = 5; // Número de objetos a serem destruídos

            for (int i = 0; i < objectsToDestroy; i++)
            {
                if (bloodList.Count > 0)
                {
                    GameObject bloodToDestroy = bloodList[Random.Range(0, bloodList.Count)];
                    Destroy(bloodToDestroy);
                    bloodList.Remove(bloodToDestroy); // Remove o objeto da lista após destruí-lo
                }
                else
                {
                    break; // Sai do loop se a lista estiver vazia
                }
            }
        }

        if(killCounter >= 50)
        {
            GameObject destroyDead = deadEnemies[Random.Range(0, deadEnemies.Count)];
            Destroy(destroyDead);
            deadEnemies.Remove(destroyDead);
        }

        if (killCounter >= wave0DemonLimit){
            wave = -1;
            GameObject[] fountains = GameObject.FindGameObjectsWithTag("Fountain");
            fountainPiece.tag = "WishFountain";
            for (int i = 0; i < fountains.Length; i++)
            {
                fountains[i].GetComponent<ParticleSystem>().Play();
            }
        } else if (killCounter >= wave1DemonLimit){
            wave = -2;
        }
    }
}
