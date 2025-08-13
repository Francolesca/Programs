using System;

namespace Starbucks.Application.Exceptions;

public sealed record ValidationError(
    string PropertyName,
    string ErrorMessage);