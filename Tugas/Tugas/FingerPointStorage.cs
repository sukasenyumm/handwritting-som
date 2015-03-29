
namespace Tugas
{
    public class FingerPointStorage
    {
        public float g_X, g_Y;
        public short numFing;
        public bool isActive;

        public FingerPointStorage(float x, float y, short num_finger)
        {
            g_X = x;
            g_Y = y;
            isActive = true;
            numFing = num_finger;
        }
    }
}
