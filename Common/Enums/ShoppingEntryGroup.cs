using System;
using System.Collections.Generic;

namespace Common.Enums
{
    public class ShoppingEntryGroup
    {
        public static readonly ShoppingEntryGroup OTHER = new ShoppingEntryGroup(0, "Other", new Uri("/Common;component/Assets/Icons/Shopping/shopping.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup FRUIT = new ShoppingEntryGroup(1, "Fruit", new Uri("/Common;component/Assets/Icons/Shopping/fruits.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup VEGETABLE = new ShoppingEntryGroup(2, "Vegetable", new Uri("/Common;component/Assets/Icons/Shopping/vegetables.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup SALAD = new ShoppingEntryGroup(3, "Salad", new Uri("/Common;component/Assets/Icons/Shopping/salad.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup DRINKS = new ShoppingEntryGroup(4, "Drinks", new Uri("/Common;component/Assets/Icons/Shopping/drinks.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup OIL_VINEGAR = new ShoppingEntryGroup(5, "Oil_and_vinegar", new Uri("/Common;component/Assets/Icons/Shopping/oil_vinegar.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup NOODLES = new ShoppingEntryGroup(6, "Noodles", new Uri("/Common;component/Assets/Icons/Shopping/noodles.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup BREAD = new ShoppingEntryGroup(7, "Bread", new Uri("/Common;component/Assets/Icons/Shopping/bread.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup SPREAD = new ShoppingEntryGroup(8, "Spread", new Uri("/Common;component/Assets/Icons/Shopping/spread.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup MILK_PRODUCT = new ShoppingEntryGroup(9, "Milk_product", new Uri("/Common;component/Assets/Icons/Shopping/milk.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup CANDY = new ShoppingEntryGroup(10, "Candy", new Uri("/Common;component/Assets/Icons/Shopping/candy.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup CLEANING_MATERIAL = new ShoppingEntryGroup(11, "Cleaning_material", new Uri("/Common;component/Assets/Icons/Shopping/cleaning.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup BATH_UTILITY = new ShoppingEntryGroup(12, "Bath_utility", new Uri("/Common;component/Assets/Icons/Shopping/bath.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup LEISURE = new ShoppingEntryGroup(13, "Leisure", new Uri("/Common;component/Assets/Icons/Shopping/leisure.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup BAKING = new ShoppingEntryGroup(14, "Baking", new Uri("/Common;component/Assets/Icons/Shopping/baking.png", UriKind.Relative));
        public static readonly ShoppingEntryGroup CEREALS = new ShoppingEntryGroup(15, "Cereals", new Uri("/Common;component/Assets/Icons/Shopping/cereals.png", UriKind.Relative));

        public static IEnumerable<ShoppingEntryGroup> Values
        {
            get
            {
                yield return OTHER;
                yield return FRUIT;
                yield return VEGETABLE;
                yield return SALAD;
                yield return DRINKS;
                yield return OIL_VINEGAR;
                yield return NOODLES;
                yield return BREAD;
                yield return SPREAD;
                yield return MILK_PRODUCT;
                yield return CANDY;
                yield return CLEANING_MATERIAL;
                yield return BATH_UTILITY;
                yield return LEISURE;
                yield return BAKING;
                yield return CEREALS;
            }
        }

        private readonly int _id;
        private readonly string _description;
        private readonly Uri _icon;

        ShoppingEntryGroup(int id, string description, Uri icon)
        {
            _id = id;
            _description = description;
            _icon = icon;
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

        public Uri Icon
        {
            get
            {
                return _icon;
            }
        }

        public static ShoppingEntryGroup GetByDescription(string description)
        {
            foreach (ShoppingEntryGroup entry in Values)
            {
                if (entry.Description.Contains(description))
                {
                    return entry;
                }
            }

            return OTHER;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
