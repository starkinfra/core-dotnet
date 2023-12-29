using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using EllipticCurve;
using StarkCore;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace StarkCore.Utils
{
    public static class Parse
    {
        public static SubResource ParseAndVerify(string content, string signature, string resourceName,
            Api.ResourceMaker resourceMaker, User user, string host, string apiVersion, string sdkVersion, string key = null)
        {
            string verifiedContent = Verify(content, signature, user, host, apiVersion, sdkVersion);
            dynamic json = Utils.Json.Decode(verifiedContent);
            if (key != null)
            {
                json = json[key];
            }
            return Api.FromApiJson(resourceMaker, json);
        }

        public static string Verify(string content, string signature, User user, string host, string apiVersion, string sdkVersion)
        {
            Signature signatureObject;
            try
            {
                signatureObject = Signature.fromBase64(signature);
            }
            catch
            {
                throw new Error.InvalidSignatureError("The provided signature is not valid");
            }

            if (VerifySignature(content, signatureObject, user, host, sdkVersion, apiVersion))
            {
                return content;
            }
            if (VerifySignature(content, signatureObject, user, host, sdkVersion, apiVersion, true))
            {
                return content;
            }

            throw new Error.InvalidSignatureError("The provided signature and content do not match the Stark public key");
        }

        public static bool VerifySignature(string content, Signature signature, User user, string host, string apiVersion, string sdkVersion, bool refresh = false)
        {

            PublicKey publicKey = Utils.Cache.StarkPublicKey;

            if (publicKey is null || refresh)
            {
                publicKey = GetPublicKeyPem(user, host, sdkVersion, apiVersion);
            }

            return Ecdsa.verify(content, signature, publicKey);
        }

        public static PublicKey GetPublicKeyPem(User user, string host, string apiVersion, string sdkVersion)
        {
            dynamic json = Utils.Request.Fetch(
                host: host,
                apiVersion: apiVersion,
                sdkVersion: sdkVersion,
                method: Request.Get,
                path: "public-key",
                query: new Dictionary<string, object> { { "limit", 1 } },
                user: user
            ).Json();
            List<JObject> publicKeys = json.publicKeys.ToObject<List<JObject>>();
            dynamic publicKey = publicKeys.First();
            string content = publicKey.content;
            PublicKey publicKeyObject = PublicKey.fromPem(content);
            Utils.Cache.StarkPublicKey = publicKeyObject;
            return publicKeyObject;
        }
    }
}
