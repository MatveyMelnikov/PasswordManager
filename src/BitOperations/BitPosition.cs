namespace Operations
{
    public struct BitPosition
    {
        public BitPosition(int bytePosition, byte bitPosition)
        {
            BytePosition = bytePosition;
            BitInBytePosition = bitPosition;
        }
        public int BytePosition;
        public byte BitInBytePosition;
    }
}
