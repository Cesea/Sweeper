using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

//TODO : use pool instead
public class VisualEffectManager : SingletonBase<VisualEffectManager>
{
    public GameObject _exclamationPrefab;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }


    public void SpawnExclamation(BoardObject boardObject)
    {
        GameObject go = Instantiate(_exclamationPrefab, boardObject.transform.position + Vector3.up, Quaternion.identity);
        go.transform.SetParent(transform);

        GameObject.Destroy(go, 1.5f);
    }


    //public void RemoveExclamations()
    //{
    //    if (_exclamations.Count > 0)
    //    {
    //        for (int i = _exclamations.Count - 1; i >= 0; --i)
    //        {
    //            Destroy(_exclamations[i]);
    //        }
    //        _exclamations.Clear();
    //    }
    //}
}
