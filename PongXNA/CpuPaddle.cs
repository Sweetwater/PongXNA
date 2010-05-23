using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PongXNA
{
    class CpuPaddle : Paddle
    {
        enum ThinkingPattern {
            LongRange,
            ShortRnage,
        }
        private ThinkingPattern thinkingPattern;

        private float changeDistance = 120;
        private float backlash = 6;
        private int predictionFrame = 60;

        private Ball ball;
        public Ball Ball
        {
            set { ball = value; }
        }


        public CpuPaddle(Texture2D image, Vector2 direction, Vector2 position) :
            base(image, direction, position)
        {
        }

        public override void Update()
        {
            base.Update();

            if (ball == null) return;

            float distance = Vector2.Distance(ball.Position, Position);
            if (distance > changeDistance)
            {
                thinkingPattern = ThinkingPattern.LongRange;
            }
            else {
                thinkingPattern = ThinkingPattern.ShortRnage;
            }

            switch (thinkingPattern)
            {
                case ThinkingPattern.LongRange:
                    ProcessLongRange();
                    break;
                case ThinkingPattern.ShortRnage:
                    ProcessShortRange();
                    break;
                default:
                    break;
            }
        }

        private void ProcessLongRange()
        {
            Vector2 predictionPos = ball.Position + ball.Velocity * predictionFrame;
            MoveTarget(predictionPos.Y);
        }

        private void ProcessShortRange()
        {
            MoveTarget(ball.Y);
        }

        private void MoveTarget(float y) {
            if (y < this.Y - backlash)
            {
                MoveUp();
            }
            else if (y > this.Y + backlash)
            {
                MoveDown();
            }
        }
    }
}
