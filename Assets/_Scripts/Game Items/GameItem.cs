using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.MultiAudioListener;

public class GameItem : MonoBehaviour
{
    public LayerMask layerMask;
    public MultiAudioSource audioSource;
    public ItemData itemData;
    public float itemHoverHeight;
    public bool isBeingZapped;

    public Material material;
    public Material dissolveMat;

    private Vector3 originalPosition;
    private bool hasBeenZapped = false;

    public bool isBoss;
    private int bossHits = 7;

    [HideInInspector]
    public GameObject playerZapping;

    private GameObject go;

    private MeshRenderer childMeshRenderer;

    public bool isDissolving = false;

    public void Start()
    {
        if (this.CompareTag("Item"))
        {
            if (itemData != null)
            {
                go = Instantiate<GameObject>(itemData.newGameObject, transform);
            }
            originalPosition = transform.position;
        }
    }

    public void Update()
    {
        if (this.CompareTag("Item"))
        {
            //item rotation
            go.transform.Rotate(0, 10 * Time.deltaTime, 0);

            //item float
            transform.Translate(0, Mathf.Sin(Time.fixedTime) / (100 / itemHoverHeight), 0);

            if (!IsGrounded())
            {
                transform.position += Vector3.down * Time.deltaTime * 3;
                if (transform.position.y < -20)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (isBeingZapped)
        {
            //if (!isDissolving)
            //{
            //    Transform propObj = transform.GetChild(0);
            //    childMeshRenderer = propObj.GetComponent<MeshRenderer>();
            //    Material[] materials = new Material[childMeshRenderer.materials.Length];
            //    for (int i = 0; i < childMeshRenderer.materials.Length; i++)
            //    {
            //        materials[i] = dissolveMat;
            //    }
            //    childMeshRenderer.materials = materials;

            //    StartCoroutine(Dissolve());
            //}
            //Transform propObj = transform.GetChild(0);
            //    MeshRenderer meshRenderer = propObj.GetComponent<MeshRenderer>();

            //Debug.Log(meshRenderer.material.name);
            //Color oldColor = meshRenderer.materials[0].GetColor("_Color");

            //meshRenderer.materials = new Material[] { dissolveMat };
            //meshRenderer.materials[0].shader = Shader.Find("MoonflowerCarnivore/Dissolve Edge");
            //meshRenderer.materials[0].SetFloat("_Progress", 0.5f);

            //meshRenderer.materials[0].SetColor("_TintColor", oldColor);
        }

        //if (isBeingZapped)
        //{
        //    if (playerZapping != null)
        //    {
        //        this.GetComponent<SmearEffect>().FrameLag = 30;

        //        // Check distance
        //        Vector3 gunPos = playerZapping.GetComponent<Zapper>().gun.transform.position;
        //        float distanceToGun = (transform.position - gunPos).magnitude;
        //        // Don't move if too close
        //        if (distanceToGun < 5.0f)
        //        {
        //            return;
        //        }


        //        transform.position = Vector3.Slerp(transform.position, gunPos, Time.time * 0.001f);
        //    }
        //}
        //else
        //{
        //    this.GetComponent<SmearEffect>().FrameLag = 0;
        //    if (transform.position != originalPosition)
        //        transform.position = Vector3.Slerp(transform.position, originalPosition, Time.time * 0.001f);
        //}
    }

    private IEnumerator Dissolve()
    {
        isDissolving = true;
        for (float f = 0; f <= 1f; f += 0.1f)
        {
            float lerpAmount = Mathf.Lerp(1, 0, f);
            for (int i = 0; i < childMeshRenderer.materials.Length; i++)
            {
                childMeshRenderer.materials[i].SetFloat("_Progress", lerpAmount);
            }

            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine(DestroyItem());
        //foreach (Material mat in childMeshRenderer.materials)
        //    mat.SetFloat("_Progress", 1.0f);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 3f);
    }

    public void ZapItem()
    {
        if (audioSource != null)
            audioSource.Play();

        if (this.CompareTag("Item"))
        {
            Transform propObj = transform.GetChild(0);
            childMeshRenderer = propObj.GetComponent<MeshRenderer>();
            Material[] materials = new Material[childMeshRenderer.materials.Length];
            for (int i = 0; i < childMeshRenderer.materials.Length; i++)
            {
                materials[i] = dissolveMat;
            }
            childMeshRenderer.materials = materials;

            //player.StopVibration();
            StartCoroutine(Dissolve());
        } else
        {
            if (isBoss)
            {
                bossHits--;
                if (bossHits <= 0)
                    StartCoroutine(DestroyItem());
                
            }
            else
            {
                StartCoroutine(DestroyItem());
            }
        }
    }

    public IEnumerator DestroyItem()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.AudioClip.length / 2);
        Destroy(gameObject);
        
    }
}
