using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongXNA
{
    class Ball
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

        private float radius = 10;
        public float Radius {
            get { return radius; }
        }

        private float speed;
        public float Speed {
            get { return speed; }
            set { speed = value; }
        }

        private Vector2 vector;
        public Vector2 Vector
        {
            get { return vector; }
            set
            {
                vector = value;
                vector.Normalize();
            }
        }

        public Vector2 Velocity {
            get { return Vector * Speed; }
        }

        public BoundingSphere Bounding
        {
            get
            {
                return new BoundingSphere(
                    new Vector3(X,Y,0), Radius);
            }
        }

        private Texture2D image;

        public Ball(Texture2D image) {
            this.image = image;
            Reset(Vector2.Zero, Vector2.Zero, 0);
        }

        public void Reset(Vector2 position, Vector2 vector, float speed) {
            this.vector = vector;
            this.position = position;
            this.speed = speed;
        }

        public void Update() {
            position += Velocity;
            if (position.Y < 0) {
                position.Y = 0;
            }
            else if (position.Y > 480) {
                position.Y = 480;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(image, position - new Vector2(radius), Color.White);
        }
    }
}
