using ChristmasPastryShop.Models.Cocktails.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Cocktails
{
    public abstract class Cocktail : ICocktail
    {
        private string name;
        private double price;
        private string size;

        public Cocktail(string cocktailName, string size, double price)
        {
            this.Name = cocktailName;
            this.Size = size;
            this.Price = price;
        }

        public string Name 
        {
            get => name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or whitespace!");
                }

                name = value;
            }
        }

        public string Size 
        { 
            get => size; 
            internal set 
            {
                size = value;
            } 
        }

        public double Price 
        {
            get => price;
            internal set
            {
                if (this.Size == "Small")
                {
                    price = value / 3;
                }
                else if (this.Size == "Middle")
                {
                    price = (value / 3) * 2;
                }
                else
                {
                    price = value;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Size}) - {this.Price:f2} lv";
        }
    }
}
