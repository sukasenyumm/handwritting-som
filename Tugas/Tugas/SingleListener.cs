using Leap;
using System.Collections.Generic;

namespace Tugas
{
    public class SingleListener : Listener
    {
        public override void OnInit(Controller cntrlr)
        {
            //Console.WriteLine("Initialized");
        }

        public override void OnConnect(Controller cntrlr)
        {
            //Console.WriteLine("Connected");
        }

        public override void OnDisconnect(Controller cntrlr)
        {
            //Console.WriteLine("Disconnected");
        }

        public override void OnExit(Controller cntrlr)
        {
            //Console.WriteLine("Exited");
        }

        private long currentTime;
        private long previousTime;
        private long timeChange;
        const int FramePause = 10000;
        public List<FingerPointStorage> fingerPoint = new List<FingerPointStorage>();

        public override void OnFrame(Controller cntrlr)
        {
            // Get the current frame.
            Frame currentFrame = cntrlr.Frame();

            currentTime = currentFrame.Timestamp;
            timeChange = currentTime - previousTime;

            if (timeChange > FramePause)
            {
                if (!currentFrame.Hands.IsEmpty)
                {
                    // Get the first finger in the list of fingers
                    Finger finger = currentFrame.Fingers[0];
                    // Get the closest screen intercepting a ray projecting from the finger
                    Screen screen = cntrlr.LocatedScreens.ClosestScreenHit(finger);

                    if (screen != null && screen.IsValid)
                    {
                        // Get the velocity of the finger tip
                        var tipVelocity = (int)finger.TipVelocity.Magnitude;
                        Hand hand = currentFrame.Hands.Frontmost;
                        // Use tipVelocity to reduce jitters when attempting to hold
                        // the cursor steady
                        //if (tipVelocity > 25)
                        if (tipVelocity > 1)
                        {
                            float xScreenIntersect = (float)screen.Intersect(finger, true).x;
                            float yScreenIntersect = (float)(1 - screen.Intersect(finger, true).y);
                            float zScreenIntersect = hand.PalmPosition.z;

                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                //var x = (int)(xScreenIntersect * screen.WidthPixels);
                                //var y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));

                                if (fingerPoint.Count <= 0)
                                {
                                    fingerPoint.Add(new FingerPointStorage(xScreenIntersect, yScreenIntersect, zScreenIntersect, 1));
                                }
                                else
                                {
                                    fingerPoint[0].g_X = xScreenIntersect;
                                    fingerPoint[0].g_Y = yScreenIntersect;
                                    fingerPoint[0].g_Z = zScreenIntersect;
                                    fingerPoint[0].isActive = true;
                                    //Console.WriteLine("leap x-axis: {0},y-axis: {1},z-axis: {2}", fingerPoint[0].g_X, fingerPoint[0].g_Y, fingerPoint[0].g_Z);
                                }
                            }
                        }
                    }
                }
                previousTime = currentTime;
            }
        }
    }
}
