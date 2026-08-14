using UnityEditor;
using UnityEngine;

public class ShowCollider
{
    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawWeaponGizmo(Transform target, GizmoType gizmoType)
    {
        //exit if not a weapon
        if (!target.CompareTag("Weapon"))
        {
            return;
        }

        //grab the collider
        BoxCollider collider = target.gameObject.GetComponent<BoxCollider>();
        if (collider.enabled)
        {
            //transform gizmo matrix to match and save a backup
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = target.localToWorldMatrix;

            //draw the gizmo
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(collider.center, collider.size);

            //reset the matrix
            Gizmos.matrix = originalMatrix;
        }
    }
}
