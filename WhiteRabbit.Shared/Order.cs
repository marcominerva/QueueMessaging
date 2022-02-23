using System;

namespace WhiteRabbit.Shared;

public class Order
{
    public int Number { get; set; }

    public string User { get; set; }

    public DateTime Date { get; set; }

    public double Amount { get; set; }
}
