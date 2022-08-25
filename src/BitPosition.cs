namespace Operations
{
    public struct BitPosition
    {
        public BitPosition(int bytePosition, byte bitPosition)
        {
            this.bytePosition = bytePosition;
            this.bitPosition = bitPosition;
        }
        public int bytePosition;
        public byte bitPosition;
    }
}
