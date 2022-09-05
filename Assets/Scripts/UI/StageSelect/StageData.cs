using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
}

public class StageData : MonoBehaviour
{ 
    bool _isRelease;
    public bool IsRelease => _isRelease;

    [SerializeField] Stage _stage;
    public Stage Stage=>_stage;

    public void Release()
    {
        _isRelease = true;
    }

    public void Lock()
    {
        _isRelease = false;
    }
}
