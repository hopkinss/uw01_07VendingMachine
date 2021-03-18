using System;
using System.Collections.Generic;
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
        private List<Can> cans;
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
            this.cans = new List<Can>();
            FillTheCanRack();
        }

        public int FillTheCanRack()
        {
            int numCans = 0;
            foreach (Flavor f in FlavorOps.AllFlavors)
            {
                while (!IsFull(f))
                {
                    numCans++;
                    AddACanOf(f);
                }
            }
            SetCanCount();
            return numCans;
        }
        public int MaxInventory
        {
            get { return maxInventory; }
            set 
            { 
                maxInventory = value;
                OnPropertyChanged("MaxInventory");
            }
        }

        public List<Can> Cans
        {
            get { return cans; }
            set
            {
                cans = value;
                OnPropertyChanged();
            }
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
            if (!IsFull(flavor))
            {
                this.cans.Add(new Can(flavor));
                SetCanCount();
            }
        }

        public void RemoveACanOf(Flavor flavor)
        {

            if (!IsEmpty(flavor))
            {
                var c = this.cans.Where(x => x.Flavor == flavor).FirstOrDefault();
                this.cans.Remove(c);
                SetCanCount();
            }            
        }

        public void SetCanCount()
        {
            CansOfLemon = this.cans.Where(x => x.Flavor == Flavor.Lemon).Count();
            CansOfRegular = this.cans.Where(x => x.Flavor == Flavor.Regular).Count();
            CansOfOrange = this.cans.Where(x => x.Flavor == Flavor.Orange).Count();
        }



        public void EmptyCanRackOf(Flavor flavor)
        {
            if (!IsEmpty(flavor))
            {
                var can = this.cans.Where(x => x.Flavor == flavor).FirstOrDefault();
                this.cans.Remove(can);
            }
            
        }

        public bool IsFull(Flavor flavor)
        {
            return this.cans.Where(x => x.Flavor == flavor).Count() >= this.maxInventory;
        }
        

        public bool IsEmpty(Flavor flavor)
        {
            return this.cans.Where(x => x.Flavor == flavor).Count() == 0;
        }
    }
}
