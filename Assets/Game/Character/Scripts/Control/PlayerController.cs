using UnityEngine;
using RPG.Move;
using System;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health myHealth;
        bool isDragging = false;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }


        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshDistance = 1f;

        private void Start()
        {
            myHealth = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI()) return;

            if (!myHealth.IsAlive())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
                isDragging = false;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                if (Input.GetMouseButtonDown(0))
                    isDragging = true;
                return true;
            }

            if (isDragging) return true;
            return false;
        }

        bool InteractWithComponent()
        {
            RaycastHit[] hits = SortHits();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }    
                }
            }

            return false;
        }

        RaycastHit[] SortHits()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int ii = 0; ii < hits.Length; ii++)
                distances[ii] = hits[ii].distance;

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
            }
            return hasHit;
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit hit;
            target = new Vector3();

            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navHit;
            bool hitNav = NavMesh.SamplePosition(hit.point, out navHit, maxNavMeshDistance, NavMesh.AllAreas);
            if (!hitNav) return false;
            target = navHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                    return mapping;
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
