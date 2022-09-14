namespace Operations
{
    public struct BitPosition
    {
        // Public fields
        public int BytePosition; // Byte position in byte array
        public byte BitInBytePosition;

        // Methods
        public BitPosition(int bytePosition, byte bitPosition)
        {
            BytePosition = bytePosition;
            BitInBytePosition = bitPosition;
        }
    }
}
