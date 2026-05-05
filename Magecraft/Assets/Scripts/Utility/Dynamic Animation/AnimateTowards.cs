using System;
using UnityEngine;

public class AnimateTowards : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private PositionMode positionMode = PositionMode.Local;
    [SerializeField, Tooltip("Resets position to it's original position when 'Animate' is called if true.")]
    private bool resetOnAnimate = true;

    public Action OnFinishedAnimating;
    public Action OnAnimationInterupted;

    private float duration;
    private float moveTime;
    private Vector3 moveStart;
    private Vector3 moveTarget;
    private bool isMoving;
    private bool invertAnim;

    private Vector3 savedPos = Vector3.zero;

    private void Awake()
    {
        savedPos = positionMode switch
        {
            PositionMode.Local => transform.localPosition,
            PositionMode.World => transform.position,
            _ => moveStart
        };
    }

    private void Update()
    {
        if (!isMoving) return;

        moveTime += Time.deltaTime;
        float t = Mathf.Clamp01(moveTime / duration);

        float sampleT = invertAnim ? 1f - t : t;
        float curvedT = animationCurve.Evaluate(sampleT);

        Vector3 target = invertAnim ? moveStart : moveTarget;
        Vector3 start = invertAnim ? moveTarget : moveStart;

        Vector3 pos = Vector3.LerpUnclamped(start, target, curvedT);
        switch (positionMode)
        {
            case PositionMode.Local:
                transform.localPosition = pos;
                break;

            case PositionMode.World:
                transform.position = pos;
                break;
        }

        if (t >= 1f)
        {
            isMoving = false;
            OnFinishedAnimating?.Invoke();
        }
    }

    public void Animate(Vector3 target, float animationDuration = 1f, bool invertAnim = false)
    {
        if (resetOnAnimate) Reset();
        this.invertAnim = invertAnim;

        duration = Mathf.Max(0.0001f, animationDuration);

        moveStart = positionMode switch
        {
            PositionMode.Local => transform.localPosition,
            PositionMode.World => transform.position,
            _ => moveStart
        };

        moveTarget = target;
        moveTime = 0f;
        isMoving = true;
    }

    public void Cancel()
    {
        isMoving = false;
        OnAnimationInterupted?.Invoke();
        OnFinishedAnimating?.Invoke();
    }

    public void Reset()
    {
        switch (positionMode)
        {
            case PositionMode.Local:
                transform.localPosition = savedPos;
                break;

            case PositionMode.World:
                transform.position = savedPos;
                break;
        }
    }
}
