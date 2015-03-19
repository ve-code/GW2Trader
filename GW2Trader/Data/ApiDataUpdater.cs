﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2TPApiWrapper.Wrapper;
using GW2Trader.Model;
using System.Threading.Tasks;

namespace GW2Trader.Data
{
    public class ApiDataUpdater : IApiDataUpdater
    {
        private readonly ITradingPostApiWrapper _tpApiWrapper;

        public ApiDataUpdater(ITradingPostApiWrapper wrapper)
        {
            _tpApiWrapper = wrapper;
        }

        public void UpdateItemData(GameItemModel item)
        {
            var updatedData = _tpApiWrapper.ItemDetails(item.ItemId);
            item.IconUrl = updatedData.IconUrl;
            item.Name = updatedData.Name;
            item.Rarity = updatedData.Rarity;
            item.RestrictionLevel = updatedData.Level;
            item.Type = updatedData.Type;
        }

        public void UpdateCommerceData(GameItemModel item)
        {
            var updatedData = _tpApiWrapper.Listings(item.ItemId);
            item.Listing.Buys = updatedData.Buys;
            item.Listing.Sells = updatedData.Sells;
            item.CommerceDataLastUpdated = DateTime.Now;
        }

        public void UpdateCommerceData(IList<GameItemModel> items)
        {
            var ids = items.Select(i => i.ItemId).ToArray();
            var updatedPrices = _tpApiWrapper.Price(ids).ToList();

            foreach (var gameItemModel in items)
            {
                var respectivePrice = updatedPrices.Find(p => p.Id == gameItemModel.ItemId);
                if (respectivePrice != null)
                {
                    gameItemModel.SellPrice = respectivePrice.Sells.UnitPrice;
                    gameItemModel.SellListingQuantity = respectivePrice.Sells.Quantity;
                    gameItemModel.BuyPrice = respectivePrice.Buys.UnitPrice;
                    gameItemModel.BuyOrderQuantity = respectivePrice.Buys.Quantity;
                    gameItemModel.CommerceDataLastUpdated = DateTime.Now;
                }
            }
        }

        public void UpdateCommerceDataParallel(IList<GameItemModel> items)
        {
            const int itemsToProcessPerTask = 200;
            int neededTasks = (int)Math.Ceiling(items.Count/ (itemsToProcessPerTask * 1.0f));

            foreach (int taskIndex in Enumerable.Range(0,neededTasks))
            {
                // divide items in smaller subsets and process each subset in parallel
                var itemSubset = items.Skip(taskIndex * itemsToProcessPerTask)
                    .Take(itemsToProcessPerTask).ToList();
                Task.Run(() => UpdateCommerceData(itemSubset));
            }
        }
    }
}