using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandCoreCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Core
{
    public class Graphics
    {
        public List<VertexPositionColorTexture> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public Texture2D Texture
        {
            get => basicEffect.Texture;
            set => basicEffect.Texture = value;
        }
        


        private BasicEffect basicEffect;
        private GraphicsDevice graphicsDevice;


        private int debug = 1;


        public Graphics(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
            Vertices = new List<VertexPositionColorTexture>();
            Indices = new List<int>();
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;
            
        }



        public void Drawing()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && debug < Indices.Count - 1)
                debug += 5;
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && debug > 1)
                debug -= 5;

            basicEffect.World = SandCore.camera.worldMatrix;
            basicEffect.View = SandCore.camera.viewMatrix;
            basicEffect.Projection = SandCore.camera.projectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (Vertices.Count > 0)
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices.ToArray(), 0, Vertices.Count, Indices.ToArray(), 0, Indices.Count - 3);
            }
        }
    }
}
