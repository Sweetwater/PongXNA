using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongXNA
{
    class FPSCounter
    {
        private long elapsedTime;
        private int frameCount;
        private int fps;
        public int FPS {
            get { return fps; }
        }

        public FPSCounter() {
        }

        public void Update(long elapsedTime) {
            this.elapsedTime += elapsedTime;
            this.frameCount++;
            if (this.elapsedTime >= 1000)
            {
                this.fps = frameCount;
                this.frameCount = 0;
                this.elapsedTime = 0;
            }
        }
    }
}
