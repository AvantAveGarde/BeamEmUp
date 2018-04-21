using Rewired;
using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

public class Zapper : MonoBehaviour
{
    public GameData gameData;
    public ParticleSystem shootingParticleSystem;
    public GameObject gun;
    public Animator AnimController;
    private ShooterGameCamera shooterGameCamera;

    [Header("Debug settings")]
    public Physics.PreviewCondition preview = Physics.PreviewCondition.Editor;

    private GameManager gm;
    private Player player;
    private int playerId;

    private GameObject zapTarget;
    private float elapsedZapTime = 0.0f;
    private bool playerIsShooting = false;

    private void Start()
    {
        PlayerController playerController = GetComponent<PlayerController>();

        playerId = playerController.playerID;
        player = ReInput.players.GetPlayer(playerId);
        gm = GameManager.instance;

        if (gm == null)
            shooterGameCamera = playerController.cam.GetComponent<ShooterGameCamera>();
    }

    public void SetShooterGameCamera(ShooterGameCamera cam)
    {
        shooterGameCamera = cam;
    }

    private void Update()
    {
        CheckGunUse();

        if (zapTarget != null)
            CheckZappingProgress();
        else
            CheckNewShoot();

        // Increase or decrease elapsedZapTime based on zapTarget
        CheckZapTime();
    }

    private bool FireRay(float rayDistance, out RaycastHit raycastHit)
    {
        LayerMask ignoreMask = (LayerMask.NameToLayer("Player"));
        RaycastHit hit;


        Debug.DrawRay(gun.transform.position, shooterGameCamera.gunTarget.position - gun.transform.position, Color.green);

        Vector3 startPoint = gun.transform.position;
        Vector3 direction = (shooterGameCamera.gunTarget.position - gun.transform.position).normalized;
        float distance = rayDistance;
        Color hitColor = Color.green;
        Color noHitColor = Color.red;


        // Original Unity physics sphere cast
        //if (Physics.SphereCast(gun.transform.position, 3.0f, 
        //    (shooterGameCamera.gunTarget.position - gun.transform.position).normalized, out hit, rayDistance, ignoreMask))

        // Extended physics sphere cast with visualizations
        if (Physics.SphereCast(startPoint, 3, direction, out hit, distance, ignoreMask, preview, 0, hitColor, noHitColor))
        {
            raycastHit = hit;
            return true;
        }

        raycastHit = new RaycastHit();
        return false;
    }

    private void CheckGunUse()
    {
        if (player.GetButtonDown("Shoot"))
        {
            playerIsShooting = true;
            shootingParticleSystem.Play();
            AnimController.SetBool("Shooting", true);
        }
        else if (player.GetButtonUp("Shoot"))
        {
            playerIsShooting = false;
            shootingParticleSystem.Stop();
            AnimController.SetBool("Shooting", false);
            player.StopVibration();
        }
    }

    private void CheckZapTime()
    {
        // Still zapping
        if (zapTarget != null)
        {
            elapsedZapTime += Time.deltaTime;
            if (elapsedZapTime >= gameData.timeToZap)
            {
                // Successful zap
                GameItem item = zapTarget.GetComponent<GameItem>();
                CancelShooting();
                elapsedZapTime = 0.0f;
                if (gm != null)
                {
                    if (gm.AddItemToPlayer(playerId, item.itemData))
                        item.ZapItem();
                }
                else
                {
                    item.ZapItem();
                }
            }
        }
        // Stopped zapping, slowly decrease zap time
        else
        {
            elapsedZapTime -= Time.deltaTime;
            if (elapsedZapTime < 0)
            {
                elapsedZapTime = 0;
            }
        }
    }

    private void CheckZappingProgress()
    {
        // Make sure trigger is still held down
        if (!playerIsShooting)
        {
            CancelShooting();
            return;
        }

        // Make sure distance to zapTarget is still within zappingDistance
        float distanceToZapTarget = (zapTarget.transform.position - gun.transform.position).magnitude;
        if (distanceToZapTarget > gameData.distanceToZap)
        {
            CancelShooting();
            return;
        }

        // Make sure player is still looking at object and still within zapping distance
        RaycastHit hit;
        if(FireRay(distanceToZapTarget, out hit))
        {
            if (hit.transform.gameObject != zapTarget)
                CancelShooting();
        }
        else
        {
            CancelShooting();
        }
    }

    private void CancelShooting()
    {
        player.StopVibration();
        zapTarget.GetComponent<GameItem>().isBeingZapped = false;
        zapTarget = null;
    }

    private void CheckNewShoot()
    {
        float distance = gameData.distanceToZap;

        if (playerIsShooting)
        {
            RaycastHit hit;
            if (FireRay(distance, out hit))
            {
                GameItem item = hit.transform.gameObject.GetComponent<GameItem>();
                if (item != null)
                {
                    zapTarget = hit.transform.gameObject;
                    zapTarget.GetComponent<GameItem>().isBeingZapped = true;
                    if (zapTarget.GetComponent<GameItem>().playerZapping == null)
                    {
                        zapTarget.GetComponent<GameItem>().playerZapping = gameObject;
                    }
                    player.SetVibration(0, 0.3f);
                }
            }
        }
    }
}
