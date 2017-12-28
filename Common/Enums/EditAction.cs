using System.Collections.Generic;

namespace Common.Enums
{
    public class EditAction
    {
        public static readonly EditAction NULL = new EditAction(0, "Null");
        public static readonly EditAction ADD = new EditAction(1, "Add");
        public static readonly EditAction UPDATE = new EditAction(2, "Update");

        public static IEnumerable<EditAction> Values
        {
            get
            {
                yield return NULL;
                yield return ADD;
                yield return UPDATE;
            }
        }

        private readonly int _id;
        private readonly string _description;

        EditAction(int id, string description)
        {
            _id = id;
            _description = description;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public static EditAction GetByDescription(string description)
        {
            foreach (EditAction entry in Values)
            {
                if (entry.Description.Contains(description))
                {
                    return entry;
                }
            }

            return NULL;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
