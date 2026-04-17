using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float normalY = 0f;
    [SerializeField] private float slideY = -0.2f;
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private Transform deadEyePoint;


    private static readonly Vector3 runRotation   = new(  0f, 0f, 0f);
    private static readonly Vector3 jumpRotation  = new( 16f, 0f, 0f);
    private static readonly Vector3 slideRotation = new(-32f, 0f, 0f);

    private float targetY;
    private Vector3 targetRotation;
    private float bobTimer;
    private bool isBobbing;
    private Vector3 initPosition;
    private Quaternion initRotation;

    void Start()
    {
        targetY = normalY;
        targetRotation = runRotation;
        initPosition = transform.position;
        initRotation = transform.rotation;
    }

    void Update()
    {
        if (isBobbing)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            targetY = normalY + Mathf.Sin(bobTimer) * bobAmplitude;
        }

        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * transitionSpeed);

        //Quaternion rot = Quaternion.Lerp(
        //    transform.localRotation,
        //    Quaternion.Euler(targetRotation),
        //    Time.deltaTime * transitionSpeed
        //);

        transform.SetLocalPositionAndRotation(pos, initRotation);
    }

    public void StartRun()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
        SetRun();
    }

    public void SetRun()
    {
        isBobbing = true;
        targetY = normalY;
        targetRotation = runRotation;
    }

    public void SetSlide()
    {
        isBobbing = false;
        targetY = slideY;
        targetRotation = slideRotation;
    }

    public void SetJump()
    {
        isBobbing = false;
        targetRotation = jumpRotation;
    }

    public void PlayDeadCamera()
    {
        StartCoroutine(DeadCameraCoroutine());
        isBobbing = false;
    }

    private IEnumerator DeadCameraCoroutine()
    {
        float duration = 1f;
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - 0.8f, startPos.z); // 바닥으로 내려감

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(-90f, 0f, 0f); // 하늘 쳐다보기

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 뒤로 넘어지는 느낌 위해 EaseOut 적용
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            transform.localPosition = Vector3.Lerp(startPos, endPos, easedT);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, easedT);
            yield return null;
        }
    }
}
