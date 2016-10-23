using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task.DB;
using Task.TestHelpers;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace Task
{
	[TestClass]
	public class SerializationSolutions
	{
		Northwind dbContext;

		[TestInitialize]
		public void Initialize()
		{
			dbContext = new Northwind();
		}

		[TestMethod]
		public void SerializationCallbacks()
		{
			dbContext.Configuration.ProxyCreationEnabled = false;

			var tester = new XmlDataContractSerializerTester<IEnumerable<Category>>(new NetDataContractSerializer(), true);
			var categories = dbContext.Categories.ToList();

			var c = categories.First();

			tester.SerializeAndDeserialize(categories);
		}

		[TestMethod]
		public void ISerializable() // Complete
		{
			dbContext.Configuration.ProxyCreationEnabled = false;

			var tester = new XmlDataContractSerializerTester<IEnumerable<Product>>(new NetDataContractSerializer(), true);
			var products = dbContext.Products.ToList();

			var t = (dbContext as IObjectContextAdapter).ObjectContext;
			foreach (var product in products)
			{
				t.LoadProperty(product, p => p.Category);
			}

			tester.SerializeAndDeserialize(products);
		}


		[TestMethod]
		public void ISerializationSurrogate()
		{
			dbContext.Configuration.ProxyCreationEnabled = false;
			dbContext.Configuration.LazyLoadingEnabled = true;

			var tester = new XmlDataContractSerializerTester<IEnumerable<Order_Detail>>(new NetDataContractSerializer(), true);
			var orderDetails = dbContext.Order_Details.ToList();

			tester.SerializeAndDeserialize(orderDetails);
		}

		[TestMethod]
		public void IDataContractSurrogate()
		{
			dbContext.Configuration.ProxyCreationEnabled = false;
			dbContext.Configuration.LazyLoadingEnabled = false;

			var tester = new XmlDataContractSerializerTester<IEnumerable<Order>>(new DataContractSerializer(typeof(IEnumerable<Order>), new DataContractSerializerSettings() {PreserveObjectReferences = true, IgnoreExtensionDataObject = true}), true);
			var orders = dbContext.Orders.ToList();

			var t = (dbContext as IObjectContextAdapter).ObjectContext;
			foreach (var order in orders)
			{
				t.LoadProperty(order, o => o.Shipper);
				//t.LoadProperty(order, o => o.Customer);
				//t.LoadProperty(order, o => o.Employee);
				//t.LoadProperty(order, o => o.Order_Details);
			}


			tester.SerializeAndDeserialize(orders);
		}
	}
}
