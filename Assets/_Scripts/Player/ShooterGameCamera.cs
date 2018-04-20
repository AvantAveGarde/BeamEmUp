// Camera code is from this Project:
// http://unity3d.com/support/resources/example-projects/3rdpersonshooter
// Thank you guys.

// Add this script to the main camera.
// In the inspector, there is a variable "player" - drag your player on it.
// Press Play!

using UnityEngine;
using System.Collections;
using Rewired;

// 3rd person game-like camera controller
// keeps camera behind the player and aimed at aiming point
public class ShooterGameCamera : MonoBehaviour
{

    public Transform target;
    public int playerID = 0;

    public Texture crosshair; // crosshair - removed it for quick and easy setup. ben0bi
    // if you add the crosshair, you need to drag a crosshair texture on the "crosshair" variable in the inspector 

    [HideInInspector]
    public Transform aimTarget; // that was public and a gameobject had to be dragged on it. - ben0bi

    [HideInInspector]
    public Transform gunTarget;

    public float smoothingTime = 10.0f; // it should follow it faster by jumping (y-axis) (previous: 0.1 or so) ben0bi
    public Vector3 pivotOffset = new Vector3(0.2f, 0.7f, 0.0f); // offset of point from player transform (?) ben0bi
    public Vector3 camOffset = new Vector3(0.0f, 0.7f, -3.4f); // offset of camera from pivotOffset (?) ben0bi
    public Vector3 closeOffset = new Vector3(0.35f, 1.7f, 0.0f); // close offset of camera from pivotOffset (?) ben0bi

    public float horizontalAimingSpeed = 800f; // was way to lame for me (270) ben0bi
    public float verticalAimingSpeed = 800f;   // --"-- (270) ben0bi
    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -80f;

    public float mouseSensitivity = 0.3f;
    
    public float angleH = 0;
    private float angleV = 0;
    private Transform camTransfrom;
    private float maxCamDist = 1;
    private LayerMask mask;
    private Vector3 smoothPlayerPos;

    private GameManager gm;
    public GameData gameData;

    private Player player;

    private Camera cam;

    public void Awake()
    {
        player = ReInput.players.GetPlayer(playerID);

        // [edit] no aimtarget gameobject needs to be placed anymore - ben0bi
        GameObject g = new GameObject("aimTarget");
        g.transform.SetParent(transform);
        aimTarget = g.transform;
        camTransfrom = transform;
        cam = camTransfrom.GetComponent<Camera>();
        maxCamDist = 3;

        GameObject f = new GameObject("gunTarget");
        f.transform.SetParent(transform);
        gunTarget = f.transform;
    }

    // Use this for initialization
    void Start()
    {
        gm = GameManager.instance;
        
        if (gm == null)
        {
            if (target == null)
            {
                Debug.LogError("Camera " + playerID + " is missing a target.");
                return;
            }
            else
            {
                SetTarget(target);
            }
        }
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        if (Time.deltaTime == 0 || Time.timeScale == 0 || target == null)
            return;

        angleH += Mathf.Clamp(player.GetAxis("LookHorizontal"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;

        if (gameData.playerYAxisInverted[playerID])
            angleV += Mathf.Clamp(-1 * player.GetAxis("LookVertical"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
        else
            angleV += Mathf.Clamp(player.GetAxis("LookVertical"), -1, 1) * verticalAimingSpeed * Time.deltaTime;

        // limit vertical angle
        angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);

        // Before changing camera, store the prev aiming distance.
        // If we're aiming at nothing (the sky), we'll keep this distance.
        float prevDist = (aimTarget.position - camTransfrom.position).magnitude;

        // Set aim rotation
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        camTransfrom.rotation = aimRotation;

        // Find far and close position for the camera
        smoothPlayerPos = Vector3.Lerp(smoothPlayerPos, target.position, smoothingTime * Time.deltaTime);
        smoothPlayerPos.x = target.position.x;
        smoothPlayerPos.z = target.position.z;
        Vector3 farCamPoint = smoothPlayerPos + camYRotation * pivotOffset + aimRotation * camOffset;
        Vector3 closeCamPoint = target.position + camYRotation * closeOffset;
        float farDist = Vector3.Distance(farCamPoint, closeCamPoint);

        // Smoothly increase maxCamDist up to the distance of farDist
        maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5 * Time.deltaTime);

        // Make sure camera doesn't intersect geometry
        // Move camera towards closeOffset if ray back towards camera position intersects something 
        RaycastHit hit;
        Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
        float padding = 0.3f;
        if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask))
        {
            maxCamDist = hit.distance - padding;
        }
        camTransfrom.position = closeCamPoint + closeToFarDir * maxCamDist;

        // Do a raycast from the camera to find the distance to the point we're aiming at.
        float aimTargetDist;
        if (Physics.Raycast(camTransfrom.position, camTransfrom.forward, out hit, 100, mask))
        {
            aimTargetDist = hit.distance + 0.05f;
        }
        else
        {
            // If we're aiming at nothing, keep prev dist but make it at least 5.
            aimTargetDist = Mathf.Max(5, prevDist);
        }

        // Set the aimTarget position according to the distance we found.
        // Make the movement slightly smooth.
        float gunTargetDist = 30.0f;
        aimTarget.position = camTransfrom.position + camTransfrom.forward * aimTargetDist;
        gunTarget.position = camTransfrom.position + camTransfrom.forward * gunTargetDist;
        
        AimModel();
    }

    private void AimModel()
    {
        Transform moveJoint = target.Find("AlienPlayer/Root_M/Spine1_M/Spine2_M/Chest_M");
        Transform gunJoint = target.Find("AlienPlayer/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/ShoulderPart1_R/ShoulderPart2_R/Elbow_R/Wrist_R/Wrist_PROP");

        //moveJoint.LookAt(aimTarget);
        moveJoint.LookAt(gunTarget);
        moveJoint.rotation *= Quaternion.Euler(0, -90, -90);

        //gunJoint.LookAt(aimTarget);
        gunJoint.LookAt(gunTarget);
        gunJoint.rotation *= Quaternion.Euler(90, 90, 0);

    }


    // so you can change the camera from a static observer (level loading) or something else
    // to your player or something else. I needed that for network init... ben0bi
    public void SetTarget(Transform t)
    {
        target = t;

        // Add player's own layer to mask
        mask = 1 << target.gameObject.layer;
        // Add Ignore Raycast layer to mask
        mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
        // Invert mask
        mask = ~mask;

        smoothPlayerPos = target.position;
    }


    void OnGUI()
    {
        if (crosshair != null && cam != null && target != null && cam.enabled == true)
        {
            if (Time.time != 0 && Time.timeScale != 0)
            {

                //float width = (Screen.width * 0.5f) * (cam.rect.x + cam.rect.width / 2);
                //float height = (Screen.width * 0.5f) * (cam.rect.y + cam.rect.height / 2);
                float leftSide = Screen.width * cam.rect.x;
                float width = leftSide + (cam.rect.width / 2) * Screen.width;

                //float width = 0.25f * Screen.width;// * cam.rect.x;
                //float width = 0.5f * Screen.width + 0.5f * cam.rect.x + 0.5f * cam.rect.width;
                float bottom = Screen.height * cam.rect.y;
                float height = bottom - (cam.rect.height / 2) * Screen.height;
                if (bottom == 0)
                    height = Screen.height - (cam.rect.height / 2) * Screen.height;

                //float height = 0.5f * Screen.height + 0.5f * cam.rect.y + 0.5f * cam.rect.height;
                //Debug.Log("playerid: " + playerID + "    rect.y: " + cam.rect.y + "    rect.height: " + cam.rect.height);
                //GUI.DrawTexture(new Rect(width - (crosshair.width * 0.5f), height - (crosshair.height * 0.5f), crosshair.width, crosshair.height), crosshair);
                GUI.DrawTexture(new Rect(width - (crosshair.width * 0.5f), height - (crosshair.height * 0.5f), crosshair.width, crosshair.height), crosshair);

                //GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crosshair.width * 0.5f), Screen.height * 0.5f - (crosshair.height * 0.5f), crosshair.width, crosshair.height), crosshair);
            }
        }
    }

}