﻿namespace Zinnia.Tracking.Follow.Modifier.Property.Rotation
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Extension;

    /// <summary>
    /// Updates the transform rotation of the target to match the difference in position of the source.
    /// </summary>
    public class TransformPositionDifferenceRotation : PropertyModifier
    {
        [Tooltip("The maximum distance the Source GameObject can be from the Offset GameObject to allow the rotation to apply.")]
        [SerializeField]
        private Vector3 sourceToOffsetMaximumDistance = Vector3.one * float.PositiveInfinity;
        /// <summary>
        /// The maximum distance the Source <see cref="GameObject"/> can be from the Offset <see cref="GameObject"/> to allow the rotation to apply.
        /// </summary>
        public Vector3 SourceToOffsetMaximumDistance
        {
            get
            {
                return sourceToOffsetMaximumDistance;
            }
            set
            {
                sourceToOffsetMaximumDistance = value;
            }
        }
        [Tooltip("The drag applied to the rotation to slow it down.")]
        [SerializeField]
        private float angularDrag = 1f;
        /// <summary>
        /// The drag applied to the rotation to slow it down.
        /// </summary>
        public float AngularDrag
        {
            get
            {
                return angularDrag;
            }
            set
            {
                angularDrag = value;
            }
        }
        [Tooltip("Determines which axes to rotate.")]
        [SerializeField]
        private Vector3State followOnAxis = Vector3State.True;
        /// <summary>
        /// Determines which axes to rotate.
        /// </summary>
        public Vector3State FollowOnAxis
        {
            get
            {
                return followOnAxis;
            }
            set
            {
                followOnAxis = value;
            }
        }
        [Tooltip("An optional GameObject that is negated from the calculation if both the source and target are descendants of it.")]
        [SerializeField]
        private GameObject ancestor;
        /// <summary>
        /// An optional <see cref="GameObject"/> that is negated from the calculation if both the source and target are descendants of it.
        /// </summary>
        public GameObject Ancestor
        {
            get
            {
                return ancestor;
            }
            set
            {
                ancestor = value;
            }
        }

        /// <summary>
        /// The current angular velocity the rotation is applying to the target.
        /// </summary>
        public Vector3 AngularVelocity { get; protected set; }

        /// <summary>
        /// The previous source world position.
        /// </summary>
        protected Vector3? previousSourcePosition;

        /// <summary>
        /// Clears <see cref="Ancestor"/>.
        /// </summary>
        public virtual void ClearAncestor()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Ancestor = default;
        }

        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> x value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetFollowOnAxisX(bool value)
        {
            FollowOnAxis = new Vector3State(value, FollowOnAxis.yState, FollowOnAxis.zState);
        }

        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> y value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetFollowOnAxisY(bool value)
        {
            FollowOnAxis = new Vector3State(FollowOnAxis.xState, value, FollowOnAxis.zState);
        }

        /// <summary>
        /// Sets the <see cref="FollowOnAxis"/> z value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetFollowOnAxisZ(bool value)
        {
            FollowOnAxis = new Vector3State(FollowOnAxis.xState, FollowOnAxis.yState, value);
        }

        /// <summary>
        /// Resets the state of the source previous position.
        /// </summary>
        public virtual void ResetPreviousState()
        {
            previousSourcePosition = null;
        }

        /// <summary>
        /// Rotates the target based on the position difference of the source.
        /// </summary>
        /// <param name="source">The source to utilize in the modification.</param>
        /// <param name="target">The target to modify.</param>
        /// <param name="offset">The offset of the target against the source when modifying.</param>
        protected override void DoModify(GameObject source, GameObject target, GameObject offset = null)
        {
            AngularVelocity = CalculateAngularVelocity(source, target, offset);
            target.transform.localRotation *= Quaternion.Euler(AngularVelocity);
        }

        protected virtual void OnDisable()
        {
            ResetPreviousState();
        }

        /// <summary>
        /// Calculates the angular velocity based on the differing source position in relation to the target position.
        /// </summary>
        /// <param name="source">The source to utilize in the modification.</param>
        /// <param name="target">The target to modify.</param>
        /// <param name="offset">The offset of the target against the source when modifying.</param>
        /// <returns>The angular velocity to project onto the target.</returns>
        protected virtual Vector3 CalculateAngularVelocity(GameObject source, GameObject target, GameObject offset)
        {
            Vector3 negatePosition = Ancestor != null ? Ancestor.transform.position : Vector3.zero;
            Vector3 sourcePosition = source.transform.position - negatePosition;
            Vector3 targetPosition = target.transform.position - negatePosition;
            Vector3 offsetPosition = offset != null ? offset.transform.position - negatePosition : Vector3.zero;

            if (previousSourcePosition == null)
            {
                previousSourcePosition = sourcePosition;
            }

            if (offset != null && !sourcePosition.WithinDistance(offsetPosition, SourceToOffsetMaximumDistance))
            {
                return Vector3.zero;
            }

            float xDegree = FollowOnAxis.xState ? CalculateAngle(target.transform.right, targetPosition, (Vector3)previousSourcePosition, sourcePosition) : 0f;
            float yDegree = FollowOnAxis.yState ? CalculateAngle(target.transform.up, targetPosition, (Vector3)previousSourcePosition, sourcePosition) : 0f;
            float zDegree = FollowOnAxis.zState ? CalculateAngle(target.transform.forward, targetPosition, (Vector3)previousSourcePosition, sourcePosition) : 0f;

            previousSourcePosition = sourcePosition;

            return ApplyDrag(new Vector3(xDegree, yDegree, zDegree));
        }

        /// <summary>
        /// Calculates the rotational angle for an axis based on the difference between two points around the origin.
        /// </summary>
        /// <param name="originDirection">The direction representing the axis.</param>
        /// <param name="originPoint">The angle centre.</param>
        /// <param name="pointA">The first point to calculate the angle from.</param>
        /// <param name="pointB">The second point to calculate the angle to.</param>
        /// <returns>The angle in degrees between the two points.</returns>
        protected virtual float CalculateAngle(Vector3 originDirection, Vector3 originPoint, Vector3 pointA, Vector3 pointB)
        {
            Vector3 heading = pointB - originPoint;
            float headingMagnitude = heading.magnitude;
            Vector3 sideA = pointA - originPoint;
            if (headingMagnitude.ApproxEquals(0f))
            {
                return 0f;
            }

            Vector3 sideB = heading * (1f / headingMagnitude);
            return Mathf.Atan2(Vector3.Dot(originDirection, Vector3.Cross(sideA, sideB)), Vector3.Dot(sideA, sideB)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Applies an opposing drag force to the current rotational velocity.
        /// </summary>
        /// <param name="angularVelocity">The current rotational velocity being applied.</param>
        /// <returns>The rotational velocity with the opposing drag applied to slow it down.</returns>
        protected virtual Vector3 ApplyDrag(Vector3 angularVelocity)
        {
            return angularVelocity * (1f / AngularDrag);
        }
    }
}