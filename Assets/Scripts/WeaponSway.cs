using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public GameObject weaponMesh;
    public GameObject impactEffect;
    public ParticleSystem muzzleFlash;
    public float swayAmount = 0.02f; // Intensidade do sway
    public float maxSwayAmount = 0.06f; // Máximo que a arma pode se mover
    public float swaySmoothAmount = 4f; // Suavização do sway
    public Vector2 swayDirection = new Vector2(1f, 1f); // Direção do sway

    public float walkAmount = 0.03f; // Intensidade do movimento de caminhar para cima e para baixo
    private float walkSmoothAmount = 6f; // Suavização do movimento de caminhar

    public float recoilAmount = 0.1f; // Intensidade do recuo do tiro
    public float recoilSmoothAmount = 2f; // Suavização do recuo do tiro

    private Vector3 initialPosition; // Posição inicial da arma
    private Vector3 targetPosition; // Posição alvo da arma
    private Camera mainCamera; // Referência para a câmera principal
    private Animator weaponAnimator;

    void Start()
    {
        weaponAnimator = weaponMesh.GetComponent<Animator>();
        initialPosition = transform.localPosition;
        mainCamera = Camera.main; // Obtemos a referência para a câmera principal no início
    }

    void Update()
    {
        // Obtenha os movimentos de input da câmera
        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        // Obtenha os movimentos de input do jogador (horizontal e vertical)
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        // Limite o movimento para evitar exageros
        moveX = Mathf.Clamp(moveX, -maxSwayAmount, maxSwayAmount);
        moveY = Mathf.Clamp(moveY, -maxSwayAmount, maxSwayAmount);

        // Calcule a posição alvo da arma com base nos movimentos da câmera
        targetPosition = new Vector3(moveX * swayDirection.x, moveY * swayDirection.y, 0f);

        // Adicione movimento de caminhar apenas se o jogador estiver se movendo
        if (Mathf.Abs(horizontalMove) > 0.1f || Mathf.Abs(verticalMove) > 0.1f)
        {
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f; // Verifica se a tecla LeftShift está pressionada
            float walkOffset = Mathf.Sin(Time.time * (walkSmoothAmount * speedMultiplier)) * walkAmount;
            targetPosition += new Vector3(0f, walkOffset, 0f);
        }

        // Adicione recuo do tiro quando o jogador pressionar o botão do mouse 0
        if (Input.GetMouseButtonDown(0) && !IsShootingAnimationPlaying("revolver_shot") && !IsShootingAnimationPlaying("revolver_trigger"))
        {
            weaponAnimator.SetTrigger("IsTrigger");
        } else if (Input.GetMouseButtonUp(0) && !IsShootingAnimationPlaying("revolver_shot") && IsShootingAnimationPlaying("revolver_trigger")) {
            Shoot();
            weaponAnimator.SetTrigger("IsShot");
        }

        // Suavize o movimento
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + initialPosition, Time.deltaTime * swaySmoothAmount);
    }

    void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100f))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.tag == "Enemy"){
                hit.transform.GetComponent<EnemyController>().TakeHit(Random.Range(11, 15));
            }
            
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hit.normal));
            Destroy(impactGO, 2f);
        }
    }

    bool IsShootingAnimationPlaying(string animationName)
    {
        // Verifica se uma animação está sendo reproduzida
        return weaponAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
}
