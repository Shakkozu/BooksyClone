using Newtonsoft.Json;

namespace BooksyClone.Contract.Shared;

public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public static Money FromDecimal(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }
    
    public static Money FromString(string amount, string currency)
    {
        return new Money(decimal.Parse(amount), currency);
    }
    
    public static Money PLN(decimal amount)
    {
        return new Money(amount, "PLN");
    }
    
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot add money of different currencies");
        }

        return new Money(a.Amount + b.Amount, a.Currency);
    }
    
    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot subtract money of different currencies");
        }

        return new Money(a.Amount - b.Amount, a.Currency);
    }
    
    
    
    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }


    public static Money FromJson(string argPrice)
    {
        var jsonMoney = JsonConvert.DeserializeObject<Money>(argPrice);
        if(jsonMoney == null)
            throw new ArgumentException("Invalid json format");
        return new Money(jsonMoney.Amount, jsonMoney.Currency);
    }
}