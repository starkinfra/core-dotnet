using System;
using System.Collections.Generic;
using EllipticCurve;
using StarkCore;


namespace StarkCore.Utils;

public static class Checks
{
    public static User CheckUser(User user)
    {
        if (user is null)
        {
            user = Settings.User;
        }
        if (user != null)
        {
            return user;
        }
        throw new Exception("A user is required to access our API. Check our README: https://github.com/starkinfra/sdk-dotnet/");

    }

    public static string CheckEnvironment(string environment)
    {
        List<string> environments = new List<string>() { "production", "sandbox" };
        if (!environments.Contains(environment))
        {
            throw new Exception("Select a valid environment: " + string.Join(", ", environments));
        }
        return environment;
    }

    public static string CheckPrivateKey(string pem)
    {
        try
        {
            PrivateKey privateKey = PrivateKey.fromPem(pem);
            if (privateKey.curve.name != "secp256k1")
            {
                throw new Exception();
            }
        }
        catch
        {
            throw new Exception("Private-key must be valid secp256k1 ECDSA string in pem format");
        }
        return pem;
    }

    public static DateTime CheckDateTime(string data)
    {
        return DateTime.Parse(data);
    }

    public static DateTime? CheckNullableDateTime(string data)
    {
        if (data is null)
        {
            return null;
        }
        return CheckDateTime(data);
    }
}
