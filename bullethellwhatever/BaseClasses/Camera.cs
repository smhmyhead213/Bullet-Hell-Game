﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.MediaFoundation;
using bullethellwhatever.MainFiles;
using System.Threading;


namespace bullethellwhatever.BaseClasses
{
    public class Camera
    {
        public Viewport Viewport;

        public Matrix4x4 Matrix;

        public float ScreenShakeOffset;

        public float CameraRotation;
        /// <summary>
        /// The position of the camera in <b>world co-ordinates</b>.
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Position;

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
            TranslationMatrix = Matrix4x4.Identity;
            ZoomMatrix = Matrix4x4.Identity;
            RotationMatrix = Matrix4x4.Identity;
            Position = new Microsoft.Xna.Framework.Vector2(0, 0);
            Origin = new Microsoft.Xna.Framework.Vector2(0, 0);
            RotationAxis = Utilities.CentreOfScreen();
            ScreenShakeOffset = 0;
            CameraRotation = 0;
            SetZoom(0.5f, new Microsoft.Xna.Framework.Vector2(0, 0));
            //SetCameraPosition(Utilities.CentreOfScreen() + new Microsoft.Xna.Framework.Vector2(100f, 0));
        }

        public void UpdateMatrices()
        {
            //SetRotation(PI / 2);

            RotationMatrix = Matrix4x4.CreateRotationZ(CameraRotation);

            TranslationMatrix = Matrix4x4.CreateTranslation(new System.Numerics.Vector3(Position.X + ScreenShakeOffset, Position.Y + ScreenShakeOffset, 0));

            // the zoom matrix normally zooms the camera to the top left of the screen.
            // this fixes the problem by translating everything so the zoom point is at the top left of the screen, zooming, and then moving everything back.

            System.Numerics.Vector3 originVector = new(GameWidth - Origin.X, GameHeight - Origin.Y, 0);

            Matrix4x4 originTransform = Matrix4x4.CreateTranslation(originVector);
            Matrix4x4 moveBackFromCorner = Matrix4x4.CreateTranslation(-originVector);
            Matrix4x4 overallZoomMatrix = moveBackFromCorner * ZoomMatrix * originTransform;

            // rotating currently rotates around 0,0.
            // we can move the camera over the rotation axis, do the rotation and translate back.

            System.Numerics.Vector3 rotationAxisVector = new(RotationAxis.X, RotationAxis.Y, 0);

            Matrix4x4 axisTransform = Matrix4x4.CreateTranslation(rotationAxisVector);
            Matrix4x4 moveBackFromAxis = Matrix4x4.CreateTranslation(-rotationAxisVector);
            Matrix4x4 overallRotateMatrix = moveBackFromAxis * RotationMatrix * axisTransform;

            Matrix = TranslationMatrix * overallRotateMatrix * overallZoomMatrix;
        }

        public void MoveCameraBy(Microsoft.Xna.Framework.Vector2 offset)
        { 
            Position += offset;
        }
        /// <summary>
        /// <param name="position">The position of the camera in world co-ordinates.</param>
        /// </summary>
        public void SetCameraPosition(Microsoft.Xna.Framework.Vector2 position)
        {
            Position = Utilities.CentreOfScreen() - position;
        }
        public void SetRotation(float radians)
        {
            CameraRotation = radians;            
        }
        public void SetZoom(float zoomFactor)
        {
            SetZoom(zoomFactor, Utilities.CentreOfScreen());
        }
        public void SetZoom(float zoomFactor, Microsoft.Xna.Framework.Vector2 focusPoint) // make sure this takes in a draw coordinate and not a world coordinate, otherwise anomalies may appear
        {
            Origin = focusPoint;

            Matrix4x4 zoomMatrix = Matrix4x4.CreateScale(new System.Numerics.Vector3(zoomFactor, zoomFactor, 0));

            ZoomMatrix = zoomMatrix;
        }

        public void SetScreenShakeOffset(float offset)
        {
            ScreenShakeOffset = offset;
        }
        public void ResetTranslation()
        {
            TranslationMatrix = Matrix4x4.Identity;
        }
        public void ResetZoom()
        {
            ZoomMatrix = Matrix4x4.Identity;
        }

        public Matrix4x4 ShaderMatrix()
        {
            Matrix4x4 translation = Matrix4x4.CreateTranslation(new System.Numerics.Vector3((Position.X + ScreenShakeOffset) / GameWidth * 2, (Position.Y + ScreenShakeOffset) / GameHeight * 2, 0));

            System.Numerics.Vector3 originVector = new(Origin.X / GameWidth * 2f, Origin.Y / GameHeight * 2f, 0);

            Matrix4x4 originTransform = Matrix4x4.CreateTranslation(originVector);
            Matrix4x4 moveBackFromCorner = Matrix4x4.CreateTranslation(-originVector);
            Matrix4x4 overallZoomMatrix = moveBackFromCorner * ZoomMatrix * originTransform;

            // rotating currently rotates around 0,0.
            // we can move the camera over the rotation axis, do the rotation and translate back.

            //System.Numerics.Vector3 rotationAxisVector = new(RotationAxis.X / GameWidth, RotationAxis.Y / GameHeight, 0);

            //Matrix4x4 axisTransform = Matrix4x4.CreateTranslation(rotationAxisVector);
            //Matrix4x4 moveBackFromAxis = Matrix4x4.CreateTranslation(-rotationAxisVector);
            //Matrix4x4 overallRotateMatrix = moveBackFromAxis * RotationMatrix * axisTransform;

            return translation *  overallZoomMatrix;
        }
        public void Reset()
        {
            ResetTranslation();
            ResetZoom();
            Origin = Microsoft.Xna.Framework.Vector2.Zero;
            Position = Microsoft.Xna.Framework.Vector2.Zero;
            ScreenShakeOffset = 0;
        }
    }

}
