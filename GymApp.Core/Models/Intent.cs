
using System;

namespace MediatR;

public interface Intent
{
    DateTime DateOccurred { get; }

    string GetKey();
}