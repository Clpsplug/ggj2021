using System.Collections.Generic;
using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class Camera2DFollowUpdated : MonoBehaviour
    {
        public Transform target;
        public Rigidbody2D targetRB;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float zoomOutFactor = 5;

        public Rect confinedZone;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        private Camera m_MainCamera;

        [Inject] private IMessageReceiver _messageReceiver;

        private void Awake()
        {
            m_MainCamera = Camera.main;
            _messageReceiver.Receive<GamePlayMessages.PlayerFailureEvent>()
                .Subscribe(x => { enabled = false; }).AddTo(this);
        }

        // Use this for initialization
        private void Start()
        {
            var position = target.position;
            m_LastTargetPosition = position;
            var selfTransform = transform;
            m_OffsetZ = (selfTransform.position - position).z;
            selfTransform.parent = null;

#if UNITY_EDITOR
            // Spawn debug marker to indicate where the camera boundaries are
            var markerPrefab = Resources.Load("Generic Marker");
            var markers = new List<GameObject>();
            for (var i = 0; i < 4; i++)
            {
                var marker = (GameObject) Instantiate(markerPrefab);
                marker.name = "CameraBoundaryMarker";
                markers.Add(marker);
            }

            markers[0].transform.position = new Vector3(confinedZone.xMin, confinedZone.yMin);
            markers[1].transform.position = new Vector3(confinedZone.xMin, confinedZone.yMax);
            markers[2].transform.position = new Vector3(confinedZone.xMax, confinedZone.yMin);
            markers[3].transform.position = new Vector3(confinedZone.xMax, confinedZone.yMax);
#endif
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            var xMoveDelta = (target.position - m_LastTargetPosition).x;

            var updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = Vector3.right * (lookAheadFactor * Mathf.Sign(xMoveDelta));
            }
            else
            {
                m_LookAheadPos =
                    Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            // This is always positive
            var velocityZoomOut = (targetRB.velocity * Vector2.up).magnitude * zoomOutFactor;

            var position = target.position;
            var aheadTargetPos = position + m_LookAheadPos + Vector3.forward * (m_OffsetZ - velocityZoomOut);

            transform.position = GetBoundSafePosition(transform.position, aheadTargetPos);

            m_LastTargetPosition = position;
        }

        private Vector3 GetBoundSafePosition(Vector3 currentPosition, Vector3 targetPosition)
        {
            var safeTargetPosition = targetPosition;
            var camSize = m_MainCamera.orthographicSize;
            var aspect = m_MainCamera.aspect;
            safeTargetPosition.x = Mathf.Clamp(
                targetPosition.x,
                confinedZone.xMin + camSize * aspect,
                confinedZone.xMax - camSize * aspect
            );
            safeTargetPosition.y = Mathf.Clamp(
                targetPosition.y,
                confinedZone.yMin + camSize,
                confinedZone.yMax - camSize
            );
            var dampedPosition =
                Vector3.SmoothDamp(currentPosition, safeTargetPosition, ref m_CurrentVelocity, damping);
            return dampedPosition;
        }
    }
}