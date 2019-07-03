using System;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;

namespace AdvancedUtilities.Cameras
{
    /// <summary>
    /// A basic camera controller.
    /// </summary>
    [Serializable]
    public class BasicCameraController : CameraController
    {
        /// <summary>
        /// The distance that the camera wants to position itself at from the target.
        /// </summary>
        [Header("Settings")]
        [Tooltip("The distance that the camera wants to position itself at from the target.")]
        public float DesiredDistance = 20f;

        /// <summary>
        /// The minimum that zooming will let you zoom in to.
        /// </summary>
        [Tooltip("The minimum that zooming will let you zoom in to.")]
        public float MinZoomDistance = 0f;

        /// <summary>
        /// The maximum that zooming will let you zoom out to.
        /// </summary>
        [Tooltip("The maximum that zooming will let you zoom out to.")]
        public float MaxZoomDistance = 50f;

        /// <summary>
        /// When the CameraController starts, horizontal rotation will be set to this value.
        /// </summary>
        [Tooltip("When the CameraController starts, horizontal rotation will be set to this value.")]
        public float InitialHorizontalRotation = 0f;

        /// <summary>
        /// When the CameraController starts, vertical rotation will be set to this value.
        /// </summary>
        [Tooltip("When the CameraController starts, vertical rotation will be set to this value.")]
        public float InitialVerticalRotation = 35f;

        #region Components
        /// <summary>
        /// TargetComponent
        /// </summary>
        [Header("Components")]
        public TargetComponent Target;
        /// <summary>
        /// RotationComponent
        /// </summary>
        public RotationComponent Rotation;
        /// <summary>
        /// ZoomComponent
        /// </summary>
        public ZoomComponent Zoom;
        /// <summary>
        /// ViewCollisionComponent
        /// </summary>
        public ViewCollisionComponent ViewCollision;
        /// <summary>
        /// InputComponent
        /// </summary>
        public InputComponent Input;
        /// <summary>
        /// EasyUnityInputComponent
        /// </summary>
        public EasyUnityInputComponent EasyUnityInput;
        /// <summary>
        /// CursorComponent
        /// </summary>
        public CursorComponent Cursor;
        /// <summary>
        /// HeadbobComponent
        /// </summary>
        public HeadbobComponent Headbob;
        /// <summary>
        /// ScreenShakeComponent
        /// </summary>
        public ScreenShakeComponent ScreenShake;
        /// <summary>
        /// 摄像机偏移的设定值
        /// </summary>
        public CameraOffsetComponent CameraOffset;
        #endregion

        /// <summary>
        /// The previous distance the camera was at during the last update.
        /// </summary>
        private float _previousDistance;
        
        protected override void AddCameraComponents()
        {
            AddCameraComponent(Rotation);
            AddCameraComponent(Zoom);
            AddCameraComponent(Target);
            AddCameraComponent(ViewCollision);
            AddCameraComponent(Input);
            AddCameraComponent(EasyUnityInput);
            AddCameraComponent(Cursor);
            AddCameraComponent(Headbob);
            AddCameraComponent(ScreenShake);
            AddCameraComponent(CameraOffset);
        }

        void Start()
        {
            // Set initial rotation and distance
            Rotation.Rotate(InitialHorizontalRotation, InitialVerticalRotation);
            _previousDistance = DesiredDistance;

            // Then let update handle everything
            UpdateCamera();
        }
        
        void LateUpdate()
        {
            UpdateCamera();
            CameraTransform.ApplyTo(Camera); // Apply the virtual transform to the actual transform
        }

        public override void UpdateCamera()
        {
            // Get Input
            //这个设计真的恶心，混乱的引用持有。获取输入，这里已经设置Input的数值了，
            //因为EasyUnityInput里面持有的Input
            EasyUnityInput.AppendInput();
            //获取到经过smooth后的Input的数值
            InputValues input = Input.ProcessedInput;
            //到此输入处理已经结束，之后的控制都是input这个拷贝过来的数值
            Input.ClearInput();

            // Handle Rotating
            //这个自动跟随没有用啊~~没有设置时间参数，为目标位置
            Rotation.UpdateAutoRotate();
            //这个开启了也有问题啊~~
            Rotation.UpdateSmartFollow();
            if (input.Horizontal.HasValue)
            {
                Rotation.RotateHorizontally(input.Horizontal.Value);
            }
            if (input.Vertical.HasValue)
            {
                Rotation.RotateVertically(input.Vertical.Value);
            }

            Rotation.CheckRotationDegreesEvents();

            // Apply target offset modifications
            //获取头部晃动的位置
            Vector3 headbobOffset = Headbob.GetHeadbobModifier(_previousDistance);
            Target.AddWorldSpaceOffset(headbobOffset);
            //获取震屏的位置
            Vector3 screenShakeOffset = ScreenShake.GetShaking();
            Target.AddWorldSpaceOffset(screenShakeOffset);
            //设置摄像机偏移
            Vector3 t_cameraOffset = CameraOffset.GetCameraOffset();
            Target.AddWorldSpaceOffset(t_cameraOffset);
            //获取目标位置
            Vector3 target = Target.GetTarget();

            // Handle Cursor
            Cursor.SetCursorLock();

            // Hanlde Zooming
            if (input.ZoomIn.HasValue)
            {
                DesiredDistance = Mathf.Max(DesiredDistance + input.ZoomIn.Value, 0);
                DesiredDistance = Mathf.Max(DesiredDistance, MinZoomDistance);
            }
            if (input.ZoomOut.HasValue)
            {
                DesiredDistance = Mathf.Min(DesiredDistance + input.ZoomOut.Value, MaxZoomDistance);
            }

            // Set Camera Position
            //计算完成的摄像机的位置
            float desired = DesiredDistance; // Where we want the camera to be
            //计算碰撞的阻挡信息
            float calculated = ViewCollision.CalculateMaximumDistanceFromTarget(target, Mathf.Max(desired, _previousDistance)); // The maximum distance we calculated we can be based off collision and preference
            //计算zoom
            float zoom = Zoom.CalculateDistanceFromTarget(_previousDistance, calculated, desired); // Where we want to be for the sake of zooming
            
            Vector3 zoomDistance = CameraTransform.Forward * zoom;
            //Debug.LogError("===:"+ zoomDistance);
            CameraTransform.Position = target - zoomDistance; // No buffer if the buffer would zoom us in past 0.

            float actual = Vector3.Distance(CameraTransform.Position, target); 

            _previousDistance = actual;
            Target.ClearAdditionalOffsets();
        }
    }
}
