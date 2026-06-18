using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAttack : InputListener
{
    public UnityEvent OnAttackStart;
    public UnityEvent OnAttackEnd;

    private void Awake()
    {
        AddSubscription(e => e.PrimaryMouse.OnEvent += Attack, e => e.PrimaryMouse.OnEvent -= Attack);
    }

    private void Attack(bool started)
    {
        if (started) OnAttackStart?.Invoke();
        else OnAttackEnd?.Invoke();
    }
}
