using System;
namespace RandomiaGaming.Helpers
{
    public static class ArrayHelper
    {
        public static T[] MergeArrays<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA is null)
            {
                throw new NullReferenceException("Merge operation was aborted because the given data for a was null.");
            }
            if (arrayB is null)
            {
                throw new NullReferenceException("Merge operation was aborted because the given data for b was null.");
            }
            try
            {
                if (arrayA.Length == 0)
                {
                    return arrayB;
                }
                else if (arrayB.Length == 0)
                {
                    return arrayA;
                }
                else
                {
                    T[] output = new T[arrayA.Length + arrayB.Length];
                    Array.Copy(arrayA, 0, output, 0, arrayA.Length);
                    Array.Copy(arrayB, 0, output, arrayA.Length, arrayB.Length);
                    return output;
                }
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Merge operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Merge operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static T[] GetRangeFromArray<T>(T[] array, int startIndex, int length)
        {
            if (array is null)
            {
                throw new NullReferenceException("Merge operation was aborted because the given array was null.");
            }

            if (startIndex < 0 || startIndex >= array.Length)
            {
                throw new Exception("Start index was outside the bounds of the array.");
            }

            if(length < 0)
            {
                throw new ArgumentException("Length was negative.");
            }

            if (array.Length < startIndex + length)
            {
                throw new Exception("Spcified range was too large to fit within the bounds of the array.");
            }

            try
            {
                T[] output = new T[length];
                Array.Copy(array, startIndex, output, 0, length);
                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Range operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Range operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static bool ArraysEqual<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA is null && arrayB is null)
            {
                return true;
            }
            else if (arrayA is null || arrayB is null)
            {
                return false;
            }
            else if (arrayA.Length != arrayB.Length)
            {
                return false;
            }
            try
            {
                for (int i = 0; i < arrayA.Length; i++)
                {
                    if (!arrayA[i].Equals(arrayB[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Equals operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Equals operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
        public static T[] CloneArray<T>(T[] array)
        {
            if (array is null)
            {
                throw new NullReferenceException("Clone operation was aborted because the given array was null.");
            }
            try
            {
                T[] output = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    output[i] = array[i];
                }
                return output;
            }
            catch (Exception ex)
            {
                Exception rethrowException;
                try
                {
                    rethrowException = new Exception($"Clone operation was aborted because a \"{ex.GetType().FullName}\" was thrown: \"{ex.Message}\".");
                }
                catch
                {
                    rethrowException = new Exception("Clone operation was aborted because an unknown exception was thrown.");
                }
                throw rethrowException;
            }
        }
    }
}
