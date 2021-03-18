using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vend.App
{
    public class PurchasePrice : INotifyPropertyChanged
    {
        private int price;

        public PurchasePrice(int initialPrice)
        {
            this.price = initialPrice;
        }

        public int Price
        {
            get
            {
                return price;
            }
            set 
            { 
                price = value;
                OnPropertyChanged("Price");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
