using System;

namespace PerlinNoiseExample
{
    public struct Vector2
    {
        public double x, y;
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public static class PerlinNoise
    {
        // Ken Perlin's original permutation table
        private static readonly int[] permutation = {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
            140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
            247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
            57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
            74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
            60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
            65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
            200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
            52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
            207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
            119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
            129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
            218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
            81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
            184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
            222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        public static int Seed { get; set; } = 0;

        // Generates a pseudo-random number based on integer coordinates
        private static int Noise(int x, int y)
        {
            return permutation[(permutation[(y + Seed) % 256] + x) % 256];
        }

        // Linear interpolation
        public static double LinearInterpolation(double a, double b, double weight)
        {
            return (b - a) * weight + a;
        }

        // Cubic interpolation (for a smoother transition)
        public static double CubicInterpolation(double a, double b, double weight)
        {
            return (b - a) * (3.0 - weight * 2.0) * weight * weight + a;
        }

        // Generates a pseudo-random gradient vector based on grid coordinates
        public static Vector2 RandomGradient(int inputX, int inputY)
        {
            double random = 2920.0 * Math.Sin(inputX * 21942.0 + inputY * 171324.0 + 8912.0)
                * Math.Cos(inputX * 23157.0 * inputY * 217832.0 + 9758.0);
            return new Vector2(Math.Cos(random), Math.Sin(random));
        }

        // Computes the dot product between the gradient and the distance vector
        public static double DotGridGradient(int inputX, int inputY, double x, double y)
        {
            Vector2 gradient = RandomGradient(inputX, inputY);
            double dx = x - inputX;
            double dy = y - inputY;
            return dx * gradient.x + dy * gradient.y;
        }

        // Computes the Perlin noise at a given (x, y) coordinate
        public static double PerlinNoiseValue(double x, double y)
        {
            int xInt = (int)Math.Floor(x);
            int yInt = (int)Math.Floor(y);
            double xFrac = x - xInt;
            double yFrac = y - yInt;

            int s = Noise(xInt, yInt);
            int t = Noise(xInt + 1, yInt);
            int u = Noise(xInt, yInt + 1);
            int v = Noise(xInt + 1, yInt + 1);

            double low = LinearInterpolation(s, t, xFrac);
            double high = LinearInterpolation(u, v, xFrac);
            return LinearInterpolation(low, high, yFrac);
        }

        // Combines multiple layers of Perlin noise for increased detail
        public static double Perlin(double x, double y, double frequency, int depth)
        {
            double xa = x * frequency;
            double ya = y * frequency;
            double amplitude = 1.0;
            double finalValue = 0;
            double divisor = 0.0;

            for (int i = 0; i < depth; i++)
            {
                divisor += 256 * amplitude;
                finalValue += PerlinNoiseValue(xa, ya) * amplitude;
                amplitude /= 2;
                xa *= 2;
                ya *= 2;
            }

            return finalValue / divisor;
        }
    }
}
