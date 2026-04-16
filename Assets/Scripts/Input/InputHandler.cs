using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private HoldButton jumpButton;
    [SerializeField] private HoldButton slideButton;

    void Awake()
    {
        jumpButton.onPress.AddListener(PressJump);
        jumpButton.onRelease.AddListener(PressJumpEnd);

        slideButton.onPress.AddListener(PressSlideStart);
        slideButton.onRelease.AddListener(PressSlideEnd);
    }

    void Update()
    {
        // 키보드 입력 (테스트용)
        if (Input.GetKeyDown(KeyCode.Space))
            playerController.OnJump();

        if (Input.GetKeyUp(KeyCode.Space))
            playerController.OnJumpEnd();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            playerController.OnSlideStart();

        if (Input.GetKey(KeyCode.DownArrow))
            playerController.OnSlideStart();

        if (Input.GetKeyUp(KeyCode.DownArrow))
            playerController.OnSlideEnd();
    }

    // 버튼에서 호출 (OnPointerDown / OnPointerUp 이벤트에 연결)
    public void PressJump() => playerController.OnJump();
    public void PressJumpEnd() => playerController.OnJumpEnd();
    public void PressSlideStart() => playerController.OnSlideStart();
    public void PressSlideEnd() => playerController.OnSlideEnd();
}