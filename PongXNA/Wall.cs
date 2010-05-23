using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongXNA
{
    class Wall
    {
        private Vector2 position;
        public Vector2 Position {
            get { return position;}
        }
        public float X
        {
            get { return position.X; }
        }
        public float Y
        {
            get { return position.Y; }
        }

        private Vector2 direction;
        public Vector2 Direction
        {
            get { return direction; }
        }

        public BoundingBox Bounding
        {
            get
            {
                return new BoundingBox(
                    new Vector3(X - width / 2, Y - thickness / 2, 0),
                    new Vector3(X + width / 2, Y + thickness / 2, 0)
                );
            }
        }

        private float width = 640;
        private float thickness = 20;

        private Texture2D image;

        public Wall(Texture2D image, Vector2 direction, Vector2 position) {
            this.image = image;
            this.direction = direction;
            this.position = position;
        }

        public void Update() {
        }

        public void Draw(SpriteBatch spriteBatch) {
            Vector2 drawPosition = position - new Vector2(width / 2, thickness / 2);
            spriteBatch.Draw(image, drawPosition, Color.White);
        }
    }
}
