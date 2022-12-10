﻿using ChristmasPastryShop.Models.Delicacies.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Delicacies
{
    public abstract class Delicacy : IDelicacy
    {
        private string name;

        public Delicacy(string delicacyName, double price)
        {
            this.Name = delicacyName;
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

        public double Price { get; private set; }

        public override string ToString()
        {
            return $"{this.Name} - {this.Price:f2} lv";
        }
    }
}