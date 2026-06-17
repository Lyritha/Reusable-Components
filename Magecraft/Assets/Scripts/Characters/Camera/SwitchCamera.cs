using System;
using Unity.Cinemachine;
using UnityEngine;

public class SwitchCamera : InputListener
{
    [SerializeField]
    private CinemachineCamera cam1;
    [SerializeField]
    private CinemachineCamera cam2;

    bool isCam1Active = true;

    private void Awake()
    {
        AddSubscription(e => e.Tab.OnEvent += OnTab, e => e.Tab.OnEvent -= OnTab);
    }

    private void OnTab()
    {
        isCam1Active = !isCam1Active;

        cam1.Priority = isCam1Active ? 1 : 0;
        cam2.Priority = isCam1Active ? 0 : 1;

        CinemachineCamera activeCam = isCam1Active ? cam1 : cam2;
    }
}
