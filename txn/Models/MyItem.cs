using System;

namespace txn.Models {
    public class MyItem {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTimeStamp { get; set; }
        public string Description { get; set; }
    }
}