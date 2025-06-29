﻿namespace AprovaFacil.Domain.Results;

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Unathorized,
    InternalError,
    Forbidden,
}

public class ErrorDetail
{
    public ErrorType Type { get; set; }
    public String Message { get; set; } = String.Empty;
}

public class Result<T>
{
    public Boolean IsSuccess { get; }
    public T? Value { get; }
    public ErrorDetail? Error { get; }

    public Boolean IsFailure => !IsSuccess;

    private Result(Boolean isSuccess, T? value, ErrorDetail? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(ErrorType type, String message) => new Result<T>(false, default, new ErrorDetail { Type = type, Message = message });
}

public class Result
{
    public Boolean IsSuccess { get; }
    public ErrorDetail? Error { get; }

    public Boolean IsFailure => !IsSuccess;

    private Result(Boolean isSuccess, ErrorDetail? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(ErrorType type, String message) => new(false, new ErrorDetail { Type = type, Message = message });

    public static Result Failure(ErrorDetail errorDetail) => new(false, errorDetail);
}