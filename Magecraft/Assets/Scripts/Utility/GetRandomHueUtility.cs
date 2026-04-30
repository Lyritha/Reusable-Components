public static class GetRandomHueUtility
{
    public static float GetFromId(int instanceID)
    {
        int x = instanceID;

        x = (x ^ 61) ^ (x >> 16);
        x += (x << 3);
        x ^= (x >> 4);
        x *= 0x27d4eb2d;
        x ^= (x >> 15);

        return (x & 0xFFFF) / 65535f;
    }
}
