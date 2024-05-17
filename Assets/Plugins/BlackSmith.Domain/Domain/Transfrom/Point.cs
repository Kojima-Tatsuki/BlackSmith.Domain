namespace BlackSmith.Domain.Transfrom
{
    public class Point
    {
        private readonly float x;
        public float X => x;

        private readonly float y;
        public float Y => y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Width
    {
        private readonly float value;
        public float Value => value;

        public Width(float value)
        {
            this.value = value;
        }
    }

    public class Height
    {
        private readonly float value;
        public float Value => value;

        public Height(float value)
        {
            this.value = value;
        }
    }
}