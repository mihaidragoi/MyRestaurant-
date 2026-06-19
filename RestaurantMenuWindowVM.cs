using RestaurantIncercareaDoua.Data_Access_Layer;
using RestaurantIncercareaDoua.Model;
using RestaurantIncercareaDoua.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data; 

namespace RestaurantIncercareaDoua.ViewModel
{
    internal class RestaurantMenuWindowVM : INotifyPropertyChanged
    {
        private MenuDataAccess menuDataAccess = new MenuDataAccess();

        private List<Order> allOrdersFromDB;

        public ObservableCollection<Order> IstoricComenzi { get; set; } = new ObservableCollection<Order>();
        public ObservableCollection<Preparat> ListaPreparate { get; set; }
        public ObservableCollection<Preparat> ListaAdminPreparate { get; set; } = new ObservableCollection<Preparat>();
        public ObservableCollection<string> PreparateEpuizate { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CartItem> CartItems { get; set; } = new ObservableCollection<CartItem>();

        private List<Preparat> toatePreparateleOriginale = new List<Preparat>();

        public ICollectionView ViewMeniu { get; set; }

        public int NewIdCategorie { get; set; } = 1;
        public string NewDenumire { get; set; }
        public decimal NewPret { get; set; }
        public decimal NewCantitatePortie { get; set; }
        public decimal NewCantitateTotala { get; set; }
        public string NewImaginePath { get; set; } = "";

        public List<KeyValuePair<int, string>> CategoriiDisponibile { get; set; } = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(1, "Pizza"),
            new KeyValuePair<int, string>(2, "Paste"),
            new KeyValuePair<int, string>(3, "Băuturi"),
            new KeyValuePair<int, string>(4, "Deserturi"),
            new KeyValuePair<int, string>(5, "Supe / Ciorbe")
        };

        private string textCautare = "";
        public string TextCautare
        {
            get => textCautare;
            set
            {
                textCautare = value;
                OnPropertyChanged(nameof(TextCautare));
                AplicaFiltrareAvansata();
            }
        }

        private bool cautaDupaAlergen = false;
        public bool CautaDupaAlergen
        {
            get => cautaDupaAlergen;
            set
            {
                cautaDupaAlergen = value;
                OnPropertyChanged(nameof(CautaDupaAlergen));
                AplicaFiltrareAvansata();
            }
        }

        private string regulaCautare = "Conține";
        public string RegulaCautare
        {
            get => regulaCautare;
            set
            {
                regulaCautare = value;
                OnPropertyChanged(nameof(RegulaCautare));
                AplicaFiltrareAvansata();
            }
        }

        public List<string> OptiuniRegula { get; set; } = new List<string> { "Conține", "Nu conține" };

        private bool onlyActiveOrders;
        public bool OnlyActiveOrders
        {
            get => onlyActiveOrders;
            set
            {
                onlyActiveOrders = value;
                OnPropertyChanged(nameof(OnlyActiveOrders));
                FiltreazaComenzile();
            }
        }

        public int CurentUserId { get; private set; }
        public UserType CurrentRole { get; private set; }

        public bool IsGuest => CurrentRole == UserType.Guest;
        public bool IsClient => CurrentRole == UserType.Client;
        public bool IsAngajat => CurrentRole == UserType.Angajat;

        public bool CanOrder => IsClient || IsAngajat;
        public bool CanSeeProfile => CurrentRole != UserType.Guest;

        public RestaurantMenuWindowVM(int idLogat = -1, UserType rol = UserType.Guest)
        {
            CurentUserId = idLogat;
            CurrentRole = rol;

            ListaPreparate = new ObservableCollection<Preparat>();

            IncarcareMeniu();
            IncarcaDateSpecificeRolului();

            AddToCartCommand = new RelayCommand(AddToCartImpl, CanAdd);
            UpdateDataCommand = new RelayCommand(UpdateDataImpl, CanUpdate);
            ViewCartCommand = new RelayCommand(ViewCartImpl, CanViewCart);
            ChangeStatusCommand = new RelayCommand(ChangeStatusImpl, CanChangeStatus);
            SeeDetailsOrderCommand = new RelayCommand(SeeDetailsOrderImpl, CanSeeDetails);

            UpdatePreparatCommand = new RelayCommand(UpdatePreparatImpl, CanAdminExecute);
            AdaugaPreparatCommand = new RelayCommand(AdaugaPreparatImpl, CanAdminExecute);
            StergePreparatCommand = new RelayCommand(StergePreparatImpl, CanAdminExecute);
            AnuleazaComandaCommand = new RelayCommand(AnuleazaComandaImpl, CanAnuleaza);
        }

        private void IncarcaDateSpecificeRolului()
        {
            var orderDAL = new OrderDataAccess();

            if (IsClient)
            {
                var comenzi = orderDAL.GetIstoricComenzi(CurentUserId);

                if (IstoricComenzi == null)
                {
                    IstoricComenzi = new ObservableCollection<Order>();
                }

                IstoricComenzi.Clear();
                foreach (var comanda in comenzi)
                {
                    IstoricComenzi.Add(comanda);
                }

                OnPropertyChanged(nameof(IstoricComenzi));
            }
            else if (IsAngajat)
            {
                allOrdersFromDB = orderDAL.GetToateComenzile();

                if (IstoricComenzi == null) IstoricComenzi = new ObservableCollection<Order>();

                IstoricComenzi.Clear();
                foreach (var c in allOrdersFromDB)
                {
                    IstoricComenzi.Add(c);
                }

                int limitaStoc = int.Parse(System.Configuration.ConfigurationManager.AppSettings["OutOfStockThreshold"] ?? "10");
                var listaEpuizate = menuDataAccess.GetPreparateEpuizate(limitaStoc);
                PreparateEpuizate.Clear();
                foreach (var item in listaEpuizate)
                {
                    PreparateEpuizate.Add(item);
                }

                FiltreazaComenzile();
            }

            if (!IsGuest)
            {
                var userDAL = new UserDataAccess();
                var user = userDAL.GetUserById(CurentUserId);

                if (user != null)
                {
                    UserEmail = user.userEmail;
                    UserPhone = user.userPhone;
                    UserAddress = user.userAddress;
                    UserPassword = user.userPassword;
                }
            }
        }

        private void IncarcareMeniu()
        {
            var preparate = menuDataAccess.GetAllPreparate();
            var meniuri = menuDataAccess.GetAllMeniuri();

            ListaAdminPreparate.Clear();
            foreach (var p in preparate)
            {
                ListaAdminPreparate.Add(p);
            }

            toatePreparateleOriginale.Clear();
            foreach (var p in preparate) { toatePreparateleOriginale.Add(p); }
            foreach (var m in meniuri) { toatePreparateleOriginale.Add(m); }

            ViewMeniu = CollectionViewSource.GetDefaultView(ListaPreparate);

            if (ViewMeniu is IEditableCollectionView editableView)
            {
                if (editableView.IsEditingItem || editableView.IsAddingNew)
                {
                    AplicaFiltrareAvansata();
                    return;
                }
            }

            using (ViewMeniu.DeferRefresh())
            {
                ViewMeniu.GroupDescriptions.Clear();
                ViewMeniu.GroupDescriptions.Add(new PropertyGroupDescription("NumeCategorie"));
            }

            AplicaFiltrareAvansata();
        }

        private void AplicaFiltrareAvansata()
        {
            if (toatePreparateleOriginale == null) return;

            IEnumerable<Preparat> colectieFiltrata = toatePreparateleOriginale;

            if (!string.IsNullOrWhiteSpace(TextCautare))
            {
                string keyword = TextCautare.Trim().ToLower();
                bool regulaContine = RegulaCautare == "Conține";

                if (CautaDupaAlergen)
                {
                    if (regulaContine)
                        colectieFiltrata = colectieFiltrata.Where(p => p.Alergeni != null && p.Alergeni.Any(a => a.ToLower().Contains(keyword)));
                    else
                        colectieFiltrata = colectieFiltrata.Where(p => p.Alergeni == null || !p.Alergeni.Any(a => a.ToLower().Contains(keyword)));
                }
                else
                {
                    if (regulaContine)
                        colectieFiltrata = colectieFiltrata.Where(p => p.Denumire != null && p.Denumire.ToLower().Contains(keyword));
                    else
                        colectieFiltrata = colectieFiltrata.Where(p => p.Denumire == null || !p.Denumire.ToLower().Contains(keyword));
                }
            }

            ListaPreparate.Clear();
            foreach (var prep in colectieFiltrata)
            {
                ListaPreparate.Add(prep);
            }

            ViewMeniu?.Refresh();
        }

        public ICommand AddToCartCommand { get; set; }
        public ICommand UpdateDataCommand { get; set; }
        public ICommand ViewCartCommand { get; set; }
        public ICommand ChangeStatusCommand { get; set; }
        public ICommand SeeDetailsOrderCommand { get; set; }
        public ICommand UpdatePreparatCommand { get; set; }
        public ICommand AdaugaPreparatCommand { get; set; }
        public ICommand StergePreparatCommand { get; set; }
        public ICommand AnuleazaComandaCommand { get; set; }

        private bool CanAdd(object parameter) => parameter is Preparat p && CanOrder && p.IsDisponibil;

        private void AddToCartImpl(object parameter)
        {
            Preparat p = (Preparat)parameter;

            int cantitateCurentaInCos = CartItems.Where(i => i.product.Id == p.Id && i.product.IsMeniu == p.IsMeniu).Sum(i => i.Quantity);

            if (cantitateCurentaInCos >= p.CantitateTotala)
            {
                MessageBox.Show($"Stoc epuizat! Ai atins limita de {p.CantitateTotala} porții.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in CartItems)
            {
                if (item.product.Id == p.Id && item.product.IsMeniu == p.IsMeniu)
                {
                    item.Quantity++;
                    return;
                }
            }

            CartItems.Add(new CartItem(p, 1));
        }

        private bool CanUpdate(object parameter) => true;

        private string userAddress;
        public string UserAddress
        {
            get => userAddress;
            set { userAddress = value; OnPropertyChanged(nameof(UserAddress)); }
        }

        private string userPhone;
        public string UserPhone
        {
            get => userPhone;
            set { userPhone = value; OnPropertyChanged(nameof(UserPhone)); }
        }

        private string userPassword;
        public string UserPassword
        {
            get => userPassword;
            set { userPassword = value; OnPropertyChanged(nameof(UserPassword)); }
        }

        private string userEmail;
        public string UserEmail
        {
            get => userEmail;
            set { userEmail = value; OnPropertyChanged(nameof(UserEmail)); }
        }

        private void UpdateDataImpl(object parameter)
        {
            var userDAL = new UserDataAccess();
            string rezultat = userDAL.UpdateDateUtilizator(CurentUserId, UserPhone, UserAddress, UserPassword);

            if (rezultat == "Succes")
                MessageBox.Show("Datele au fost salvate cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Eroare la salvarea datelor.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool CanViewCart(object parameter) => CanOrder;

        private bool CanChangeStatus(object parameter) => IsAngajat;

        private void ChangeStatusImpl(object parameter)
        {
            var comandaSelectata = parameter as Order;
            if (comandaSelectata != null)
            {
                if (comandaSelectata.Status == "Livrata" || comandaSelectata.Status == "Anulata" || comandaSelectata.Status == "Finalizata")
                {
                    MessageBox.Show("Această comandă este deja finalizată/anulată și nu mai poate fi modificată!", "Restricție", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string noulStatus = "In curs de livrare";

                if (comandaSelectata.Status == "Inregistrata") noulStatus = "In curs de preparare";
                else if (comandaSelectata.Status == "In curs de preparare") noulStatus = "In curs de livrare";
                else if (comandaSelectata.Status == "In curs de livrare") noulStatus = "Livrata";

                var orderDAL = new OrderDataAccess();
                orderDAL.UpdateStatusComanda(comandaSelectata.IdComanda, noulStatus);

                comandaSelectata.Status = noulStatus;
                OnPropertyChanged(nameof(IstoricComenzi));

                if (noulStatus == "Livrata")
                {
                    IncarcareMeniu(); 
                    IncarcaDateSpecificeRolului(); 
                    MessageBox.Show("Comanda a fost livrată cu succes, iar stocurile au fost actualizate!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void FiltreazaComenzile()
        {
            if (allOrdersFromDB == null) return;

            if (OnlyActiveOrders)
            {
                var active = allOrdersFromDB.Where(c => c.Status != "Livrata" && c.Status != "Finalizata" && c.Status != "Anulata").ToList();
                IstoricComenzi.Clear();
                foreach (var c in active) IstoricComenzi.Add(c);
            }
            else
            {
                IstoricComenzi.Clear();
                foreach (var c in allOrdersFromDB) IstoricComenzi.Add(c);
            }
        }

        private bool CanSeeDetails(object parameter) => (IsAngajat || IsClient) && parameter is Order;

        private void SeeDetailsOrderImpl(object parameter)
        {
            var comanda = parameter as Order;
            if (comanda != null)
            {
                OrderDetailsWindow detaliiWin = new OrderDetailsWindow(comanda.IdComanda);
                detaliiWin.ShowDialog();
            }
        }

        private bool CanAdminExecute(object parameter) => IsAngajat;

        private void UpdatePreparatImpl(object parameter)
        {
            var preparat = parameter as Preparat;
            if (preparat != null)
            {
                string rez = menuDataAccess.UpdatePreparat(preparat.Id, preparat.Denumire, preparat.Pret, preparat.CantitatePortie, preparat.CantitateTotala);

                if (rez == "Succes")
                {
                    MessageBox.Show($"Preparatul '{preparat.Denumire}' a fost actualizat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    IncarcareMeniu();
                    IncarcaDateSpecificeRolului();
                }
                else
                {
                    MessageBox.Show($"Eroare la actualizare: {rez}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AdaugaPreparatImpl(object parameter)
        {
            if (string.IsNullOrEmpty(NewDenumire) || NewPret <= 0)
            {
                MessageBox.Show("Te rog introdu o denumire validă și un preț mai mare ca 0.");
                return;
            }

            string rez = menuDataAccess.AdaugaPreparat(NewIdCategorie, NewDenumire, NewPret, NewCantitatePortie, NewCantitateTotala, NewImaginePath);

            if (rez == "Succes")
            {
                MessageBox.Show("Preparatul a fost adăugat cu succes!");
                IncarcareMeniu();
            }
            else
            {
                MessageBox.Show($"Eroare la adăugare: {rez}");
            }
        }

        private void StergePreparatImpl(object parameter)
        {
            var preparat = parameter as Preparat;
            if (preparat != null)
            {
                var result = MessageBox.Show($"Ești sigur că vrei să ștergi preparatul '{preparat.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string rez = menuDataAccess.StergePreparat(preparat.Id);
                    if (rez == "Succes")
                    {
                        MessageBox.Show("Preparatul a fost șters!");
                        IncarcareMeniu();
                    }
                    else
                    {
                        MessageBox.Show($"Eroare la ștergere: {rez}");
                    }
                }
            }
        }

        private void ViewCartImpl(object parameter)
        {
            CartWindow cartWindow = new CartWindow(CartItems);

            if (cartWindow.DataContext is CartWindowVM cartVM)
            {
                cartVM.ConfigurareInitialaDateLivrare(this.UserAddress, this.UserPhone, this.CurentUserId);
            }

            cartWindow.ShowDialog();
            IncarcaDateSpecificeRolului();
        }

        private bool CanAnuleaza(object parameter)
        {
            if (parameter is Order comanda)
            {
                return IsClient && (comanda.Status == "Inregistrata");
            }
            return false;
        }

        private void AnuleazaComandaImpl(object parameter)
        {
            var comanda = parameter as Order;
            if (comanda != null)
            {
                var result = MessageBox.Show($"Ești sigur că vrei să anulezi comanda #{comanda.IdComanda}?", "Confirmare Anulare", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var orderDAL = new OrderDataAccess();
                    orderDAL.UpdateStatusComanda(comanda.IdComanda, "Anulata");

                    comanda.Status = "Anulata";
                    MessageBox.Show("Comanda a fost anulată cu succes.");

                    IncarcaDateSpecificeRolului();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}