using System.ComponentModel.DataAnnotations;
namespace GymApp.Core.Interfaces;
public interface IVersioned
{
    int RawVersion { get; }

    [Timestamp]
    byte[]? RowVersion { get; }

    string RowVersionHex();

    string RowVersionHex(byte[]? anotherVersion);
}