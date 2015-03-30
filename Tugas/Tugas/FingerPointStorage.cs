
namespace Tugas
{
    public class FingerPointStorage
    {
        public float g_X, g_Y, g_Z;
        public short numFing;
        public bool isActive;

        public FingerPointStorage(float x, float y, float z, short num_finger)
        {
            g_X = x;
            g_Y = y;
            g_Z = z;
            isActive = true;
            numFing = num_finger;
        }
    }
}
