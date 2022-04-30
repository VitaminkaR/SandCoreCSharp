using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core
{
    class Graphics
    {
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        public List<VertexPositionColor> Vertices { get; set; }
        public List<int> Indices { get; set; }
        


        private BasicEffect basicEffect;
        private GraphicsDevice graphicsDevice;


        private int debug = 1;


        public Graphics(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
            Vertices = new List<VertexPositionColor>();
            Indices = new List<int>();

            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)graphicsDevice.Viewport.Width /
                (float)graphicsDevice.Viewport.Height,
                1.0f, 100.0f);


            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
        }



        public void Drawing()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1 + Mouse.GetState().ScrollWheelValue / -100), Vector3.Zero, Vector3.Up);

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && debug < Indices.Count - 1)
                debug += 5;
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && debug > 1)
                debug -= 5;

            worldMatrix = SandCore.camera.WorldPos;

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (Vertices.Count > 0)
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices.ToArray(), 0, Vertices.Count, Indices.ToArray(), 0, Indices.Count - 3);
            }
        }
    }
}
