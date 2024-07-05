using Microsoft.Xna.Framework.Graphics;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.MediaFoundation;


namespace bullethellwhatever.BaseClasses
{
    public class Camera
    {
        public Viewport Viewport;

        public Matrix4x4 Matrix;

        /// <summary>
        /// The position of the camera in <b>world co-ordinates</b>.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The origin point of the camera, where it has its centre (0,0).
        /// </summary>
        public Vector2 Origin; // camera origin

        private Matrix4x4 RotationMatrix; // to be added (what rotation matrix would be used, considering that the origin is the top left)

        private Matrix4x4 TranslationMatrix; // translation matrix that allows for camera panning

        private Matrix4x4 ZoomMatrix; // scale matrix that allows for zooming in and out
        public Camera()
        {
            Viewport = new(0, 0, ScreenWidth, ScreenHeight);
            TranslationMatrix = Matrix4x4.Identity;
            ZoomMatrix = Matrix4x4.Identity;
            RotationMatrix = Matrix4x4.Identity;
            Position = new Vector2(0, 0);
            Origin = new Vector2(0, 0);
        }

        public void UpdateMatrices()
        {
            TranslationMatrix = Matrix4x4.CreateTranslation(new Vector3(Position, 0));
            Matrix4x4 originTransform = Matrix4x4.CreateTranslation(new Vector3(Origin, 0));
            Matrix = TranslationMatrix * RotationMatrix * ZoomMatrix * originTransform;
        }

        public void MoveCameraBy(Vector2 offset)
        {
            Position += offset;
        }
        public void SetCameraPositon(Vector2 position)
        {
            Position = position;
        }
        public void SetRotation(float radians)
        {
            RotationMatrix = Matrix4x4.CreateRotationZ(radians);
        }
        public void SetZoom(float zoomFactor)
        {
            SetZoom(zoomFactor, Vector2.Zero);
        }
        public void SetZoom(float zoomFactor, Vector2 focusPoint) // make sure this takes in a draw coordinate and not a world coordinate, otherwise anomalies may appear
        {
            Origin = focusPoint;

            Matrix4x4 zoomMatrix = Matrix4x4.CreateScale(new Vector3(zoomFactor, zoomFactor, 0));

            ZoomMatrix = zoomMatrix;
        }
        public void ResetTranslation()
        {
            TranslationMatrix = Matrix4x4.Identity;
        }
        public void ResetZoom()
        {
            ZoomMatrix = Matrix4x4.Identity;
        }
        public void ResetMatrices()
        {
            ResetTranslation();
            ResetZoom();
            Origin = Vector2.Zero;
            Position = Vector2.Zero;
        }
    }

}
