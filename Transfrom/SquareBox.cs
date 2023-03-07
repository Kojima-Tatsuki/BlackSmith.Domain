using System;

namespace BlackSmith.Domain.Transfrom
{
    public class Squarebox
    {
        private readonly Point center;
        public Point Center => center;

        private readonly Width width;
        public Width Width => width;
        private readonly Height height;
        public Height Height => height;

        public Squarebox(Point center, Width w, Height h)
        {
            if (center is null) throw new ArgumentNullException(nameof(center));
            if (w is null) throw new ArgumentNullException(nameof(w));
            if (h is null) throw new ArgumentNullException(nameof(h));

            this.center = center;
            this.width = w;
            this.height = h;
        }
    }
}