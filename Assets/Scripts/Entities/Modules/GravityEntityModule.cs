using System;
using Unity.Mathematics;
using UnityEngine;

namespace Refactor.Entities.Modules
{
    [Serializable]
    public class GravityEntityModule : EntityModule
    {
        public const float EDGE_DETECTION_ANGLE = 5f;
        
        [Header("SETTINGS")]
        public float gravityForce = 20f;
        public float finalVelocity = -30f;
        public float slideFriction = 0.3f;
        public float maxGroundDistance = 3f;
        public LayerMask groundLayerMask;
        
        [Header("STATE")] 
        public GameObject groundObject;
        public Vector3 groundNormal;
        public float lastGroundY;

        public override void UpdatePhysics(float deltaTime)
        {
            if (entity.isGrounded)
            {
                entity.velocity.y = math.max(-1f, entity.velocity.y);
            }
            else
            {
                entity.velocity.y = math.max(entity.velocity.y - gravityForce * deltaTime, finalVelocity);
            }
            
            #region Slide Off Edges and Steep Surfaces
            var h = entity.controller.height;
            var r = entity.controller.radius;
            var center = entity.transform.position + new Vector3(0, r + 0.1f, 0); /*- new Vector3(0, h/2f - r, 0;*/
            Debug.DrawRay(center, Vector3.down, Color.magenta, 0.02f);
            if (Physics.SphereCast(center, r, Vector3.down, out var hit, maxGroundDistance, groundLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                if (hit.distance > entity.controller.skinWidth + 0.01f)
                {
                    return;
                }

                entity.isGrounded = true;
                groundNormal = hit.normal;
                groundObject = hit.collider.gameObject;

                Physics.Raycast(center, hit.point - center, out var groundHit, 3f, groundLayerMask,
                    QueryTriggerInteraction.Ignore);

                var groundAngle = Vector3.Angle(Vector3.up, groundHit.normal);
                var delta = (hit.point - center).normalized;
                var angle = Vector3.Angle(Vector3.down, delta);

#if UNITY_EDITOR
                Debug.DrawLine(center, groundHit.point, Color.yellow, deltaTime);
                Debug.DrawRay(groundHit.point, groundHit.normal * 0.25f, Color.red, deltaTime);
#endif

                if (Utils.OutInterval(groundAngle, 10f, entity.controller.slopeLimit) && angle > EDGE_DETECTION_ANGLE)
                {
                    if (lastGroundY > hit.point.y - 0.1 ||
                        hit.point.y - lastGroundY > entity.controller.stepOffset + 0.05f)
                    {
                        var hitNormal = -delta;
                        hitNormal.y = -1f;

                        var mv = new Vector3
                        {
                            x = (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction) * deltaTime,
                            z = (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction) * deltaTime,
                            y = -1f * deltaTime
                        };

                        var soc = entity.controller.stepOffset;
                        entity.controller.stepOffset = 0;
                        entity.controller.Move(mv);
                        entity.controller.stepOffset = soc;
                    }
                }
                else
                {
                    lastGroundY = hit.point.y;
                }
            }
            else
                groundObject = null;

            #endregion
        }
    }
}