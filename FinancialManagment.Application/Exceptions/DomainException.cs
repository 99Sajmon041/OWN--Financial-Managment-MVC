namespace FinancialManagment.Application.Exceptions;

public sealed class DomainException(string message) : Exception(message) { }
