using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class ProceduralRope : MonoBehaviour
    {
        [SerializeField] private float jointMass = 0.2f;
        [SerializeField] private int jointCount = 8;
        [SerializeField] private bool shouldHaveCollider = true;
        [SerializeField] private float jointWidth = 0.3f;
        [SerializeField] private PhysicsMaterial2D jointPhysicsMaterial;

        private LineRenderer lineRenderer;
        private PlayerController playerController;
        private Joint2D rootJoint;
        private float distancePerSegment;
        private List<Joint2D> joints = new List<Joint2D>();

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            lineRenderer = GetComponent<LineRenderer>();
            rootJoint = GetComponent<DistanceJoint2D>();
        }

        private void Start()
        {
            distancePerSegment = (jointCount - 2) / playerController.PayloadDistance;
            GenerateJoints();
        }

        private void Update()
        {
            if (playerController.IsReleased)
            {
                lineRenderer.positionCount = 0;
            }
            else
            {
                lineRenderer.positionCount = joints.Count + 2;

                Vector3[] positions = joints
                    .Select(h => h.transform.position)
                    .Prepend(transform.position)
                    .Append(playerController.PayloadBody.position)
                    .ToArray();
                
                lineRenderer.SetPositions(positions);
            }
        }

        public void GenerateJoints()
        {
            Vector2 direction = playerController.PayloadVector.normalized;
            Joint2D lastJoint = rootJoint;

            for (int i = 0; i < jointCount; i++)
            {
                float segmentDistance = distancePerSegment * i;
                Vector2 segmentPosition = (Vector2) transform.position + (direction * segmentDistance);
                Vector2 offset = -direction * distancePerSegment / 2f;

                GameObject jointObject = new GameObject($"Rope Joint - Segment {i}");
                jointObject.transform.SetParent(transform.parent);
                jointObject.transform.position = segmentPosition + offset;
                
                var jointBody = jointObject.AddComponent<Rigidbody2D>();
                jointBody.mass = jointMass;
                jointBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                jointBody.interpolation = RigidbodyInterpolation2D.Interpolate;
                jointBody.sharedMaterial = jointPhysicsMaterial;
                ApplyCollider(jointBody, direction);

                var joint = jointObject.AddComponent<DistanceJoint2D>();
                lastJoint.connectedBody = jointBody;
                joint.breakForce = 1200;
                joint.anchor = -offset;
                joint.connectedAnchor = offset;
                SetSegmentJointDistance(lastJoint);
                
                lastJoint = joint;
                joints.Add(joint);
            }
            
            lastJoint.connectedBody = playerController.PayloadBody;
            playerController.PayloadBody.sharedMaterial = jointPhysicsMaterial;
            //ApplyCollider(lastJoint.gameObject, direction);
            SetSegmentJointDistance(lastJoint);
        }

        public void ClearJoints()
        {
            foreach (Joint2D joint in joints)
            {
                Destroy(joint.gameObject);
            }
            
            rootJoint.connectedBody = null;
            joints.Clear();
        }

        private void ApplyCollider(Rigidbody2D body, Vector2 direction)
        {
            if (!shouldHaveCollider) return;

            GameObject go = body.gameObject;
            go.layer = 7;
            var jointCollider = go.AddComponent<BoxCollider2D>();
            // Vector2 colliderCenter = go.transform
            //     .InverseTransformVector(-direction * distancePerSegment / 2f);
            // jointCollider.offset = colliderCenter;
            jointCollider.size = new Vector2(distancePerSegment * 2, jointWidth);
            //body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void SetSegmentJointDistance(Joint2D joint)
        {
            // Bad game jam code
            if (joint is DistanceJoint2D distanceJoint)
            {
                distanceJoint.autoConfigureDistance = false;
                distanceJoint.distance = distancePerSegment;
            }
        }
    }
}