﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using GW2Trader.Command;
using GW2Trader.Data;
using GW2Trader.Model;

namespace GW2Trader.ViewModel
{
    public class InvestmentViewModel : BaseViewModel
    {
        #region Observable Members

        private string _investmentListName;
        private string _investmentListDescription;
        private InvestmentWatchlistModel _selectedWatchlist;
        private List<InvestmentWatchlistModel> _selectedWatchlists;
        private ObservableCollection<InvestmentWatchlistModel> _watchlists;
        #endregion

        private readonly IGameDataContextProvider _contextProvider;
        private List<GameItemModel> _items;

        public enum SelectionMode
        {
            All,
            Current,
            Past
        };

        public InvestmentViewModel(IGameDataContextProvider contextProvider, List<GameItemModel> items)
        {
            ViewModelName = "Investments";
            _contextProvider = contextProvider;
            _items = items;

            BuildWatchlists();

            if (Watchlists.Any())
            {
                SelectedWatchlist = Watchlists[0];
            }
        }

        public string InvestmentListName
        {
            get { return _investmentListName; }
            set
            {
                _investmentListName = value;
                RaisePropertyChanged("InvestmentListName");
            }
        }

        public string InvestmentListDescription
        {
            get { return _investmentListDescription; }
            set
            {
                _investmentListDescription = value;
                RaisePropertyChanged("InvestmentListDescription");
            }
        }

        public InvestmentWatchlistModel SelectedWatchlist
        {
            get { return _selectedWatchlist; }
            set
            {
                _selectedWatchlist = value;
                RaisePropertyChanged("SelectedWatchlist");
                InvestmentListName = _selectedWatchlist != null ?
                                     _selectedWatchlist.Name : null;
                InvestmentListDescription = _selectedWatchlist != null ?
                    _selectedWatchlist.Description : null;
            }
        }

        public List<InvestmentWatchlistModel> SelectedWatchlists
        {
            get { return _selectedWatchlists; }
            set
            {
                _selectedWatchlists = value;
                RaisePropertyChanged("SelectedWatchlists");
            }
        }

        public ObservableCollection<InvestmentWatchlistModel> Watchlists
        {
            get { return _watchlists; }
            set
            {
                _watchlists = value;
                RaisePropertyChanged("Watchlists");
            }
        }

        public List<GameItemModel> SharedItems
        {
            get { return _items; }
        }

        public void AddInvestmentList()
        {
            InvestmentWatchlistModel newWatchlist = new InvestmentWatchlistModel
            {
                Name = InvestmentListName,
                Description = InvestmentListDescription
            };
            using (var context = _contextProvider.GetContext())
            {
                context.InvestmentWatchlists.Add(newWatchlist);
                context.Save();
                newWatchlist.Id = context.InvestmentWatchlists.ToList().Last().Id;
            }
            Watchlists.Add(newWatchlist);
        }

        public void UpdateInvestmentList()
        {
            using (var context = _contextProvider.GetContext())
            {
                var listToUpdate = context.InvestmentWatchlists.Single(l => l.Id == SelectedWatchlist.Id);
                listToUpdate.Name = InvestmentListName;
                listToUpdate.Description = InvestmentListDescription;
                context.Save();
            }
            SelectedWatchlist.Name = InvestmentListName;
            SelectedWatchlist.Description = InvestmentListDescription;
        }

        public void DeleteInvestmentList(InvestmentWatchlistModel watchlist)
        {
            using (var context = _contextProvider.GetContext())
            {
                var watchlistToDelete = context.InvestmentWatchlists.Single(wl => wl.Id == watchlist.Id);
                context.InvestmentWatchlists.Remove(watchlistToDelete);
                context.Save();
            }
            _watchlists.Remove(watchlist);
            if (Watchlists.Any())
            {
                SelectedWatchlist = Watchlists.Last();
            }
        }

        public void DeleteInvestment(InvestmentModel investment)
        {
            using (var context = _contextProvider.GetContext())
            {
                InvestmentWatchlistModel contextWatchlist =
                    context.InvestmentWatchlists.Single(wl => wl.Id == SelectedWatchlist.Id);
                InvestmentModel investmentToDelete = context.Investments.Single(inv => inv.Id == investment.Id);
                contextWatchlist.Items.Remove(investmentToDelete);
                context.Investments.Remove(investmentToDelete);
                context.Save();
            }
            SelectedWatchlist.Items.Remove(investment);
        }

        public void AddInvestment(InvestmentModel investment)
        {
            
        }

        private void BuildWatchlists()
        {
            using (var context = _contextProvider.GetContext())
            {
                var watchlists = context.InvestmentWatchlists.Include(wl => wl.Items).ToList();
                Watchlists = new ObservableCollection<InvestmentWatchlistModel>(watchlists);
            }

            // TODO

        }

        #region Commands

        private RelayCommand _addInvestmentListCommand;

        public RelayCommand AddInvestmentListCommand
        {
            get
            {
                if (_addInvestmentListCommand == null)
                    _addInvestmentListCommand = new AddInvestmentListCommand();
                return _addInvestmentListCommand;
            }
        }

        private RelayCommand _deleteInvestmentListCommand;

        public RelayCommand DeleteInvestmentListCommand
        {
            get
            {
                if (_deleteInvestmentListCommand == null)
                    _deleteInvestmentListCommand = new DeleteInvestmentListCommand();
                return _deleteInvestmentListCommand;
            }
        }

        private RelayCommand _updateInvestmentListCommand;

        public RelayCommand UpdateInvestmentListCommand
        {
            get
            {
                if (_updateInvestmentListCommand == null)
                    _updateInvestmentListCommand = new UpdateInvestmentListCommand();
                return _updateInvestmentListCommand;
            }
        }

        private RelayCommand _addInvestmentCommand;

        public RelayCommand AddInvestmentCommand
        {
            get
            {
                if (_addInvestmentCommand == null)
                    _addInvestmentCommand = new AddInvestmentCommand();
                return _addInvestmentCommand;
            }
        }

        #endregion
    }
}