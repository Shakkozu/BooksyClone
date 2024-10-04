using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;


public class RegisterNewBusinessRequest
{
    public Guid CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }

    // Informacje o biznesie
    public required string BusinessName { get; set; }  // Nazwa biznesu
    public required BusinessType BusinessType { get; set; }  // Typ biznesu (np. salon, restauracja, itd.)
    public required string BusinessNIP { get; set; }  // Numer identyfikacji podatkowej (NIP)
    public required string BusinessAddress { get; set; }  // Adres siedziby biznesu
    public required string BusinessPhoneNumber { get; set; }  // Numer telefonu kontaktowego
    public required string BusinessEmail { get; set; }  // Adres e-mail do kontaktu

    // Informacje o użytkowniku
    public Guid UserId { get; set; }
    public required string UserFullName { get; set; }  // Imię i nazwisko właściciela
    public required string UserIdNumber { get; set; }  // Numer dowodu osobistego
    public required string UserEmail { get; set; }  // Adres e-mail właściciela
    public required string UserPhoneNumber { get; set; }  // Numer telefonu właściciela

    // Dokumenty potwierdzające
    public IFormFile BusinessProofDocument { get; set; }  // Dokument potwierdzający istnienie biznesu (np. umowa, zaświadczenie)
    public IFormFile UserIdentityDocument { get; set; }  // Dokument potwierdzający tożsamość właściciela (np. dowód osobisty)

    // Zgoda prawna
    public bool LegalConsent { get; set; }  // Potwierdzenie zgody prawnej
    public required string LegalConsentContent { get; set; }
}

public enum BusinessType
{
    Barber,
    Hair,
    Nails,
    Brows,
    BeautySalons
}


internal class RegisterNewBusiness
{
}
