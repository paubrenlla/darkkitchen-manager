namespace DarkKitchen.Domain.Users;

public class Address
{
    public Address(string street, string number, string apartment, string city, string country)
    {
        Street = street;
        Number = number;
        Apartment = apartment;
        City = city;
        Country = country;
    }

    public string Street { get; private set; }
    public string Number { get; private set; }
    public string Apartment { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
}
