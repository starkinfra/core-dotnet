using Xunit;
using System;
using StarkCore;
using StarkCore.Utils;
using System.Collections.Generic;
using static StarkCore.Utils.Api;
using System.Transactions;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Data;
using System.Linq;

namespace StarkCoreTests
{
    public class Transaction : Resource
    {
        public long Amount { get; }

        public Transaction(long amount, string id = null) : base(id)
        {
            Amount = amount;
        }

        internal static (string resourceName, ResourceMaker resourceMaker) Resource()
        {
            return (resourceName: "Transaction", resourceMaker: ResourceMaker);
        }

        internal static Resource ResourceMaker(dynamic json)
        {
            string id = json.id;
            long amount = json.amount;

            return new Transaction(
                id: id, amount: amount
            );
        }

    }

    public partial class Webhook : Resource
    {
        public string Url { get; }
        public List<string> Subscriptions { get; }

        public Webhook(string url, List<string> subscriptions = null, string id = null) : base(id)
        {
            Url = url;
            Subscriptions = subscriptions;

        }

        internal static (string resourceName, ResourceMaker resourceMaker) Resource()
        {
            return (resourceName: "webhook", resourceMaker: ResourceMaker);
        }

        internal static Resource ResourceMaker(dynamic json)
        {
            string Url = json.url;
            List<string> Subscriptions = json.Subscriptions;
            string Id = json.id;

            return new Webhook(url: Url, subscriptions: Subscriptions, id: Id);
        }
    }

    public partial class Invoice : Resource
    {
        public string Name { get; }
        public string TaxID { get; }
        public long Amount { get; }

        public Invoice(long amount, string name, string taxID, string id = null) : base(id)
        {
            Amount = amount;
            Name = name;
            TaxID = taxID;
        }

        internal static (string resourceName, ResourceMaker resourceMaker) Resource()
        {
            return (resourceName: "invoice", resourceMaker: ResourceMaker);
        }

        internal static Resource ResourceMaker(dynamic json)
        {
            string id = json.id;
            long amount = json.amount;
            string name = json.name;
            string taxID = json.taxId;

            return new Invoice(id: id, amount: amount, name: name, taxID: taxID);
        }
    }

    public class TestRestGet
    {

        public readonly User user = TestUser.SetDefaultProject();

        [Fact]

        public void GetPage()
        {
            string cursor = null;
            (string resourceName, ResourceMaker resourceMaker) = Transaction.Resource();
            (List<SubResource> page, string pageCursor) = Rest.GetPage(
                host: "bank",
                apiVersion: "v2",
                sdkVersion: "0.2.0",
                resourceName: "Transaction",
                resourceMaker: resourceMaker,
                query: new Dictionary<string, object> {
                    { "cursor", cursor }
                },
                user: user
            );

            List<Transaction> transactions = new List<Transaction>();

            foreach (SubResource subResource in page)
            {
                transactions.Add(subResource as Transaction);
            }


            Transaction transaction = transactions[0];
            Console.WriteLine(transaction);

            int amount = (int)transaction.Amount;

            Assert.IsType<int>(amount);
        }

        [Fact]

        public void GetList()
        {
            (string resourceName, ResourceMaker resourceMaker) = Invoice.Resource();
            string cursor = null;
            List<Invoice> invoices = Rest.GetList(  
                host: "bank",
                apiVersion: "v2",
                sdkVersion: "0.2.0",
                resourceName: resourceName,
                resourceMaker: resourceMaker,
                query: new Dictionary<string, object> {
                    { "cursor", cursor },
                    { "limit", 2 }
                },
                user: user
            ).Cast<Invoice>().ToList();

            Assert.Equal(2, invoices.Count);
            Assert.True(invoices.First().ID != invoices.Last().ID);
        }
    }

    public class TestRestPost
    {
        public readonly User user = TestUser.SetDefaultProject();

        [Fact]

        public void Post()
        {
            (string resourceName, ResourceMaker resourceMaker) = Invoice.Resource();
            List<Invoice> invoices = Rest.Post(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    resourceName: resourceName,
                    resourceMaker: resourceMaker,
                    entities: new List<Invoice>() { new Invoice(amount: 100, name: "Arya Stark", taxID: "012.345.678-90") },
                    user: user
                ).ToList().ConvertAll(o => (Invoice)o);

            foreach (Invoice invoice in invoices)
            {
                Assert.NotNull(invoice.ID);
            }
        }

    }

    public class TestRestPatch
    {
        public readonly User user = TestUser.SetDefaultProject();

        [Fact]

        public void Patch()
        {
            (string resourceName, ResourceMaker resourceMaker) = Invoice.Resource();

            string cursor = null;
            List<Invoice> invoices = Rest.GetList(
                host: "bank",
                apiVersion: "v2",
                sdkVersion: "0.2.0",
                resourceName: resourceName,
                resourceMaker: resourceMaker,
                query: new Dictionary<string, object> {
                    { "cursor", cursor },
                    { "limit", 2 }
                },
                user: user
            ).Cast<Invoice>().ToList();

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "amount", 0 }
            };

            Invoice invoice = Rest.PatchId(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    resourceName: resourceName,
                    resourceMaker: resourceMaker,
                    payload: data,
                    id: invoices.First().ID.ToString(),
                    user: user
                ) as Invoice;

            Assert.Equal(0, invoice.Amount);
        }
    }

    public class TestRestDelete
    {
        public readonly User user = TestUser.SetDefaultProject();

        [Fact]

        public void Delete()
        {
            (string resourceName, ResourceMaker resourceMaker) = Webhook.Resource();

            string cursor = null;
            Webhook webhook = Rest.PostSingle(
                host: "bank",
                apiVersion: "v2",
                sdkVersion: "0.2.0",
                resourceName: resourceName,
                resourceMaker: resourceMaker,
                entity: new Webhook(url: "https://webhook.site/" + Guid.NewGuid(), subscriptions: new List<string> { "transfer", "boleto", "boleto-payment", "utility-payment", "boleto-holmes" }),
                user: user
            ) as Webhook;

            Webhook deletedWebhook = Rest.DeleteId(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    resourceName: resourceName,
                    resourceMaker: resourceMaker,
                    id: webhook.ID.ToString(),
                    user: user
                ) as Webhook;

            Assert.NotNull(deletedWebhook);

        }
    }

    public class TestRestRaw
    {

        public readonly User user = TestUser.SetDefaultProject();

        [Fact]

        public void Get()
        {
            string path = "/invoice";
            Dictionary<string, object> query = new Dictionary<string, object>() { { "limit", 10 } };

            JObject request = Rest.GetRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    query: query,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            Assert.NotNull(request["invoices"][0]["id"]);
        }

        [Fact]

        public void Post()
        {
            string path = "/invoice";
            Dictionary<string, object> data = new Dictionary<string, object>() {
                {
                    "invoices", new List<Dictionary<string, object>>() { new Dictionary<string, object>()
                        {
                            { "amount", 100 },
                            { "name", "Iron Bank S.A." },
                            { "taxId", "20.018.183/0001-80" }
                        },

                    }
                }
            };

            JObject request = Rest.PostRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    payload: data,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            Assert.NotNull(request["invoices"][0]["id"]);
        }

        [Fact]

        public void Patch()
        {
            string path = "/invoice";

            JObject initialState = Rest.GetRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    query: new Dictionary<string, object>() { { "limit", 1 } },
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            path += "/" + initialState["invoices"][0]["id"].ToString();

            Dictionary<string, object> data = new Dictionary<string, object>() { { "amount", 0 } };

            Rest.PatchRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    payload: data,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                );

            JObject finalState = Rest.GetRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    query: null,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            Assert.NotNull(finalState["invoice"]["id"]);
        }

        [Fact]

        public void Put()
        {
            string path = "/split-profile";
            Dictionary<string, object> data = new Dictionary<string, object>() {
                {
                    "profiles", new List<Dictionary<string, object>>() {
                        new Dictionary<string, object>()
                        {
                            { "interval", "day" },
                            { "delay", 0 }
                        }
                    }
                }
            };

            JObject request = Rest.PutRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: path,
                    payload: data,
                    query: null,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            Assert.Equal(request["profiles"][0]["delay"], 0);
            Assert.Equal(request["profiles"][0]["interval"], "day");

        }

        [Fact]

        public void Delete()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {
                    "transfers", new List<Dictionary<string, object>>() {

                        new Dictionary<string, object>()
                        {
                            { "amount", 10000 },
                            { "name", "Steve Rogers" },
                            { "taxId", "851.127.850-80" },
                            { "bankCode",  "001" },
                            { "branchCode", "1234" },
                            { "accountNumber", "123456-0" },
                            { "accountType", "checking" },
                            { "scheduled", DateTime.Now.AddDays(1) },
                            { "externalId", Guid.NewGuid().ToString() }
                        }
                    }
                }
            };

            JObject create = Rest.PostRaw(
                    host: "bank",
                    apiVersion: "v2",
                    sdkVersion: "0.2.0",
                    path: "/transfer/",
                    payload: data,
                    query: null,
                    user: user,
                    prefix: "Joker",
                    raiseException: false
                ).Json();

            JObject deleted = Rest.DeleteRaw(
                host: "bank",
                apiVersion: "v2",
                sdkVersion: "0.2.0",
                path: "/transfer/" + create["transfers"][0]["id"].ToString(),
                query: null,
                user: user,
                prefix: "Joker",
                raiseException: false
               ).Json();

            Assert.NotNull(deleted["transfer"]["id"]);

        }

    }

}