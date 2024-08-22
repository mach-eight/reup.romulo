using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class LinearAlgebraUtils
    {
        public static float[,] RREF(float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int lead = 0;

            for (int r = 0; r < rows; r++)
            {
                if (lead >= cols)
                {
                    break;
                }

                int i = r;
                while (matrix[i, lead] == 0)
                {
                    i++;
                    if (i == rows)
                    {
                        i = r;
                        lead++;
                        if (lead == cols)
                        {
                            return matrix;
                        }
                    }
                }

                // Swap rows i and r
                for (int j = 0; j < cols; j++)
                {
                    float temp = matrix[r, j];
                    matrix[r, j] = matrix[i, j];
                    matrix[i, j] = temp;
                }

                // Normalize the pivot row
                float pivotValue = matrix[r, lead];
                for (int j = 0; j < cols; j++)
                {
                    matrix[r, j] /= pivotValue;
                }

                // Make all rows above and below the pivot zero in the current column
                for (int iRow = 0; iRow < rows; iRow++)
                {
                    if (iRow != r)
                    {
                        float factor = matrix[iRow, lead];
                        for (int j = 0; j < cols; j++)
                        {
                            matrix[iRow, j] -= factor * matrix[r, j];
                        }
                    }
                }
                lead++;
            }

            return matrix;
        }

        public static void PrintMatrix(float[,] matrix)
        {
            string result = "";

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    //Debug.Log(rrefMatrix[i, j] + " ");
                    result = result + matrix[i, j] + " ";
                }
                result = result + "\n";
            }
            Debug.Log(result);
        }

    }
}
