using System.CodeDom;

namespace ToolsCORE
{
    public static class ArrayTools
    {
        public static Int32[] CoordinatesOf<T>(this T[,] matrix, T value)
        {
            Int32 w = matrix.GetLength(0); // width
            Int32 h = matrix.GetLength(1); // height

            for (Int32 x = 0; x < w; ++x)
            {
                for (Int32 y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return [x, y];
                }
            }

            return [-1, -1];
        }

        public static List<Int32[]> CompareDiff<T>(this T[,] matrix1, T[,] matrix2)
        {
            Int32 w1 = matrix1.GetLength(0); // width
            Int32 h1 = matrix1.GetLength(1); // height

            Int32 w2 = matrix2.GetLength(0); // width
            Int32 h2 = matrix2.GetLength(1); // height

            if (w1 == w2 && h1 == h2)
            {
                List<Int32[]> diffs = [];
                for (Int32 x = 0; x < w1; ++x)
                {
                    for (Int32 y = 0; y < h1; ++y)
                    {
                        //Console.WriteLine(matrix1[x, y] + "=" + matrix2[x, y]);
                        if (!matrix1[x, y].Equals(matrix2[x, y]))
                            diffs.Add([x,y]);
                    }
                }
                return diffs;
            }
            else return [[-1, -1]];

        }
        public static Boolean BoardContainsCount(this Byte[,] matrix, Byte item, Int16 count)
        {
            if (item == 0 || item == 100 || item == 200 || item == 255) return false;
            Byte contains = 0;
            foreach (byte element in matrix)
            {
                if (element == item) contains++;
            }
            return contains > count;
        }

        public static Boolean Contains<T>(this T[,] matrix, T item)
        {
            Boolean contains = false;
            foreach (T element in matrix)
            {
                if (element.Equals(item)) contains = true;
            }
            return contains;
        }

        public static List<Int32> IndizesOf<T>(this List<T> array, T item)
        {
            Int32 l = array.Count; // length

            List<Int32> indizes = [];
            for (Int32 y = 0; y < l; ++y)
            {
                if (array[y].Equals(item))
                    indizes.Add(y);
            }

            if (indizes.Count > 0) return indizes;


            else return [];
        }
    }
}