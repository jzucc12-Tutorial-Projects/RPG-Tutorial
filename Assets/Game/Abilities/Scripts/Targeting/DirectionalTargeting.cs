using System;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "Abilities/Targeting/Directional Targetting", order = 0)]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] float groundOffSet = 1;


        public override void StartTargeting(AbilityData data, Action finished)
        {
            RaycastHit raycastHit;
            Ray ray = PlayerController.GetMouseRay();
            if(Physics.Raycast(ray, out raycastHit, 1000, layerMask))
            {
                data.SetPoint(raycastHit.point + ray.direction * groundOffSet / ray.direction.y);
            }

            finished?.Invoke();
        }
    }
}