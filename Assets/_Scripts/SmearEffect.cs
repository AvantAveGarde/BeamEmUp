using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Script from https://github.com/cjacobwade/HelpfulScripts/blob/master/SmearEffect/SmearEffect.cs
[ExecuteInEditMode]
public class SmearEffect : MonoBehaviour
{
    Queue<Vector3> _recentPositions = new Queue<Vector3>();

    [SerializeField]
    private int _frameLag = 0;

    [SerializeField]
    private int smearMaterialIndex = 1;

    Material _smearMat = null;
    public Material smearMat
    {
        get
        {
            if (!_smearMat)
            {
                _smearMat = this.GetComponent<Renderer>().materials[smearMaterialIndex];
            }

            if (!_smearMat.HasProperty("_PrevPosition"))
            {
                _smearMat.shader = Shader.Find("Custom/ToonSmear");
            }

            return _smearMat;
        }
    }

    public int FrameLag
    {
        get
        {
            return _frameLag;
        }
        set
        {
            _frameLag = value;
        }
    }

    private void Update()
    {
        if (_frameLag == 0)
        {
            this._recentPositions.Clear();
            smearMat.SetVector("_PrevPosition", gameObject.transform.position);
        }
    }

    void LateUpdate()
    {
        if (_recentPositions.Count > _frameLag)
        {
            smearMat.SetVector("_PrevPosition", _recentPositions.Dequeue());
        }

        smearMat.SetVector("_Position", transform.position);
        _recentPositions.Enqueue(transform.position);
    }
}