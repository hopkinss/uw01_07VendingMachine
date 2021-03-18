using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vend.Lib
{
   
    public class CanRack : INotifyPropertyChanged
    {
        private int maxInventory;
        private ObservableCollection<CanInventory> cans;
        private int cansOfOrange;
        private int cansOfLemon;
        private int cansOfRegular;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CanRack(int inventory)
        {
            this.maxInventory = inventory;
            this.cans = new ObservableCollection<CanInventory>();
            FillTheCanRack();

        }

        public int MaxInventory
        {
            get { return maxInventory; }
            set { maxInventory = value; }
        }

        public int CansOfOrange
        {
            get { return cansOfOrange; }
            set
            {
                cansOfOrange = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<CanInventory> Cans
        {
            get { return cans; }
            set
            {
                cans = value;
                OnPropertyChanged();
            }
        }
        public int CansOfLemon
        {
            get { return cansOfLemon; }
            set
            {
                cansOfLemon = value;
                OnPropertyChanged();
            }
        }
        public int CansOfRegular
        {
            get { return cansOfRegular; }
            set
            {
                cansOfRegular = value;
                OnPropertyChanged();
            }
        }

        public void AddACanOf(Flavor flavor)
        {
            if(!IsFull(flavor))
            {
                var can = this.cans.Where(x => x.Can.Flavor == flavor).FirstOrDefault();
                if (can != null)
                {
                    can.Amount++;
                    SetCanCount();
                }
                else
                {
                    this.cans.Add(new CanInventory(1, new Can(flavor)));
                }
            }          
        }

        public void RemoveACanOf(Flavor flavor)
        {
            if (!IsEmpty(flavor))
            {
                var can = this.cans.Where(x => x.Can.Flavor == flavor).FirstOrDefault();
                can.Amount--;
                SetCanCount();
            }                     
        }

        public void SetCanCount()
        {
            CansOfLemon = this.cans.Where(x => x.Can.Flavor == Flavor.Lemon).Select(x =>x.Amount).FirstOrDefault();
            CansOfRegular= this.cans.Where(x => x.Can.Flavor == Flavor.Regular).Select(x => x.Amount).FirstOrDefault();
            CansOfOrange = this.cans.Where(x => x.Can.Flavor == Flavor.Orange).Select(x => x.Amount).FirstOrDefault();
        }

        public void FillTheCanRack()
        {
            foreach (Flavor f in FlavorOps.AllFlavors)
            {                
                while (!IsFull(f))
                {
                    AddACanOf(f);
                }
            }
            SetCanCount();
        }
        public void FillTheCanRack(int maxInventory)
        {
            this.maxInventory = maxInventory;
            foreach (Flavor f in FlavorOps.AllFlavors)
            {
                while (!IsFull(f))
                {
                    AddACanOf(f);
                }
            }
            SetCanCount();
        }

        public void EmptyCanRackOf(Flavor flavor)
        {
            var can = this.cans.Where(x => x.Can.Flavor == flavor).FirstOrDefault();
            can.Amount = 0;
        }

        public bool IsFull(Flavor flavor)
        {
            return this.cans.Where(x=> x.Can.Flavor==flavor).Count() >= this.maxInventory;
        }

        public bool IsEmpty(Flavor flavor)
        {
            return this.cans.Where(x => x.Can.Flavor == flavor).Count() == 0;
        }
    }

    public class CanInventory
    {
        public CanInventory(int amount,Can can)
        {
            Amount = amount;
            Can = can;
        }
        public int Amount { get; set; }
        public Can Can { get; set; }
    }
}
