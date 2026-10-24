namespace WebStore.Services;

internal static class Pln
{
    // Największa wartość, którą decimal zachowuje z dokładnością do grosza (P11).
    internal const decimal MaxValue = 792281625142643375935439503.35m;

    internal static void Validate(decimal value, string name)
    {
        if (value < 0 || value > MaxValue || decimal.Round(value, 2) != value)
            throw new ArgumentException("Kwota PLN musi mieścić się w zakresie 0–792281625142643375935439503,35 i mieć maksymalnie dwa miejsca po przecinku.", name);
    }

    internal static decimal Total(IEnumerable<(decimal Amount, int Quantity)> lines)
    {
        decimal grosz = 0;
        try
        {
            foreach (var (amount, quantity) in lines)
            {
                Validate(amount, nameof(amount));
                // Całkowite grosze zapobiegają utracie cyfr ułamkowych przy dużych sumach.
                grosz += amount * 100m * quantity;
            }
            return grosz / 100m;
        }
        catch (OverflowException error)
        {
            throw new ArgumentException("Wyliczona kwota PLN przekracza maksymalną dopuszczalną wartość.", nameof(lines), error);
        }
    }
}
