using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.InputSystem.InputAction;

public class InteractorController : MonoBehaviour
{
    [SerializeField] XRDirectInteractor directInteractor;
    [SerializeField] XRRayInteractor rayInteractor;
    [SerializeField] XRRayInteractor teleportInteractor;

    [SerializeField] List<LocomotionProvider> locomotions;  

    [SerializeField] InputActionReference teleportModeActivate; // InputActionReference : 입력 행위에 대해 하나하나 지정

    private void Start()
    {
        // 같은 키를 누르더라도 상황에 따라 다른 기능이 적용되도록 함
        if (rayInteractor != null)
        {
            rayInteractor.gameObject.SetActive(true);
        }
        if (teleportInteractor != null)
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.selectEntered.AddListener(DisableLocomotions);    // 물건을 잡고있는 경우
            rayInteractor.selectExited.AddListener(EnableLocomotions);      // 잡고있는 상태를 풀 경우
        }

        InputAction teleportModeActivate = this.teleportModeActivate?.action;
        if (teleportModeActivate != null)
        {
            teleportModeActivate.performed += OnStartTeleport;
            teleportModeActivate.canceled += OnStopTeleport;
        }
    }

    // 컴포넌트를 비활성화했을 때
    private void OnDisable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.selectEntered.RemoveListener(DisableLocomotions);
            rayInteractor.selectExited.RemoveListener(EnableLocomotions);
        }

        InputAction teleportModeActivate = this.teleportModeActivate?.action;
        if (teleportModeActivate != null)
        {
            teleportModeActivate.performed -= OnStartTeleport;
            teleportModeActivate.canceled -= OnStopTeleport;
        }
    }

    // 잡는 상태에 진입했을 때
    private void DisableLocomotions(SelectEnterEventArgs args)
    {
        // 모든 locomotion 게임오브젝트들을 비활성화
        foreach (LocomotionProvider locomotion in locomotions)
        {
            locomotion.gameObject.SetActive(false);
        }

        InputAction teleportModeActivate = this.teleportModeActivate?.action;
        if (teleportModeActivate != null)
            teleportModeActivate.Enable();
    }

    private void EnableLocomotions(SelectExitEventArgs args)
    {
        // 모든 locomotion 게임오브젝트들을 사용 가능 하도록 활성화
        foreach (LocomotionProvider locomotion in locomotions)
        {
            locomotion.gameObject.SetActive(true);
        }

        InputAction teleportModeActivate = this.teleportModeActivate?.action;
        if (teleportModeActivate != null)
            teleportModeActivate.Disable();
    }

    private void OnStartTeleport(CallbackContext context)
    {
        if (rayInteractor != null)
            rayInteractor.gameObject.SetActive(false);
        if (teleportInteractor != null)
            teleportInteractor.gameObject.SetActive(true);
    }

    private void OnStopTeleport(CallbackContext context)
    {
        if (rayInteractor != null)
            rayInteractor.gameObject.SetActive(true);
        if (teleportInteractor != null)
            StartCoroutine(TeleportDelay());
    }

    IEnumerator TeleportDelay()
    {
        yield return new WaitForEndOfFrame();   // Delay
        teleportInteractor.gameObject.SetActive(false);
    }
}
