﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using System.Linq;
using Raven.Client.Embedded;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using Raven.Json.Linq;
using Raven.Tests.Bundles.PeriodicBackups;
using Raven.Tests.Bundles.Replication.Bugs;
using Raven.Tests.Bundles.Versioning;

namespace Raven.Tryouts
{
	public class Person
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			var store = new EmbeddableDocumentStore
			{
				//Url = "http://localhost:8080",
				//DefaultDatabase = "DB9",
				//UseEmbeddedHttpServer = true
			}.Initialize();

			//store.DatabaseCommands.EnsureDatabaseExists("DB9");

			//new RavenDocumentsByEntityName().Execute(store.DatabaseCommands, new DocumentConvention());

			var watch = Stopwatch.StartNew();

			var tasks = new List<Task>();
			for (var i = 1; i <= 20; i++)
			{
				var taskNumber = i;
				tasks.Add(Task.Factory.StartNew(() => Save(store, taskNumber)));
			}

			Task.WaitAll(tasks.ToArray());

			store.Dispose();

			Console.WriteLine("Elapsed: " + watch.Elapsed.TotalSeconds + " seconds");

			Console.ReadLine();
		}

		private static void Save(IDocumentStore store, int j)
		{
			for (var i = 1; i <= 1000; i++)
			{
				store.DatabaseCommands.Put(string.Format("people/{0}/{1}", j, i), null, RavenJObject.FromObject(new Person
				{
					FirstName = "FirstName" + i,
					LastName = "LastName" + i
				}), new RavenJObject());

				//if (i % 5 == 0)
				//	Thread.Sleep(30000);

				//using (var session = store.OpenSession())
				//{
				//	session.Store(new Person
				//	{
				//		FirstName = "FirstName" + i,
				//		LastName = "LastName" + i
				//	});

				//	if (i % 100 == 0)
				//		session.SaveChanges();
				//}
			}
		}
	}
}