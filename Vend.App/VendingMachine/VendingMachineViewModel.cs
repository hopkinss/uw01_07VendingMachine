
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vend.Lib;

namespace Vend.App.VendingMachine
{
    public class VendingMachineViewModel : INotifyPropertyChanged
    {
        #region backing fields
        private VendingMachine vm = new VendingMachine(3, 55);
        private BitmapImage imgSoda;
        private bool canMakeChange;
        private string uiMessage;
        private CoinBox bank;
        private ObservableCollection<Coin> selectedCoins;
        #endregion

        #region constructor
        public VendingMachineViewModel()
        {
            Vm = vm;
            canMakeChange = true;
            SelectedCoins = new ObservableCollection<Coin>();

            // Represent financial institution - seed it with 20 random coins
            Bank = new CoinBox();
            foreach(var c in CoinBox.GenerateRandomCoins(20))
                Bank.Deposit(c);
        }

        #endregion

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region public Properties
        public VendingMachine Vm { get; set; }
        public CoinBox Bank
        {
            get { return bank; }
            set
            {
                bank = value;
                OnPropertyChanged("Bank");
            }
        }
        public ObservableCollection<Coin> SelectedCoins
        {
            get { return selectedCoins; }
            set
            {
                selectedCoins = value;
                OnPropertyChanged("SelectedCoins");
            }
        }
        public bool CanMakeChange
        {
            get { return canMakeChange; }
            set
            {
                canMakeChange = value;
                OnPropertyChanged("CanMakeChange");
            }
        }
        // Can dispensed to user as animation
        public BitmapImage ImgSoda
        {
            get => imgSoda;
            set
            {
                imgSoda = value;
                OnPropertyChanged("ImgSoda");
            }
        }
        // Message to display to user about transaction as animation
        public string UiMessage
        {
            get { return uiMessage; }
            set
            {
                uiMessage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand DepositCommand { get { return new RelayCommand(e => true, this.OnDeposit); } }
        public ICommand ReturnCoinsCommand
        {
            get
            {
                return new RelayCommand(e => vm.TrxBox.ValueOf > 0, this.OnReturnCoins);
            }
        }
        public ICommand EjectCanCommand { get { return new RelayCommand(e => true, this.OnEjectCan); } }
        public ICommand TransferToBankCommand 
        {
            get
            {
                return new RelayCommand(e => vm.Box.ValueOf > 0, this.OnTransferToBank);
            }
        }
        public ICommand TransferToVendCommand
        {
            get
            {
                return new RelayCommand(e => Bank.ValueOf > 0, this.OnTransferToVend);
            }
        }

        public ICommand FillCanRackCommand
        {
            get
            {
                return new RelayCommand(e => true, this.OnFillCanRack);
            }
        }

        public ICommand AddACanCommand
        {
            get
            {
                return new RelayCommand(e => true, this.OnAddACan);
            }
        }
        public ICommand RemoveACanCommand
        {
            get
            {
                return new RelayCommand(e => true, this.OnRemoveACan);
            }
        }
        #endregion

        #region private methods
        private void OnDeposit(object obj)
        {
            vm.TrxBox.Deposit(new Coin((Denomination)obj));
            CanMakeChange = vm.TrxBox.CanMakeChange(vm.PurchasePrice.Price);
        }


        private void OnEjectCan(object flavor)
        {
            if (this.canMakeChange)
            {
                if (vm.IsAmountSufficient())
                {
                    var f = (FlavorOps.ToFlavor(flavor.ToString()));
                    if (!vm.CanRack.IsEmpty(f))
                    {
                        vm.CanRack.RemoveACanOf(f);
                        vm.TrxBox.Transfer(vm.Box, vm.PurchasePrice.Price);
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri($"/Images/{flavor.ToString().ToLower()}.jpg", UriKind.Relative);
                        img.Rotation = Rotation.Rotate270;
                        img.EndInit();

                        this.ImgSoda = img;

                        var refundMsg = string.Empty;

                        if (vm.TrxBox.Box.Count > 0)
                        {
                            refundMsg = $" and {((Convert.ToInt32(RefundCoins()) * .01).ToString("C2"))} change";
                        }

                        this.UiMessage = $"Here is your {f} soda{refundMsg}";
                    }
                    else
                    {
                        this.UiMessage = $"Sorry, we are out of {f}";
                    }
                }
                else
                {
                    this.UiMessage = $"Please deposit an additional {((vm.PurchasePrice.Price - Convert.ToInt32(vm.TrxBox.ValueOf)) * .01).ToString("C2")}";
                }
            }
            else
            {
                this.UiMessage = $"Exact change required. Eject coins and try again";
            }
        }

        private void OnReturnCoins(object obj)
        {
            var refund = RefundCoins();
            this.UiMessage = $"Here's you refund of {(Convert.ToInt32(refund) * .01).ToString("C2")}";
            this.CanMakeChange = true;
        }

        private void OnTransferToBank(object isSelectAll)
        {
            var selectAll = bool.Parse(isSelectAll.ToString());

            var coins = selectAll ? vm.Box.Box.Select(x => x).ToList():
                                    vm.Box.Box.Where(x=>x.IsSelected).Select(x => x).ToList();

            foreach (var c in coins)
            {
                vm.Box.Withdraw(c.CoinEnumeral);
                Bank.Deposit(c);
            }
        }

        private void OnTransferToVend(object isSelectAll)
        {
            var selectAll = bool.Parse(isSelectAll.ToString());

            var coins = selectAll ? Bank.Box.Select(x => x).ToList() :
                                    Bank.Box.Where(x => x.IsSelected).Select(x => x).ToList();

            foreach (var c in coins)
            {
                Bank.Withdraw(c.CoinEnumeral);
                vm.Box.Deposit(c);
            }
        }

        private void OnAddACan(object flavor)
        {
            var f = (FlavorOps.ToFlavor(flavor.ToString()));
            if (vm.CanRack.Cans.Where(x=>x.Flavor == f).Count()< vm.CanRack.MaxInventory)
            {
                vm.CanRack.AddACanOf(f);
                this.UiMessage = $"A can of {f} soda was added to the vending machine";
            }
            else
            {
                this.UiMessage = $"{f} soda is full";
            }
        }
        private void OnRemoveACan(object flavor)
        {
            var f = (FlavorOps.ToFlavor(flavor.ToString()));
            if (vm.CanRack.Cans.Where(x => x.Flavor == f).Count() >0)
            {
                vm.CanRack.RemoveACanOf(f);
                this.UiMessage = $"A can of {f} soda was removed from the vending machine";
            }
            else
            {
                this.UiMessage = $"{f} soda is empty";
            }
        }

        private void OnFillCanRack(object obj)
        {
            var cans = vm.CanRack.FillTheCanRack();
            this.UiMessage = $"{cans} cans of soda stocked in the vending machine";
        }
        
        private bool HasMaxInventory(Flavor f)
        {
            return true;
        }
        private bool HasMaxInventory()
        {
            foreach(var f in FlavorOps.AllFlavors)
            {
                if (!vm.CanRack.IsFull(f)) { return true; }                    
            }
            return false;
        }

        private decimal RefundCoins()
        {
            var refund = 0M;
            while (vm.TrxBox.ValueOf > 0)
            {
                var coin = vm.TrxBox.Box.LastOrDefault();
                refund += coin.ValueOf;
                vm.TrxBox.Withdraw(coin.CoinEnumeral);
            }
            return refund;
        }
        #endregion
    }
}
