using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PongXNA
{
    class KeyboardPaddle : Paddle
    {
        public KeyboardPaddle(Texture2D image, Vector2 direction, Vector2 position) :
            base(image, direction, position)
        {
        }

        public override void Update()
        {
            base.Update();
            KeyboardState newKeyState = Keyboard.GetState();
            if (newKeyState.IsKeyDown(Keys.Up))
            {
                MoveUp();
            }
            if (newKeyState.IsKeyDown(Keys.Down))
            {
                MoveDown();
            }
        }

    }
}
