﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS.CharacterMotion.Parameters;

namespace NeoFPS.CharacterMotion.Conditions
{
    [MotionGraphElement("Physics/Ray Cast (Enhanced)")]
    public class EnhancedRayCastCondition : MotionGraphCondition
    {
        [SerializeField, Tooltip("The point on the character capsule to cast from. 0 is the base of the capsule centerline. 1 is the top of the capsule centerline.")]
        private float m_NormalisedHeight = 0f;
        [SerializeField, Tooltip("What to use for the cast vector.")]
        private CastType m_CastType = CastType.LocalVector;
        [SerializeField, Tooltip("The direction and distance to cast relative to the character. The vector does not have to be normalised, as the magnitude will be the maximum distance.")]
        private Vector3 m_CastVector = Vector3.forward;
        [SerializeField, Tooltip("The distance to cast based on the parameter direction vector.")]
        private VectorParameter m_CastVectorParameter = null;
        [SerializeField, Tooltip("The distance to cast based on the parameter direction vector.")]
        private float m_Distance = 1f;
        [SerializeField, Tooltip("How does the cast vector react to slopes. Deflect will deflect the cast vector up if it intersects with the ground plane. Project will project the vector onto the ground slope from above regardless whether the vector collides with the slope. Both only work if the character is grounded.")]
        private SlopeEffect m_SlopeEffect = SlopeEffect.None;
        [SerializeField, Tooltip("The layers to check against.")]
        private LayerMask m_LayerMask = (int)PhysicsFilter.Masks.CharacterBlockers;
        [SerializeField, Tooltip("Is the condition true if the cast hits something or if it doesn't.")]
        private bool m_DoesHit = true;
        [SerializeField, Tooltip("The vector parameter to output the hit point to (optional).")]
        private VectorParameter m_OutputPoint = null;
        [SerializeField, Tooltip("The vector parameter to output the hit normal to (optional).")]
        private VectorParameter m_OutputNormal = null;
        [SerializeField, Tooltip("The transform parameter to output the hit transform to (optional).")]
        private TransformParameter m_OutputTransform = null;

        public enum CastType
        {
            LocalVector,
            WorldVector,
            LocalParameter,
            LocalParameterInverse,
            WorldParameter,
            WorldParameterInverse,
        }

        public enum SlopeEffect
        {
            None,
            Deflect,
            Project
        }

        bool GetScaledVector(out Vector3 output, bool inverse)
        {
            // Check the parameter is set
            if (m_CastVectorParameter == null)
            {
                output = Vector3.zero;
                return false;
            }

            // Check the length
            float length = m_CastVectorParameter.value.magnitude;
            if (length < 0.5f)
            {
                output = Vector3.zero;
                return false;
            }
                    
            // Get the desired cast distance
            float distance = m_Distance + controller.characterController.radius;
            if (inverse)
                distance *= -1f;

            // Set the output
            output = m_CastVectorParameter.value * (distance / length);

            return true;
        }

        public override bool CheckCondition(MotionGraphConnectable connectable)
        {
            bool valid = true;
            Vector3 castVector = Vector3.zero;
            Space space = Space.Self;

            var characterController = controller.characterController;

            switch (m_CastType)
            {
                case CastType.LocalVector:
                    castVector = m_CastVector;
                    space = Space.Self;
                    break;
                case CastType.WorldVector:
                    castVector = m_CastVector;
                    space = Space.World;
                    break;
                case CastType.LocalParameter:
                    valid = GetScaledVector(out castVector, false);
                    space = Space.Self;
                    break;
                case CastType.LocalParameterInverse:
                    valid = GetScaledVector(out castVector, true);
                    space = Space.Self;
                    break;
                case CastType.WorldParameter:
                    valid = GetScaledVector(out castVector, false);
                    space = Space.World;
                    break;
                case CastType.WorldParameterInverse:
                    valid = GetScaledVector(out castVector, true);
                    space = Space.World;
                    break;
            }

            if (valid)
            {
                // Adapt to ground slope
                if (m_SlopeEffect != SlopeEffect.None && characterController.isGrounded)
                {
                    var groundNormal = characterController.groundNormal;

                    // Convert to world space
                    if (space == Space.Self)
                    {
                        castVector = Quaternion.Inverse(controller.localTransform.rotation) * castVector;
                        space = Space.World;
                    }

                    // Project or deflect
                    if (m_SlopeEffect == SlopeEffect.Project || Vector3.Dot(castVector, groundNormal) < 0f)
                    {
                        var up = characterController.up;

                        // Get the horizontal component of the up vector
                        var projected = Vector3.ProjectOnPlane(castVector, up);

                        // Find the vertical offset to the ground plane
                        var groundPlane = new Plane(groundNormal, 0f);
                        float offset;
                        groundPlane.Raycast(new Ray(projected, up), out offset);

                        // Apply offset to get projected vector
                        projected = projected + up * offset;

                        // Scale to original length
                        projected *= castVector.magnitude / projected.magnitude;
                        castVector = projected;
                    }
                }

                // Perform the cast
                RaycastHit hit;
                bool didHit = controller.characterController.RayCast(m_NormalisedHeight, castVector, space, out hit, m_LayerMask);

                // Output to paramters
                if (m_OutputPoint != null)
                    m_OutputPoint.value = hit.point;
                if (m_OutputNormal != null)
                    m_OutputNormal.value = hit.normal;
                if (m_OutputTransform != null)
                    m_OutputTransform.value = hit.transform;
                
                // return does hit
                return didHit == m_DoesHit;
            }
            else
            {
                // Output to paramters
                if (m_OutputPoint != null)
                    m_OutputPoint.value = Vector3.zero;
                if (m_OutputNormal != null)
                    m_OutputNormal.value = Vector3.zero;
                if (m_OutputTransform != null)
                    m_OutputTransform.value = null;

                return false;
            }
        }

        public override void CheckReferences(IMotionGraphMap map)
        {
            m_CastVectorParameter = map.Swap(m_CastVectorParameter);
            m_OutputPoint = map.Swap(m_OutputPoint);
            m_OutputNormal = map.Swap(m_OutputNormal);
            m_OutputTransform = map.Swap(m_OutputTransform);
        }
    }
}
