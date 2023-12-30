using Xunit;
using System;
using StarkCore;
using StarkCore.Utils;
using System.Collections.Generic;
using static StarkCore.Utils.Api;
using System.Transactions;

namespace StarkCoreTests
{
    public class Transaction : Resource
    {
        public long Amount { get; }

        public Transaction(long amount, string id = null) : base(id)
        {
            Amount = amount;
        }

    }


    public class TestRestGet
    {

        public readonly User user = TestUser.SetDefaultProject();

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

        [Fact]

        public void Get()
        {
            string cursor = null;
            (string resourceName, ResourceMaker resourceMaker) = Resource();
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
    }

}