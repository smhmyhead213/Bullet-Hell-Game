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

        public bool Locked;
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
            Locked = false;
        }

        public void LockCamera(bool locked)
        {
            Locked = locked;
        }
        public void UpdateVisibleArea()
        {
            // try again later
            Microsoft.Xna.Framework.Vector2 centre = Position;
            Microsoft.Xna.Framework.Vector2 topLeft = centre - (Utilities.CentreOfScreen() / CameraScale);
            VisibleArea = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(GameWidth / CameraScale), (int)(GameHeight / CameraScale));
        }

        public void UpdateMatrices()
        {
            // The camera translation has positive X and Y directions as right and down respectively.

            RotationMatrix = Matrix4x4.CreateRotationZ(CameraRotation);

            TranslationMatrix = CalculateTranslationMatrix();

            ZoomMatrix = CalculateScaleMatrix();

            RotationMatrix = CalculateRotationMatrix();

            Matrix = TranslationMatrix * RotationMatrix * ZoomMatrix;
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
        public void SetZoom(float zoomFactor, Microsoft.Xna.Framework.Vector2 focusPoint) // make sure this takes in a draw coordinate and not a world coordinate, otherwise anomalies may appear
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
            // https://learnopengl.com/Getting-Started/Coordinate-Systems

            // the idea is to apply the same transformation that the camera applies to the vertex coordinates.
            // in order to this, we start at world vertex space, move to regular world space, apply the camera transform, go back to vertex space and we're done.

            Matrix4x4 vertexConversion = PrimitiveManager.FourByFourGameToVertex();
            Matrix4x4 invertVertexMatrix = Matrix4x4.Identity;
            bool invertedSuccessfully = Matrix4x4.Invert(vertexConversion, out invertVertexMatrix);

            return vertexConversion * Matrix * invertVertexMatrix;

            Microsoft.Xna.Framework.Vector2 cameraPos = Position - new Microsoft.Xna.Framework.Vector2(GameWidth / 2, GameHeight / 2);
            Matrix4x4 translation = Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-cameraPos.X / GameWidth * 2, cameraPos.Y / GameHeight * 2, 0));

            // we need to transform in vertex coords, the coordinates that the primitives use.
            Microsoft.Xna.Framework.Vector2 vertexOrigin = Utilities.GameCoordsToVertexCoords(Origin);

            System.Numerics.Vector3 originVector = new(vertexOrigin.X, vertexOrigin.Y, 0);

            Matrix4x4 originTransform = Matrix4x4.CreateTranslation(-originVector);
            Matrix4x4 moveBackFromCorner = Matrix4x4.CreateTranslation(originVector);
            Matrix4x4 overallZoomMatrix = moveBackFromCorner * Matrix4x4.CreateScale(CameraScale) * originTransform;

            // rotating currently rotates around 0,0.
            // we can move the camera over the rotation axis, do the rotation and translate back.
            // we need to transform in vertex coords, the coordinates that the primitives use.

            Microsoft.Xna.Framework.Vector2 vertexAxisOfRotation = Utilities.GameCoordsToVertexCoords(RotationAxis);

            // TO DO: MAKE PRIMITVES WORK WITH CAMERA ROTATION
            // rotating currently rotates around 0,0.
            // we can move the camera over the rotation axis, do the rotation and translate back.

            System.Numerics.Vector3 rotationAxisVector = new(vertexAxisOfRotation.X, vertexAxisOfRotation.Y, 0);
            Matrix4x4 axisTransform = Matrix4x4.CreateTranslation(rotationAxisVector);
            Matrix4x4 moveBackFromAxis = Matrix4x4.CreateTranslation(-rotationAxisVector);
            Matrix4x4 overallRotateMatrix = moveBackFromAxis * Matrix4x4.CreateRotationZ(CameraRotation) * axisTransform;

            return translation * overallZoomMatrix;
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
