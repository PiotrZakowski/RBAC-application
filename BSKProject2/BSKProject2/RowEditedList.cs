using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSKProject2
{
    /// <summary>
    /// Klasa dla listy, ktora rejestrowalaby edycje komorek poszczegolnych wierszy.
    /// </summary>
    class RowEditedList
    {
        public object primaryKey;
        public string primaryKeyColumnName;
        public int rowIndex;
        public List<int> columnIndexes;

        public RowEditedList()
        {
            columnIndexes = new List<int>();
        }

        public void decrementIndex()
        {
            this.rowIndex--;
        }
    }
}
