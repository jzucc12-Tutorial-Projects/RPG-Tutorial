using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delay Click Targeting", menuName = "Abilities/Targeting/Delay Click", order = 0)]
    public class DelayClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture = null;
        [SerializeField] Vector2 cursorHotspot = Vector2.zero;
        [SerializeField] float targetRadius = 1f;
        [SerializeField] LayerMask layerMask;
        [SerializeField] Transform targetingPrefab = null;
        Transform targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            var controller = data.GetUser().GetComponent<PlayerController>();
            controller.StartCoroutine(Targeting(data, controller, finished));
        }

        IEnumerator Targeting(AbilityData data, PlayerController controller, Action finished)
        {
            controller.enabled = false;
            
            if(targetingPrefabInstance == null)
                targetingPrefabInstance = Instantiate(targetingPrefab);
            else
                targetingPrefabInstance.gameObject.SetActive(true);

            targetingPrefabInstance.localScale = new Vector3(targetRadius * 2, 1, targetRadius * 2);

            while(!data.IsCancelled())
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit raycastHit;
                if(Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.transform.position = raycastHit.point;
                    if(Input.GetMouseButtonDown(0))
                    {
                        yield return new WaitWhile(() => Input.GetMouseButton(0));
                        data.SetPoint(raycastHit.point);
                        data.SetTargets(GetGameObjectsInRadius(raycastHit.point));
                        break;
                    }
                }
                yield return null;
            }
            controller.enabled = true;
            targetingPrefabInstance.gameObject.SetActive(false);
            finished?.Invoke();
        }

        IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, targetRadius, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}