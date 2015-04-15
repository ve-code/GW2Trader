﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2TPApiWrapper.Entities;
using GW2TPApiWrapper.Wrapper;
using GW2Trader.Model;
using Xunit;
using GW2Trader.Service;
using GW2TraderTestxUnit.Mock;
using GW2TraderTestxUnit.TestData;

namespace GW2TraderTestxUnit.ServiceTest
{
    public class GameItemModelTest
    {
        const float CommissionFee = 0.15f;

        [Fact]
        public void MarginShouldBeCalculatedCorrectly()
        {
            GameItemModel item = new GameItemModel
            {
                BuyPrice = 100,
                SellPrice = 200
            };

            int expectedMargin = (int)Math.Round(item.SellPrice * (1 - CommissionFee) - item.BuyPrice);

            Assert.Equal(expectedMargin, item.Margin);
        }

        [Fact]
        public void ReturnOfInvestmentShouldBeCalculatedCorrectly()
        {
            GameItemModel item = new GameItemModel
            {
                BuyPrice = 100,
                SellPrice = 200
            };

            // ROI = net revenue(margin) / invested money
            int expectedROI = (int)((float)item.Margin / (float)item.BuyPrice * 100);
            Assert.Equal(expectedROI, item.ROI);
        }
        
    }
}
