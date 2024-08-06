using UnityEngine;

namespace ET
{
    [ChildOf(typeof(LSBulletViewComponent))]
    public class LSBulletView: Entity, IAwake<GameObject>, IUpdate, ILSRollback
    {
        public GameObject GameObject { get; set; }
        public Transform Transform { get; set; }
        public EntityRef<LSBullet> Unit;
        public Vector3 Position;
        public Quaternion Rotation;
        public float totalTime;
        public float t;
    }
}