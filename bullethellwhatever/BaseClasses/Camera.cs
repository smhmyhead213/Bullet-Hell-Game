using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.MediaFoundation;
using bullethellwhatever.MainFiles;
using System.Threading;
using SharpDX.Direct3D9;
using bullethellwhatever.DrawCode;


namespace bullethellwhatever.BaseClasses
{
    public class Camera
    {
        public Viewport Viewport;

        public Matrix4x4 Matrix;

        private float screenShakeOffset;
        private float screenShakeRotationOffset;
        private float cameraRotation;
        private float cameraScale;

        private Microsoft.Xna.Framework.Vector2 position;
        public float ScreenShakeOffset
        {
            get
            {
                return screenShakeOffset;
            }
            set
            {
                screenShakeOffset = value;
                UpdateMatrices();
            }
        }

        public float ScreenShakeRotationOffset
        {
            get
            {
                return screenShakeRotationOffset;
            }
            set
            {
                screenShakeRotationOffset = value;
                UpdateMatrices();
            }
        }
        public float CameraRotation
        {
            get
            {
                return cameraRotation;
            }
            set
            {
                cameraRotation = value;
                UpdateMatrices();
            }
        }

        public float CameraScale
        {
            get
            {
                return cameraScale;
            }
            set
            {
                cameraScale = value;
                UpdateMatrices();
            }
        }

        public Rectangle VisibleArea;

        public bool TranslationLocked;
        public bool ZoomLocked;
        /// <summary>
        /// The position of the camera in <b>world co-ordinates</b>.
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                UpdateMatrices();
            }
        }

        /// <summary>
        /// The origin point of the camera, where it has its centre (0,0).
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Origin; // camera origin

        /// <summary>
        /// The point in world space that the camera can rotate around.
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 RotationAxis;

        public Matrix4x4 RotationMatrix; // to be added (what rotation matrix would be used, considering that the origin is the top left)

        public Matrix4x4 TranslationMatrix; // translation matrix that allows for camera panning

        public Matrix4x4 ZoomMatrix; // scale matrix that allows for zooming in and out
        public Camera()
        {
            Reset();
            UpdateMatrices();
            TranslationLocked = false;
            ZoomLocked = false;
        }

        public void Lock(bool locked = true)
        {
            LockCameraMovement(locked);
            LockCameraZoom(locked);
        }

        public void Unlock()
        {
            LockCameraMovement(false);
            LockCameraZoom(false);
        }
        public void LockCameraMovement(bool locked = true)
        {
            TranslationLocked = locked;
        }
        public void UnlockCameraMovement()
        {
            TranslationLocked = false;
        }
        public void LockCameraZoom(bool locked = true)
        {
            ZoomLocked = locked;
        }
        public void UnlockCameraZoom()
        {
            ZoomLocked = false;
        }

        public void UpdateVisibleArea()
        {
            // only with translation for now, add zoom later possibly

            Microsoft.Xna.Framework.Vector2 visibleCentre = position;
            Microsoft.Xna.Framework.Vector2 topLeft = position - new Microsoft.Xna.Framework.Vector2(GameWidth, GameHeight) / 2 / cameraScale;
            VisibleArea = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(GameWidth / cameraScale), (int)(GameHeight / cameraScale));
        }

        public void UpdateMatrices()
        {
            // The camera translation has positive X and Y directions as right and down respectively.

            RotationMatrix = Matrix4x4.CreateRotationZ(CameraRotation);

            TranslationMatrix = CalculateTranslationMatrix();

            ZoomMatrix = CalculateScaleMatrix();

            RotationMatrix = CalculateRotationMatrix();
         
            Matrix = TranslationMatrix * RotationMatrix * ZoomMatrix;
            //Matrix = Matrix4x4.Identity;
        }

        public Microsoft.Xna.Framework.Vector2 CameraPositionWithScreenShake()
        {
            return new Microsoft.Xna.Framework.Vector2(Position.X + ScreenShakeOffset, Position.Y + ScreenShakeOffset);
        }
        private Matrix4x4 CalculateTranslationMatrix()
        {
            Microsoft.Xna.Framework.Vector2 cameraPos = Position - new Microsoft.Xna.Framework.Vector2(GameWidth / 2, GameHeight / 2);
            return Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-(cameraPos.X + ScreenShakeOffset), -(cameraPos.Y + ScreenShakeOffset), 0));
        }
        private Matrix4x4 CalculateScaleMatrix()
        {
            // the zoom matrix normally zooms the camera to the top left of the screen.
            // this fixes the problem by translating everything so the zoom point is at the top left of the screen, zooming, and then moving everything back.

            System.Numerics.Vector3 originVector = new(Origin.X - GameWidth, Origin.Y - GameHeight, 0);

            Matrix4x4 originTransform = Matrix4x4.CreateTranslation(-originVector);
            Matrix4x4 moveBackFromCorner = Matrix4x4.CreateTranslation(originVector);
            return moveBackFromCorner * Matrix4x4.CreateScale(CameraScale) * originTransform;
        }

        private Matrix4x4 CalculateRotationMatrix()
        {
            // rotating currently rotates around 0,0.
            // we can move the camera over the rotation axis, do the rotation and translate back.

            RotationMatrix = Matrix4x4.CreateRotationZ(CameraRotation + ScreenShakeRotationOffset);
            System.Numerics.Vector3 rotationAxisVector = new(RotationAxis.X, RotationAxis.Y, 0);
            Matrix4x4 axisTransform = Matrix4x4.CreateTranslation(rotationAxisVector);
            Matrix4x4 moveBackFromAxis = Matrix4x4.CreateTranslation(-rotationAxisVector);
            return moveBackFromAxis * RotationMatrix * axisTransform;
        }

        public void MoveCameraBy(Microsoft.Xna.Framework.Vector2 offset)
        {
            Position += offset;
            UpdateMatrices();
        }
        /// <summary>
        /// <param name="position">The position of the camera in world co-ordinates.</param>
        /// </summary>

        public void SetRotation(float radians)
        {
            CameraRotation = radians;
            UpdateMatrices();
        }

        public void ZoomBy(float extraZoom)
        {
            CameraScale += extraZoom;
        }

        public void SetZoom(float zoomFactor, Microsoft.Xna.Framework.Vector2 focusPoint) // make sure this takes in a draw coordinate and not a world coordinate, otherwise anomalies may appear (?)
        {
            CameraScale = zoomFactor;

            Origin = focusPoint;

            Matrix4x4 zoomMatrix = Matrix4x4.CreateScale(new System.Numerics.Vector3(zoomFactor, zoomFactor, 0));

            ZoomMatrix = zoomMatrix;

            UpdateMatrices();
        }

        public void ResetTranslation()
        {
            TranslationMatrix = Matrix4x4.Identity;
            UpdateMatrices();
        }
        public void ResetZoom()
        {
            ZoomMatrix = Matrix4x4.Identity;
            UpdateMatrices();
        }


        public Matrix4x4 ShaderMatrix()
        {
            //return Matrix4x4.Identity;

            System.Numerics.Vector3 cameraPositionVertex = PrimitiveManager.ToVertexCoordsThree(CameraPositionWithScreenShake());
            Matrix4x4 translation = Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-cameraPositionVertex.X, -cameraPositionVertex.Y, 0));

            System.Numerics.Vector3 originPositionVertex = PrimitiveManager.ToVertexCoordsThree(Origin);
            Matrix4x4 gothere = Matrix4x4.CreateTranslation(-originPositionVertex);
            Matrix4x4 zoom = new Matrix4x4(CameraScale, 0, 0, 0, 0, CameraScale, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1); // mess not with the z coordinate. if we change the z-coord too much, everything vanishes. my guess is near-far plane shenanigans?
            Matrix4x4 goback = Matrix4x4.CreateTranslation(originPositionVertex);
            Matrix4x4 zoomMatrix = goback * zoom * gothere;

            float aspectRatio = Utilities.AspectRatio();
            Matrix4x4 rotation = Utilities.ScaleMatrix(aspectRatio, 1) * Matrix4x4.CreateRotationZ(-CameraRotation + ScreenShakeRotationOffset) * Utilities.ScaleMatrix(1 / aspectRatio, 1);
            return translation * rotation * zoomMatrix;

            // https://learnopengl.com/Getting-Started/Coordinate-Systems
        }
        public void Reset()
        {
            TranslationMatrix = Matrix4x4.Identity;
            ZoomMatrix = Matrix4x4.Identity;
            RotationMatrix = Matrix4x4.Identity;
            Position = Utilities.CentreOfScreen();
            Origin = Utilities.CentreOfScreen();
            RotationAxis = Utilities.CentreOfScreen();
            ScreenShakeOffset = 0;
            CameraRotation = 0;
            CameraScale = 1;
        }
    }

}
