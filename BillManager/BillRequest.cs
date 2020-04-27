using System;
using System.Collections.Generic;
using System.Text;

namespace BillManager
{
    class BillRequest
    {
        private List<ItemLine> itemLines;
        private User user;

        public BillRequest(List<ItemLine> itemLines, User user)
        {
            this.itemLines = itemLines;
            this.User = user;
        }

        public List<ItemLine> ItemLines { get => itemLines; set => itemLines = value; }
        public User User { get => user; set => user = value; }
    }
}
