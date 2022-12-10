using ChristmasPastryShop.Core.Contracts;
using ChristmasPastryShop.Models.Booths;
using ChristmasPastryShop.Models.Cocktails;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChristmasPastryShop.Core
{
    public class Controller : IController
    {
        private BoothRepository booths;

        public Controller()
        {
            booths = new BoothRepository();
        }

        public string AddBooth(int capacity)
        {
            var count = this.booths.Models.Count + 1;
            var booth = new Booth(count, capacity);
            booths.AddModel(booth);
            return $"Added booth number {booth.BoothId} with capacity {capacity} in the pastry shop!";
        }

        public string AddCocktail(int boothId, string cocktailTypeName, string cocktailName, string size)
        {
            if (cocktailTypeName != "Hibernation" && cocktailTypeName != "MulledWine")
            {
                return $"Cocktail type {cocktailTypeName} is not supported in our application!";
            }
            if (size != "Small" && size != "Middle" && size != "Large")
            {
                return $"{size} is not recognized as valid cocktail size!";
            }

            ICocktail cocktail;
            switch (cocktailTypeName)
            {
                case "Hibernation":
                    cocktail = new Hibernation(cocktailName, size);
                    if (booths.Models.FirstOrDefault(m => m.BoothId == boothId).CocktailMenu.Models.Contains(cocktail))
                    {
                        return $"{size} {cocktailName} is already added in the pastry shop!";
                    }
                    booths.Models.FirstOrDefault(m => m.BoothId == boothId).CocktailMenu.AddModel(cocktail);
                    break;
                case "MulledWine":
                    cocktail = new MulledWine(cocktailName, size);
                    if (booths.Models.FirstOrDefault(m => m.BoothId == boothId).CocktailMenu.Models.Contains(cocktail))
                    {
                        return $"{size} {cocktailName} is already added in the pastry shop!";
                    }
                    booths.Models.FirstOrDefault(m => m.BoothId == boothId).CocktailMenu.AddModel(cocktail);
                    break;
            }

            return $"{size} {cocktailName} {cocktailTypeName} added to the pastry shop!";
        }

        public string AddDelicacy(int boothId, string delicacyTypeName, string delicacyName)
        {
            if (delicacyTypeName != "Gingerbread" && delicacyTypeName != "Stolen")
            {
                return $"Delicacy type {delicacyTypeName} is not supported in our application!";
            }
            if (booths.Models.Any(d => d.DelicacyMenu.Models.Any(d => d.Name == delicacyName)))
            {
                return $"{delicacyName} is already added in the pastry shop!";
            }

            IDelicacy delicacy;
            switch (delicacyTypeName)
            {
                case "Gingerbread":
                    delicacy = new Gingerbread(delicacyName);
                    booths.Models.FirstOrDefault(m => m.BoothId == boothId).DelicacyMenu.AddModel(delicacy);
                    break;
                case "Stolen":
                    delicacy = new Stolen(delicacyName);
                    booths.Models.FirstOrDefault(m => m.BoothId == boothId).DelicacyMenu.AddModel(delicacy);
                    break;
            }

            return $"{delicacyTypeName} {delicacyName} added to the pastry shop!";
        }

        public string BoothReport(int boothId)
        {
            var booth = booths.Models.First(b => b.BoothId == boothId);

            return booth.ToString();
        }

        public string LeaveBooth(int boothId)
        {
            var booth = booths.Models.First(b => b.BoothId == boothId);
            var result = booth.Turnover + booth.CurrentBill;
            booth.Charge();
            booth.ChangeStatus();

            return $"Bill {result:f2} lv" + Environment.NewLine + $"Booth {boothId} is now available!";
        }

        public string ReserveBooth(int countOfPeople)
        {
            var ordered = booths.Models.Where(b => !b.IsReserved && b.Capacity >= countOfPeople)
                .OrderBy(b => b.Capacity).ThenByDescending(b => b.BoothId).ToList();
            
            if (ordered.Count == 0)
            {
                return $"No available booth for {countOfPeople} people!";
            }

            var first = (Booth)ordered[0];
            first.IsReserved = true;
            return $"Booth {first.BoothId} has been reserved for {countOfPeople} people!";
        }

        public string TryOrder(int boothId, string order)
        {
            var split = order.Split("/");

            var itemTypeName = split[0];

            if (itemTypeName != "Stolen" && itemTypeName != "Gingerbread" && itemTypeName != "MulledWine" &&
                itemTypeName != "Hibernation")
            {
                return $"{itemTypeName} is not recognized type!";
            }

            var itemName = split[1];
            var orderedPiecesCount = int.Parse(split[2]);
            string sizeCocktail = "";
            if (itemTypeName == "Cocktail" || itemTypeName == "Hibernation" || itemTypeName == "MulledWine")
            {
                sizeCocktail = split[3];
            }

            var booth = booths.Models.First(b => b.BoothId == boothId);

            if (itemTypeName == "Cocktail" || itemTypeName == "MulledWine" || itemTypeName == "Hibernation")
            {
                if (booth.CocktailMenu.Models.Any(c => c.GetType().Name == itemTypeName && c.Name == itemName && c.Size == sizeCocktail))
                {
                    var item = booth.CocktailMenu.Models.First(c => c.GetType().Name == itemTypeName && c.Name == itemName && c.Size == sizeCocktail);
                    ((Booth)booth).CurrentBill += item.Price * orderedPiecesCount;
                    return $"Booth {boothId} ordered {orderedPiecesCount} {itemName}!";
                }
                else
                {
                    return $"There is no {sizeCocktail} {itemName} available!";
                }
            }
            if (itemTypeName == "Delicacy" || itemTypeName == "Gingerbread" || itemTypeName == "Stolen")
            {
                if (booth.DelicacyMenu.Models.Any(c => c.GetType().Name == itemTypeName && c.Name == itemName))
                {
                    var item = booth.DelicacyMenu.Models.First(c => c.GetType().Name == itemTypeName && c.Name == itemName);
                    ((Booth)booth).CurrentBill += item.Price * orderedPiecesCount;
                    return $"Booth {boothId} ordered {orderedPiecesCount} {itemName}!";
                }
                else
                {
                    return $"There is no {itemTypeName} {itemName} available!";
                }
            }

            return $"{itemTypeName} is not recognized type!";
        }
    }
}
