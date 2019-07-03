﻿using UnityEngine;

namespace AdvancedUtilities
{
    /// <summary>
    /// See more Advanced Utilities @
    /// https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:18832
    /// 
    /// This class' purpose is to represent a Transform and to enable the use of its functions without 
    /// being attached to an object. Transforms must be attached to a Game Object and cannot be new'd up, 
    /// and they also affect the state of the world in Unity when altered.
    /// 
    /// This class will act as a detached Transform allowing you to use the normal functions of a transform 
    /// without affecting the state of the world, or needing to use a hidden transform to get a similar effect.
    /// </summary>
    public class VirtualTransform
    {
        #region Public Properties

        /// <summary>
        /// The position of the transform stored as a Vector3 in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the transform stored as a Quaternion in world space.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The scale of the transform relative to the parent.
        /// </summary>
        public Vector3 LocalScale { get; set; }

        /// <summary>
        /// The rotation as Euler angles in degrees.
        /// </summary>
        public Vector3 EulerAngles
        {
            get
            {
                return Rotation.eulerAngles;
            }
            set
            {
                Rotation = Quaternion.Euler(value);
            }
        }

        /// <summary>
        /// The forward vector of the transform.
        /// The blue axis of the transform in world space.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                return (Rotation * Vector3.forward).normalized;
            }
        }

        /// <summary>
        /// The right vector of the transform.
        /// The red axis of the transform in world space.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return (Rotation * Vector3.right).normalized;
            }
        }

        /// <summary>
        /// The upwards vector of the transform.
        /// The green axis of the transform in world space.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                return (Rotation * Vector3.up).normalized;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a VirtualTransform with the position at the world origin, the rotation as the quaternion identity, and the local scale as 1.
        /// </summary>
        public VirtualTransform()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            LocalScale = Vector3.one;
        }

        /// <summary>
        /// Constructs a VirtualTransform with the position at given position, the rotation as the quaternion identity, and the local scale as 1.
        /// </summary>
        public VirtualTransform(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
            LocalScale = Vector3.one;
        }

        /// <summary>
        /// Constructs a VirtualTransform with the position at the world origin, the rotation as the given quaternion, and the local scale as 1.
        /// </summary>
        public VirtualTransform(Quaternion rotation)
        {
            Position = Vector3.zero;
            Rotation = rotation;
            LocalScale = Vector3.one;
        }

        /// <summary>
        /// Constructs a VirtualTransform with the position at the given position, the rotation as the given quaternion, and the local scale as 1.
        /// </summary>
        public VirtualTransform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            LocalScale = Vector3.one;
        }

        /// <summary>
        /// Constructs a VirtualTransform with the position at the given position, the rotation as the given quaternion, and the local scale as the given local scale.
        /// </summary>
        public VirtualTransform(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            Position = position;
            Rotation = rotation;
            LocalScale = localScale;
        }

        /// <summary>
        /// Constructs a VirtualTransform setting the position, rotation, and local scale to the values of the given Transform.
        /// </summary>
        /// <param name="transform">Transform whose properties you want to copy.</param>
        public VirtualTransform(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            LocalScale = transform.localScale;
        }

        /// <summary>
        /// Constructs a VirtualTransform setting the position, rotation, and local scale to the values of the given GameObject's transform.
        /// </summary>
        /// <param name="gameObject">GameObject whose properties you want to copy.</param>
        public VirtualTransform(GameObject gameObject)
        {
            Position = gameObject.transform.position;
            Rotation = gameObject.transform.rotation;
            LocalScale = gameObject.transform.localScale;
        }

        /// <summary>
        /// Constructs a VirtualTransform setting the position, rotation, and local scale to the values of the given MonoBehaviour's transform.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour whose properties you want to copy.</param>
        public VirtualTransform(MonoBehaviour monoBehaviour)
        {
            Position = monoBehaviour.transform.position;
            Rotation = monoBehaviour.transform.rotation;
            LocalScale = monoBehaviour.transform.localScale;
        }

        /// <summary>
        /// Constructs a VirtualTransform setting the position, rotation, and local scale to the values of the given Camera's transform.
        /// </summary>
        /// <param name="camera">Camera whose properties you want to copy.</param>
        public VirtualTransform(Camera camera)
        {
            Position = camera.transform.position;
            Rotation = camera.transform.rotation;
            LocalScale = camera.transform.localScale;
        }

        /// <summary>
        /// Constructs a VirtualTransform setting the position, rotation, and local scale to the values of the given VirtualTransform.
        /// </summary>
        /// <param name="virtualTransform">VirtualTransform whose properties you want to copy.</param>
        public VirtualTransform(VirtualTransform virtualTransform)
        {
            Position = virtualTransform.Position;
            Rotation = virtualTransform.Rotation;
            LocalScale = virtualTransform.LocalScale;
        }

        #endregion

        #region Apply

        /// <summary>
        /// Applies the position, rotation, and localScale properties of the Transform to this VirtualTransform.
        /// </summary>
        /// <param name="transform">The Transform whose properties you want to apply to this VirtualTransform.</param>
        public void Apply(Transform transform)
        {
            this.Position = transform.position;
            this.Rotation = transform.rotation;
            this.LocalScale = transform.localScale;
        }

        /// <summary>
        /// Applies the position, rotation, and localScale properties of the VirtualTransform to this VirtualTransform.
        /// </summary>
        /// <param name="virtualTransform">The VirtualTransform whose properties you want to apply to this VirtualTransform.</param>
        public void Apply(VirtualTransform virtualTransform)
        {
            this.Position = virtualTransform.Position;
            this.Rotation = virtualTransform.Rotation;
            this.LocalScale = virtualTransform.LocalScale;
        }

        /// <summary>
        /// Applies the position, rotation, and localScale properties of the GameObject's transform to this VirtualTransform.
        /// </summary>
        /// <param name="gameObject">The GameObject whose transform's properties you want to apply to this VirtualTransform.</param>
        public void Apply(GameObject gameObject)
        {
            Apply(gameObject.transform);
        }

        /// <summary>
        /// Applies the position, rotation, and localScale properties of the MonoBehaviour's transform to this VirtualTransform.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour whose transform's properties you want to apply to this VirtualTransform.</param>
        public void Apply(MonoBehaviour monoBehaviour)
        {
            Apply(monoBehaviour.transform);
        }

        /// <summary>
        /// Applies the position, rotation, and localScale properties of the Camera's transform to this VirtualTransform.
        /// </summary>
        /// <param name="camera">The Camera whose transform's properties you want to apply to this VirtualTransform.</param>
        public void Apply(Camera camera)
        {
            Apply(camera.transform);
        }

        #endregion

        #region ApplyTo

        /// <summary>
        /// Applies the properties of this VirtualTransform to the transform of the given Game Object.
        /// Scale isn't applied unless set to true.
        /// </summary>
        /// <param name="gameObject">GameObject whose transform you want to apply this VirtualTransform's properties to.</param>
        /// <param name="applyScale">If you also want to apply scale.</param>
        public void ApplyTo(GameObject gameObject, bool applyScale = false)
        {
            ApplyTo(gameObject.transform, applyScale);
        }

        /// <summary>
        /// Applies the properties of this VirtualTransform to the transform of the given MonoBehaviour.
        /// Scale isn't applied unless set to true.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour whose transform you want to apply this VirtualTransform's properties to.</param>
        /// <param name="applyScale">If you also want to apply scale.</param>
        public void ApplyTo(MonoBehaviour monoBehaviour, bool applyScale = false)
        {
            ApplyTo(monoBehaviour.transform, applyScale);
        }

        /// <summary>
        /// Applies the properties of this VirtualTransform to the transform of the given Camera.
        /// Scale isn't applied unless set to true.
        /// </summary>
        /// <param name="camera">Camera whose transform you want to apply this VirtualTransform's properties to.</param>
        /// <param name="applyScale">If you also want to apply scale.</param>
        public void ApplyTo(Camera camera, bool applyScale = false)
        {
            ApplyTo(camera.transform, applyScale);
        }

        /// <summary>
        /// Applies the properties of this VirtualTransform to the transform of the given Transform.
        /// Scale isn't applied unless set to true.
        /// </summary>
        /// <param name="transform">The transform you want to apply this VirtualTransform's properties to.</param>
        /// <param name="applyScale">If you also want to apply scale.</param>
        public void ApplyTo(Transform transform, bool applyScale = false)
        {
            transform.position = this.Position;
            transform.rotation = this.Rotation;

            if (applyScale)
            {
                transform.localScale = this.LocalScale;
            }
        }

        /// <summary>
        /// Applies the properties of this VirtualTransform to the given VirtualTransform.
        /// Scale isn't applied unless set to true.
        /// </summary>
        /// <param name="virtualTransform">VirtualTransform you want to apply this VirtualTransform's properties to.</param>
        /// <param name="applyScale">If you also want to apply scale.</param>
        public void ApplyTo(VirtualTransform virtualTransform, bool applyScale = false)
        {
            virtualTransform.Position = this.Position;
            virtualTransform.Rotation = this.Rotation;

            if (applyScale)
            {
                virtualTransform.LocalScale = this.LocalScale;
            }
        }

        #endregion

        #region LookAt

        /// <summary>
        /// Rotates the VirtualTransform to look at the point.
        /// Uses Vector3.up as the worldUp vector.
        /// </summary>
        /// <param name="position">Position in world space to look at.</param>
        public void LookAt(Vector3 position)
        {
            LookAt(position, Vector3.up);
        }

        /// <summary>
        /// Rotates the VirtualTransform to look at the point.
        /// </summary>
        /// <param name="position">Position in world space to look at.</param>
        /// <param name="worldUp">Upwards vector.</param>
        public void LookAt(Vector3 position, Vector3 worldUp)
        {
            Vector3 relativePos = position - this.Position;

            Rotation = Quaternion.LookRotation(relativePos, worldUp.normalized);
        }
        #endregion

        #region Translate

        /// <summary>
        /// Translates the position of the transform by the given value in world space.
        /// </summary>
        /// <param name="movement">Amount to move by.</param>
        public void Translate(Vector3 movement)
        {
            Position += movement;
        }

        #endregion

        #region Rotate

        /// <summary>
        /// Rotates the rotation of the transform by the given quaternion value in the world space.
        /// </summary>
        /// <param name="rotation">Amount to rotate by.</param>
        public void Rotate(Quaternion rotation)
        {
            Rotate(rotation, Space.World);
        }

        /// <summary>
        /// Rotates the rotation of the transform by the given quaternion value in the given space.
        /// </summary>
        /// <param name="rotation">Amount to rotate by.</param>
        /// <param name="space">The space you want to rotate in.</param>
        public void Rotate(Quaternion rotation, Space space)
        {
            
            if (space == Space.World)
            {
                Rotation = rotation * Rotation;
            }
            else
            {
                Rotation = Rotation * rotation;
            }
        }

        /// <summary>
        /// Rotates the transform on the given axis by the given angle in the given space.
        /// </summary>
        /// <param name="axis">Space you want to rotate on.</param>
        /// <param name="angle">Angle you want to rotate by.</param>
        /// <param name="space">Space you want to rotate in.</param>
        public void Rotate(Vector3 axis, float angle, Space space)
        {
            Rotate(Quaternion.AngleAxis(angle, axis), space);
        }

        /// <summary>
        /// Rotates the transform on the given axis by the given angle in the world space.
        /// </summary>
        /// <param name="axis">Space you want to rotate on.</param>
        /// <param name="angle">Angle you want to rotate by.</param>
        public void Rotate(Vector3 axis, float angle)
        {
            Rotate(Quaternion.AngleAxis(angle, axis));
        }

        #endregion

        #region GetLocalPosition

        /// <summary>
        /// Finds the local position of this VirtualTransform using the given parent position.
        /// </summary>
        /// <param name="parentPositon">The position of the parent.</param>
        /// <returns>The local position relative to the given parent position.</returns>
        public Vector3 GetLocalPosition(Vector3 parentPositon)
        {
            return Position - parentPositon;
        }
        #endregion

        #region GetLocalRotation

        /// <summary>
        /// Finds the local rotation of this VirtualTransform using the given parent rotation.
        /// </summary>
        /// <param name="parentRotation">Rotation of the parent.</param>
        /// <returns>Local rotation of this VirtualTransform relative to the parent rotation.</returns>
        public Quaternion GetLocalRotation(Quaternion parentRotation)
        {
            return Quaternion.Inverse(parentRotation) * Rotation;
        }

        /// <summary>
        /// Finds the local rotation of this VirtualTransform using the given parent Transform rotation.
        /// </summary>
        /// <param name="parentTransform">Transform of the parent.</param>
        /// <returns>Local rotation of this VirtualTransform relative to the parent rotation.</returns>
        public Quaternion GetLocalRotation(Transform parentTransform)
        {
            return GetLocalRotation(parentTransform.rotation);
        }

        /// <summary>
        /// Finds the local rotation of this VirtualTransform using the given parent VirtualTransform rotation.
        /// </summary>
        /// <param name="parentTransform">Transform of the parent.</param>
        /// <returns>Local rotation of this VirtualTransform relative to the parent rotation.</returns>
        public Quaternion GetLocalRotation(VirtualTransform parentTransform)
        {
            return GetLocalRotation(parentTransform.Rotation);
        }

        #endregion
        
        public override string ToString()
        {
            return string.Format("Position: {0}, Rotation: {1}, LocalScale: {2}", Position, Rotation, LocalScale);
        }
    }
}