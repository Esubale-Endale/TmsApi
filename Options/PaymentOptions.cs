
public class PaymentOptions
{

    public required string GatewayUrl { get; init; }
    public decimal MaxDepositBirr(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    // TODO1:Create a class called PaymentOptions with two properties:
    //- GatewayUrl (string, required use [Required] attribute)
    //- MaxDepositBirr (decimal, range 100-100000 use [Range] attribute)
    // Stuck? public class PaymentOptions { [Required] public required string GatewayUrl { get; init; }... }

    // TODO2:In Program.cs, bind PaymentOptions to the "Payments" section of appsettings.json
    // and enable startup validation.
    // Stuck? builder.Services.AddOptions<PaymentOptions>()
    //.BindConfiguration("Payments")
    // ValidateDataAnnotations()
    //.ValidateOnStart();

    // TODO3:Test it delete the "Payments" section from appsettings.json and run the app.
    // What error do you see? Does the app start or crash immediately?
}