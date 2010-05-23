using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PongXNA
{
    class Paddle
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
        public Vector2 Direction {
            get { return direction; }
        }

        private Vector2 size;
        private Vector2 resetPosition;

        private float speed;

        private Texture2D image;

        public Paddle(Texture2D image, Vector2 direction, Vector2 position) {
            this.image = image;
            this.direction = direction;
            this.resetPosition = position;
            Reset();
        }

        public BoundingBox Bounding {
            get {
                return new BoundingBox(
                    new Vector3(X - size.X / 2, Y - size.Y / 2, 0),
                    new Vector3(X+size.X/2, Y+size.Y/2, 0)
                );
            }
        }

        public void Reset() {
            this.position = resetPosition;
            this.speed = 4.0f;
            this.size.X = 20.0f;
            this.size.Y = 90.0f;
        }

        public virtual void Update() {
        }

        protected void MoveUp() {
            float wallTop = 20;
            this.position.Y -= speed;
            if (this.position.Y < wallTop + this.size.Y / 2)
            {
                this.position.Y = wallTop + this.size.Y / 2;
            }
        }

        protected void MoveDown()
        {
            float wallBottom = 460;
            this.position.Y += speed;
            if (this.position.Y > wallBottom - this.size.Y / 2)
            {
                this.position.Y = wallBottom - this.size.Y / 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(image, position - (size / 2), Color.White);
        }
    }
}
