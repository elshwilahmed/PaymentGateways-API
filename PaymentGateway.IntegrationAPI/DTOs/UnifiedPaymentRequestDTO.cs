using System.Text.Json.Serialization;

public class UnifiedPaymentRequestDTO
{
    public int Amount { get; set; }
    public string Currency { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; } = "EG";         
    public string City { get; set; } = "Not Provided";  
    public string Street { get; set; } = "NA";          
    public string Building { get; set; } = "NA";
    public string Floor { get; set; } = "NA";
    public string Apartment { get; set; } = "NA";
}

