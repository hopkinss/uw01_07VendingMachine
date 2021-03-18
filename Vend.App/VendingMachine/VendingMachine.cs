using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vend.Lib;

namespace Vend.App.VendingMachine
{
    public class VendingMachine
    {

        private PurchasePrice purchasePrice;
        private CanRack canRack;
        private CoinBox trxBox;
        private CoinBox box;

        public VendingMachine(int inventory, dynamic price)
        {
            canRack = new CanRack(inventory);
            purchasePrice = new PurchasePrice(price);
            trxBox = new CoinBox();
            box = new CoinBox();
        }

        public CanRack CanRack
        {
            get { return canRack; }
            set
            {
                if (canRack != null)
                {
                    canRack = value;
                }
            }
        }

        public PurchasePrice PurchasePrice
        {
            get { return purchasePrice; }
            set { purchasePrice = value; }
        }

        // Coinbox for current transaction
        public CoinBox TrxBox
        {
            get { return trxBox; }
            set
            {
                trxBox = value;
            }
        }

        // Vending machines permanent coinbox
        public CoinBox Box
        {
            get { return box; }
            set { box = value; }
        }
        private decimal RefundCoins()
        {
            var refund = 0M;
            while (this.trxBox.ValueOf > 0)
            {
                var coin = this.trxBox.Box.LastOrDefault();
                refund += coin.ValueOf;
                trxBox.Withdraw(coin.CoinEnumeral);
            }
            return refund;
        }

        public bool IsAmountSufficient()
        {
            return trxBox.Box.Sum(x => x.ValueOf) >= purchasePrice.Price;
        }
    }
}
