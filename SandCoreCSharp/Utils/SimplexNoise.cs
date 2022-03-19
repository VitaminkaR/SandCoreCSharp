using System;
using System.Collections.Generic;
using System.Text;

namespace SandCoreCSharp.Utils
{
    public static class SimplexNoise
    {
        const float F2 = 0.366025403f;
        const float G2 = 0.211324865f;



        private static byte[] perm;


        // устанавливает сид
        public static void CreateSeed(int seed)
        {
            perm = new byte[512];
            new Random(seed).NextBytes(perm);
        }


        public static float Noise(float x, float y)
        {
            int ix0, iy0, ix1, iy1;
            float fx0, fy0, fx1, fy1;
            float s, t, nx0, nx1, n0, n1;

            ix0 = FastFloor(x); // Integer part of x
            iy0 = FastFloor(y); // Integer part of y
            fx0 = x - ix0;        // Fractional part of x
            fy0 = y - iy0;        // Fractional part of y
            fx1 = fx0 - 1.0f;
            fy1 = fy0 - 1.0f;
            ix1 = (ix0 + 1) & 0xff;  // Wrap to 0..255
            iy1 = (iy0 + 1) & 0xff;
            ix0 = ix0 & 0xff;
            iy0 = iy0 & 0xff;

            t = Fade(fy0);
            s = Fade(fx0);

            nx0 = Grad(perm[ix0 + perm[iy0]], fx0, fy0);
            nx1 = Grad(perm[ix0 + perm[iy1]], fx0, fy1);
            n0 = Lerp(t, nx0, nx1);

            nx0 = Grad(perm[ix1 + perm[iy0]], fx1, fy0);
            nx1 = Grad(perm[ix1 + perm[iy1]], fx1, fy1);
            n1 = Lerp(t, nx0, nx1);

            return 0.507f * (Lerp(s, n0, n1));
        }

        public static float SNoise(float x, float y)
        {
            float n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            float s = (x + y) * F2; // Hairy factor for 2D
            float xs = x + s;
            float ys = y + s;
            int i = FastFloor(xs);
            int j = FastFloor(ys);

            float t = (float)(i + j) * G2;
            float X0 = i - t; // Unskew the cell origin back to (x,y) space
            float Y0 = j - t;
            float x0 = x - X0; // The x,y distances from the cell origin
            float y0 = y - Y0;

            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else { i1 = 0; j1 = 1; }      // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6

            float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
            float y2 = y0 - 1.0f + 2.0f * G2;

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            int ii = i & 0xff;
            int jj = j & 0xff;

            // Calculate the contribution from the three corners
            float t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Grad(perm[ii + perm[jj]], x0, y0);
            }

            float t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Grad(perm[ii + i1 + perm[jj + j1]], x1, y1);
            }

            float t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Grad(perm[ii + 1 + perm[jj + 1]], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 40.0f * (n0 + n1 + n2); // TODO: The scale factor is preliminary!
        }

        private static float Grad(int hash, float x, float y)
        {
            var h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }

        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float t, float a, float b) => a + t * (b - (a));

        private static int FastFloor(float x) => (x > 0) ? ((int)x) : ((int)x - 1);

        // возваращет высоты
        public static int[,] GetNoise(int w, int h, float s)
        {
            var values = new int[w, h];
            for (var i = 0; i < w; i++)
                for (var j = 0; j < h; j++)
                    values[i, j] = (int)(Math.Abs(SNoise(i * s, j * s)) * 20);
            return values;
        }
    }
}
