using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteractionHandler : MonoBehaviour
{
    [Header("[ Raycast Info ]")]
    [SerializeField] private float raycastRange; // Raycast 거리
    [SerializeField] private RaycastHit? hit = null;
    [SerializeField] private IInteractive curInteractObject = null;
    [SerializeField] private FristPersonController playerController = null;
    private void Awake()
    {
        raycastRange = 3f;
        playerController = GetComponent<FristPersonController>();
    }
    void Update()
    {
        if (!playerController.isUpdate)
            return;

        PerformRaycast();
    }

    private void PerformRaycast()
    {
        Camera mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hitInfo;
        IInteractive interactObject = null;

        if (Physics.Raycast(ray, out hitInfo, raycastRange))
        {
            hit = hitInfo;
            hitInfo.transform.TryGetComponent<IInteractive>(out interactObject);

            if (interactObject != curInteractObject)
            {
                if (curInteractObject != null)
                {
                    curInteractObject.GetBaseInteractive().OnPointerExit();
                }

                if (interactObject != null)
                {
                    interactObject.GetBaseInteractive().OnPointerEnter();
                }

                curInteractObject = interactObject;
            }

            //Debug.Log($"Hit: {hitInfo.collider.name}");
        }
        else
        {
            if (curInteractObject != null)
            {
                curInteractObject.GetBaseInteractive().OnPointerExit();
                curInteractObject = null;
            }

            hit = null;
        }

        if (null != curInteractObject && null != hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Interactive(curInteractObject);
            }
        }
    }

    private void Interactive(IInteractive interactiveObject)
    {
        var baseInteractive = interactiveObject.GetBaseInteractive();

        // 획득가능 아이템이 우선순위가 먼저.
        IStorageableItem storageableItem = null;
        if (baseInteractive.TryGetComponent<IStorageableItem>(out storageableItem))
        {
            storageableItem.Store();
            return;
        }

        baseInteractive.Interaction();
    }


}
