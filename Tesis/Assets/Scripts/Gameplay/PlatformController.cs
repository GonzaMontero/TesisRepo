using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [System.Serializable]
    class Target
    {
        public Transform transform;
        public Transform oldParent;
        public Vector3 offset;
        public bool onPlatform;

        public Target(Transform newTransform, Vector3 newOffset)
        {
            transform = newTransform;
            offset = newOffset;
            onPlatform = true;
        }
    }

    [Header("Set Values")]
    [SerializeField] LayerMask targetLayers;
    [Header("Runtime Values")]
    [SerializeField] List<Target> targets;
    Dictionary<int, Target> targetIndexes;

    private void Start()
    {
        targetIndexes = new Dictionary<int, Target>();
    }
    void Update()
    {
        //Reset onPlatform checker
        foreach (var obj in targets)
        {
            obj.onPlatform = false;
        }

        //Get the check position, size and rotation
        Vector3 boxPos = transform.position;
        boxPos.y += transform.localScale.y;
        Vector3 boxRadius = transform.localScale * 0.5f;
        Quaternion boxRot = transform.rotation;

        //Check for objects above platform
        Collider[] objectsOnPlatform;
        objectsOnPlatform = Physics.OverlapBox(boxPos, boxRadius, boxRot, targetLayers);

        //Check every obj collided and update or add to list
        Vector3 offset;
        Target target;
        foreach (var obj in objectsOnPlatform)
        {
            offset = obj.transform.position - transform.position;

            //If target was already in platform, update offset, else, add to list
            if (targetIndexes.TryGetValue(obj.transform.GetInstanceID(), out target))
            {
                target.onPlatform = true;
                target.offset = offset;
            }
            else
            {
                //Create target
                target = new Target(obj.transform, offset);

                //Set platform as parent
                target.oldParent = obj.transform.parent;
                obj.transform.parent = transform;

                //Add to list
                targets.Add(target);
                targetIndexes.Add(obj.transform.GetInstanceID(), target);
            }
        }
    }
    void LateUpdate()
    {
        foreach (Target target in targets)
        {
            //If target it's not on platform, remove and skip to next
            if (!target.onPlatform)
            {
                //Set old parent
                target.transform.parent = target.oldParent;

                //Remove from list
                targets.Remove(target);
                targetIndexes.Remove(target.transform.GetInstanceID());
                
                //Skip to next
                continue;
            }
            target.transform.position = transform.position + target.offset;
        }
    }
}
