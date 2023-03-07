using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Domain.Inventory
{
    using Currency;

    public class Wallet : IWallet
    {
        private Dictionary<CurrencyType, Currency> Money;

        public Wallet()
        {
            Money = new Dictionary<CurrencyType, Currency>();
        }

        public Wallet(IReadOnlyCollection<Currency> money)
        {
            Money = new Dictionary<CurrencyType, Currency>(
                money.Select(m => new KeyValuePair<CurrencyType, Currency>(m.Type, m)));
        }

        public void AdditionMoney(Currency money)
        {
            if (!IsContainType(money.Type))
                Money.Add(money.Type, money);
            else
                Money[money.Type] = Money[money.Type].Add(money);
        }

        public void SubtractMoney(Currency money)
        {
            if (!IsContainType(money.Type))
                throw new ArgumentException("指定型のお金を所持していません");

            Money[money.Type] = Money[money.Type].Subtract(money);
        }

        public IReadOnlyCollection<Currency> GetMoney() => Money.Values;

        public Currency GetMoney(CurrencyType type)
        {
            if (!IsContainType(type))
                throw new ArgumentException("指定型のお金を所持していません");

            return Money[type];
        }

        public bool IsContainType(CurrencyType type) => Money.ContainsKey(type);
    }
}
