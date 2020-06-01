using System;
using System.Collections.Generic;
using System.Text;

namespace ColdClearNET
{
    public class FieldConverter
    {
        public static bool[] Converter(int[,] intfield)
        {
            if (!(intfield.GetLength(0) == 40 && intfield.GetLength(1) == 10))
                throw new IndexOutOfRangeException("intfield is not int[40, 10]");

            bool[] boolfield = new bool[400];
            int count = 0;
            
            for (int i = 0; i < intfield.GetLength(0); i++)
            {
                for (int j = 0; j < intfield.GetLength(1); j++)
                {
                    boolfield[count] = intfield[i, j] > 0 ? true : false;
                    count++;
                }
            }

            return boolfield;
        }
    }
}
