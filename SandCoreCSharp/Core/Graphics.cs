using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public List<VertexPositionColor> vertices { get; set; }
        public VertexPositionColor[] vertx;
        private BasicEffect basicEffect;
        private GraphicsDevice graphicsDevice;

        

        public Graphics(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
            vertices = new List<VertexPositionColor>();

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

            vertx = vertices.ToArray();
        }



        public void Drawing()
        {
            worldMatrix = SandCore.camera.WorldPos;

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if(vertx.Length > 0)
                    graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertx, 0, vertices.Count - 1);
            }
        }
    }
}
