namespace ExpenseTracker.Application.DTOs;

public record ReceiptDto(
    Guid Id,
    string FileName,
    string ContentType,
    long FileSize,
    DateTime UploadDate);
