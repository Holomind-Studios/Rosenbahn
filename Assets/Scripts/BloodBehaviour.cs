using UnityEngine;
using System.Collections.Generic;

public class BloodBehaviour : MonoBehaviour
{
    public GameObject[] bloodOBJ;

    private GameObject bloodContainer;
    private List<GameObject> bloodList = new List<GameObject>();
    private int bloodCounter = 0;
    private GameObject gameController;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        bloodContainer = GameObject.FindGameObjectWithTag("BloodContainer");
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Platform"))
        {
            ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
            int numCollisionEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 collisionPoint = collisionEvents[i].intersection;
                Quaternion collisionRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));

                if (bloodOBJ.Length > 0)
                {
                    int randomIndex = Random.Range(0, bloodOBJ.Length);

                    // Instancia a prefab
                    GameObject bloodInstancePrefab = bloodOBJ[randomIndex];

                    // Instancia um novo objeto para armazenar a instância da prefab
                    GameObject bloodInstance = Instantiate(bloodInstancePrefab, collisionPoint - (new Vector3(0, 0.25f, 0)), collisionRotation);
                    bloodInstance.transform.parent = bloodContainer.transform;

                    gameController.GetComponent<GameController>().bloodCounter = gameController.GetComponent<GameController>().bloodCounter + 1;
                    gameController.GetComponent<GameController>().bloodList.Add(bloodInstance);
                }
            }
        }
    }
}
