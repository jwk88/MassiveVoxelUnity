namespace RideTools.Qube
{
    [System.Flags]
    public enum QubeFace : byte
    {
        None = 0,
        Top = 1 << 0,
        Bot = 1 << 1,
        Front = 1 << 2,
        Back = 1 << 3,
        Right = 1 << 4,
        Left = 1 << 5,
    }
}