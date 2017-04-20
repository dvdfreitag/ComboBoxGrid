using System;

namespace Controls
{
    public class CheckBoxGridCheckedEventArgs : EventArgs
    {
        public readonly int Column;
        public readonly int Row;
        public readonly bool IsChecked;

        public CheckBoxGridCheckedEventArgs(int column, int row, bool isChecked)
        {
            Column = column;
            Row = row;
            IsChecked = isChecked;
        }
    }
}
