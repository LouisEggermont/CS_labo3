namespace Company.Function.Models;
using System;

public class Registration
{
    public Guid RegistrationId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Zipcode { get; set; }
    public int Age { get; set; }
    public bool IsFirstTimer { get; set; }

public override string ToString()
{
    return $"RegistrationId: {RegistrationId}\nLastName: {LastName}\nFirstName: {FirstName}\nEmail: {Email}\nZipcode: {Zipcode}\nAge: {Age}\nIsFirstTimer: {IsFirstTimer}";
}

}
