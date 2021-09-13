using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class ControlPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;

        private void OnDrawGizmos()
        {
            for(int ii = 0; ii < transform.childCount; ii++)
            {
                int jj = GetNextJJ(ii);
                Gizmos.DrawSphere(GetWaypoint(ii), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(ii), GetWaypoint(jj));
            }
        }

        public int GetNextJJ(int ii)
        {
            if (ii + 1 == transform.childCount)
                return 0;
            else
                return ii + 1;
        }

        public Vector3 GetWaypoint(int ii)
        {
            return transform.GetChild(ii).position;
        }
    }

}