namespace DarkKitchen.Domain;

public class Address
{
    public Address(string street, string number, string? apartment, string city, string country)
    {
        ValidateRequired(street, nameof(Street));
        ValidateRequired(number, nameof(Number));
        ValidateRequired(city, nameof(City));
        ValidateRequired(country, nameof(Country));

        Street = street;
        Number = number;
        Apartment = apartment;
        City = city;
        Country = country;
    }

    public string Street { get; private set; }
    public string Number { get; private set; }
    public string? Apartment { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }

    private static void ValidateRequired(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }
    }
}
