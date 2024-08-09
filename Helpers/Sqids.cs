///
/// Source: https://github.com/sqids/sqids-dotnet#ensuring-an-id-is-canonical
/// 

using Sqids;

namespace BlazorADAuth.Helpers;

public static class Sqids
{
    /// <summary>
    /// Decodes a SQID-encoded string back to its integer representation.
    /// </summary>
    /// <param name="encodedId">SQID-encoded string to decode</param>
    /// <param name="encoder">
    /// <para>
    /// Optional parameter. Mainly for using DI-provided instances
    /// of <see cref="SqidsEncoder{T}"/>.
    /// </para>
    /// <para>
    /// If <paramref name="encoder"/> is null, a new method-scoped instance will be constructed.
    /// </para>
    /// </param>
    /// <returns>Decoded integer or <c>null</c> on failure.</returns>
    public static int DecodeSingleOrDefault(string encodedId, SqidsEncoder<int>? encoder = null)
    {
        encoder ??= new SqidsEncoder<int>();

        if (encoder.Decode(encodedId) is [var decodedId] &&
            encodedId == encoder.Encode(decodedId))
        {
            return decodedId;
        }
        else
        {
            throw new Exception($"Parameter `{nameof(encodedId)}` failed to decode.");
        }
    }    
}
