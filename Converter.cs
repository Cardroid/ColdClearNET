using System;
using System.Collections.Generic;
using System.Text;

namespace ColdClearNET
{
    public class Converter
    {
        public static bool[] FieldConverter(int[,] intfield)
        {
            bool[] boolfield = new bool[400];
            int count = 0;

            for (int j = 0; j < intfield.GetLength(1); j++)
            {
                for (int i = 0; i < intfield.GetLength(0); i++)
                {
                    boolfield[count++] = intfield[i, j] >= 0;
                }
            }

            return boolfield;
        }

        public static CCPiece PieceConverter(Piece pptpiece)
        {
            CCPiece ccPiece = CCPiece.CC_S;
            switch (pptpiece)
            {
                case Piece.S:
                    ccPiece = CCPiece.CC_S;
                    break;
                case Piece.Z:
                    ccPiece = CCPiece.CC_Z;
                    break;
                case Piece.J:
                    ccPiece = CCPiece.CC_J;
                    break;
                case Piece.L:
                    ccPiece = CCPiece.CC_L;
                    break;
                case Piece.T:
                    ccPiece = CCPiece.CC_T;
                    break;
                case Piece.O:
                    ccPiece = CCPiece.CC_O;
                    break;
                case Piece.I:
                    ccPiece = CCPiece.CC_I;
                    break;
            }
            return ccPiece;
        }
    }

    public enum Piece
    {
        S, Z, J, L, T, O, I
    }
}
